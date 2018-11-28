/* InstantVR Hand Movements
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarnes
 * email: support@passervr.com
 * version: 3.8.4
 * date: July 3, 2017
 * 
 * - Fixed Thumb curl axis without middle finger
 */

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
#if INSTANTVR_EDGE
using UnityEngine.Networking;
#endif

namespace IVR {

    public class IVR_HandMovements : IVR_HandMovementsBase {

#if INSTANTVR_EDGE
        public enum PhysicsMode {
            NoPhysics,
            BasicPhysics,
            FullPhysics
        }
        public PhysicsMode physicsMode = PhysicsMode.BasicPhysics;
        public float strength = 100;
#endif
        private const bool bodyPull = true;
        private const bool verticalBodyPull = false;

        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        private GameObject hand;
        [HideInInspector]
        private Transform forearm;
        private float forearmLength;
        [HideInInspector]
        private Transform handTarget;
        private readonly bool stretchyArms = false;
        [HideInInspector]
        private IVR_BodyMovements bodyMovements;

        private Thumb thumb = null;
        private Finger indexFinger = null;
        private Finger middleFinger = null;
        private Finger ringFinger = null;
        private Finger littleFinger = null;

        private Digit[] digits = null;

        public float thumbCurl;
        public float indexCurl;
        public float middleCurl;
        public float ringCurl;
        public float littleCurl;

        public bool easyGrab = true;

        public enum InteractionType {
            None,
            Touching,
            Pointing
        }
        public InteractionType interaction;
        public float autoActivation;
        public ControllerInput.Button activationButton;

        public GameObject focusPointObj;
        [HideInInspector]
        public Vector3 focusPoint;
        public GameObject pointingAtObject;
        public GameObject touchingObject;
        [HideInInspector]
        private IVR_Interaction inputModule;

        [HideInInspector]
        public IVR_Kinematics handPhysics = null;

        [HideInInspector]
        private Collider collidedObject;

        [HideInInspector]
        public Transform handPalm;
        [HideInInspector]
        public GameObject handObj;
        [HideInInspector]
        private Vector3 palmOffset;
        [HideInInspector]
        private IVR_HandMovementsBase otherHand;

        [HideInInspector]
        private Vector3 handRightAxis, handRightAxisThumb;

        private enum LeftHandBones {
            ThumbProximal = 24,
            ThumbIntermediate = 25,
            ThumbDistal = 26,
            IndexProximal = 27,
            IndexIntermediate = 28,
            IndexDistal = 29,
            MiddleProximal = 30,
            MiddleIntermediate = 31,
            MiddleDistal = 32,
            RingProximal = 33,
            RingIntermediate = 34,
            RingDistal = 35,
            LittleProximal = 36,
            LittleIntermediate = 37,
            LittleDistal = 38
        };
        private enum RightHandBones {
            ThumbProximal = 39,
            ThumbIntermediate = 40,
            ThumbDistal = 41,
            IndexProximal = 42,
            IndexIntermediate = 43,
            IndexDistal = 44,
            MiddleProximal = 45,
            MiddleIntermediate = 46,
            MiddleDistal = 47,
            RingProximal = 48,
            RingIntermediate = 49,
            RingDistal = 50,
            LittleProximal = 51,
            LittleIntermediate = 52,
            LittleDistal = 53
        };

        public override void StartMovements(InstantVR ivr) {
            this.ivr = ivr;

            handTarget = this.transform;

            hand = this.gameObject;
            thumb = new Thumb();
            indexFinger = new Finger();
            middleFinger = new Finger();
            ringFinger = new Finger();
            littleFinger = new Finger();

            animator = ivr.GetComponentInChildren<Animator>();
            bodyMovements = ivr.GetComponent<IVR_BodyMovements>();

            if (animator != null) {
                if (this.transform == ivr.leftHandTarget) {
                    hand = animator.GetBoneTransform(HumanBodyBones.LeftHand).gameObject;

                    thumb.transform = animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
                    indexFinger.transform = animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
                    middleFinger.transform = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                    ringFinger.transform = animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
                    littleFinger.transform = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);

                    forearm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);

                } else {
                    hand = animator.GetBoneTransform(HumanBodyBones.RightHand).gameObject;

                    thumb.transform = animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
                    indexFinger.transform = animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
                    middleFinger.transform = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                    ringFinger.transform = animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
                    littleFinger.transform = animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);

                    forearm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                }

                DeterminePalmPosition();

                if (indexFinger.transform != null && littleFinger.transform != null) {
                    handRightAxisThumb = thumb.transform.InverseTransformDirection(littleFinger.transform.position - indexFinger.transform.position);
                    handRightAxis = indexFinger.transform.InverseTransformDirection(littleFinger.transform.position - indexFinger.transform.position);
                } else if (indexFinger.transform != null && middleFinger.transform != null) {
                    handRightAxisThumb = thumb.transform.InverseTransformDirection(middleFinger.transform.position - indexFinger.transform.position);
                    handRightAxis = indexFinger.transform.InverseTransformDirection(middleFinger.transform.position - indexFinger.transform.position);
                } else {
                    handRightAxisThumb = -ivr.characterTransform.right;
                    handRightAxis = -ivr.characterTransform.right;
                }
                if (this.transform == ivr.leftHandTarget) {
                    handRightAxisThumb = -handRightAxisThumb;
                    handRightAxis = -handRightAxis;
                }
            }

            if (transform == ivr.leftHandTarget)
                otherHand = ivr.rightHandTarget.GetComponent<IVR_HandMovementsBase>();
            else
                otherHand = ivr.leftHandTarget.GetComponent<IVR_HandMovementsBase>();


            handObj = DetachHand();
#if INSTANTVR_EDGE
            if (physicsMode == PhysicsMode.NoPhysics)
                SetColliderToTrigger(handObj, true);
#endif
            handPhysics = this.GetComponent<IVR_Kinematics>();
            if (handPhysics == null) {
#if INSTANTVR_EDGE
                if (physicsMode == PhysicsMode.FullPhysics) {
                    IVR_KinematicPhysics kp = this.gameObject.AddComponent<IVR_KinematicPhysics>();
                    kp.strength = this.strength;
                    handPhysics = kp;
                } else
#endif
                    handPhysics = this.gameObject.AddComponent<IVR_Kinematics>();
            }

            if (stretchyArms)
                handPhysics.target = handTarget;
            else
                handPhysics.target = stretchlessTarget;
            handPhysics.Kinematize(handObj.GetComponent<Rigidbody>());

            if (forearm != null)
                forearmLength = Vector3.Distance(forearm.position, hand.transform.position);
            else
                forearmLength = 0;
#if INSTANTVR_EDGE
            if (physicsMode == PhysicsMode.FullPhysics) {
                FullHandPhysics handPhysics = handObj.GetComponent<FullHandPhysics>();
                handPhysics.Initialize(stretchlessTarget, forearm, forearmLength);
            }
#endif

            IVR_HandColliderHandler handCH = handObj.AddComponent<IVR_HandColliderHandler>();
            handCH.Initialize(this.transform == ivr.leftHandTarget, this);

            if (interaction != InteractionType.None)
                StartInteraction();


            digits = new Digit[5];
            digits[0] = thumb;
            digits[1] = indexFinger;
            digits[2] = middleFinger;
            digits[3] = ringFinger;
            digits[4] = littleFinger;

            InitFingers();

            if (bodyMovements != null) {
                if (this.transform == ivr.leftHandTarget) {
                    bodyMovements.SetLeftHandTarget(handObj.transform);
                } else {
                    bodyMovements.SetRightHandTarget(handObj.transform);
                }
            }
        }

        private void InitFingers() {
            if (animator != null) {
                Vector3 thumbCurlAxis = ThumbCurlAxis();

                thumb.characterTransform = animator.transform;
                thumb.Init(hand.transform, thumbCurlAxis, hand.transform.position - forearm.position, this.transform == ivr.leftHandTarget, 0);
                for (int i = 1; i < digits.Length; i++)
                    digits[i].Init(hand.transform, handRightAxis, hand.transform.position - forearm.position, this.transform == ivr.leftHandTarget, i);
            }
        }

        private Vector3 ThumbCurlAxis() {
            Transform middleProximal;
            Transform middleDistal;

            if (transform == ivr.leftHandTarget) {
                middleProximal = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                middleDistal = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
            } else {
                middleProximal = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                middleDistal = animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
            }
            Vector3 handOutward = Vector3.left;
            if (middleDistal == null || middleProximal == null) {
                handOutward = (transform == ivr.leftHandTarget) ? Vector3.left : Vector3.right;
            } else {
                handOutward = thumb.transform.InverseTransformDirection(middleDistal.position - middleProximal.position);
            }
            Vector3 handUp = Vector3.Cross(handOutward, handRightAxisThumb);

            if (transform == ivr.leftHandTarget)
                return -handUp;
            else
                return handUp;
        }

        private void StartInteraction() {
            inputModule = ivr.GetComponent<IVR_Interaction>();
            if (inputModule == null) {
                EventSystem eventSystem = FindObjectOfType<EventSystem>();
                if (eventSystem != null)
                    DestroyImmediate(eventSystem.gameObject);
                inputModule = ivr.gameObject.AddComponent<IVR_Interaction>();
            }

#if INSTANTVR_ADVANCED
            bool isLeft = this.transform == ivr.leftHandTarget;
            inputModule.EnableFingerInputModule(this, isLeft, (interaction == InteractionType.Touching), autoActivation);
#endif

            if (interaction == InteractionType.Pointing) {
                if (focusPointObj == null) {
                    focusPointObj = new GameObject("Focus Point");
                    focusPointObj.transform.parent = handObj.transform;
                }
                LineRenderer lineRenderer = focusPointObj.GetComponent<LineRenderer>();
                if (lineRenderer == null) {
                    lineRenderer = focusPointObj.AddComponent<LineRenderer>();
#if (UNITY_5_4 || UNITY_5_3)
                    //lineRenderer.SetWidth(0.01F, 0.01F);
#else
                    lineRenderer.startWidth = 0.01F;
                    lineRenderer.endWidth = 0.01F;
#endif
                }
                if (lineRenderer != null) {
                    lineRenderer.useWorldSpace = false;
                    lineRenderer.SetPosition(1, Vector3.zero);
                }
            }
        }

        private GameObject DetachHand() {
            handObj = new GameObject();
            handObj.transform.position = hand.transform.position;

            if (this.transform == ivr.leftHandTarget) {
                handObj.name = "Left hand";
                handObj.transform.rotation = hand.transform.rotation * bodyMovements.leftArm.fromNormHand;
            } else {
                handObj.name = "Right hand";
                handObj.transform.rotation = hand.transform.rotation * bodyMovements.rightArm.fromNormHand;
            }
            hand.transform.parent = handObj.transform;


            handRigidbody = handObj.GetComponent<Rigidbody>();
            if (handRigidbody == null)
                handRigidbody = handObj.AddComponent<Rigidbody>();

            handRigidbody.mass = 1;
            handRigidbody.drag = 0;
            handRigidbody.angularDrag = 10;
            handRigidbody.useGravity = false;
            handRigidbody.isKinematic = true;
            handRigidbody.interpolation = RigidbodyInterpolation.None;
            handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

#if INSTANTVR_EDGE
            // This should only be used for networking or the hand will not move!
            //NetworkIdentity identity = handObj.AddComponent<NetworkIdentity>();
            //identity.localPlayerAuthority = true;
#endif
            GameObject stretchlessTargetObj = new GameObject("Stretchless Target");
            stretchlessTarget = stretchlessTargetObj.transform;
            stretchlessTarget.parent = this.transform;
            stretchlessTarget.localPosition = Vector3.zero;
            stretchlessTarget.localRotation = Quaternion.identity;

            return handObj;
        }

        private void DeterminePalmPosition() {
            GameObject handPalmObj = new GameObject("Hand Palm");
            handPalm = handPalmObj.transform;
            handPalm.parent = hand.transform;

            // Determine position
            if (indexFinger.transform)
                palmOffset = (indexFinger.transform.position - hand.transform.position) * 0.9f;
            else if (middleFinger.transform)
                palmOffset = (middleFinger.transform.position - hand.transform.position) * 0.9f;
            else
                palmOffset = new Vector3(0.1f, 0, 0);

            handPalm.position = hand.transform.position + palmOffset;

            // Determine rotation
            if (indexFinger.transform)
                handPalm.LookAt(indexFinger.transform, Vector3.up);
            else if (middleFinger.transform)
                handPalm.LookAt(middleFinger.transform, Vector3.up);
            else if (this.transform == ivr.leftHandTarget)
                handPalm.LookAt(handPalm.position - ivr.characterTransform.right, Vector3.up);
            else
                handPalm.LookAt(handPalm.position + ivr.characterTransform.right, Vector3.up);

            // Now get it in the palm

            handPalm.rotation *= Quaternion.Euler(50, 0, 0); // * handPalm.rotation;
            if (transform == ivr.leftHandTarget) {
                handPalm.position += handPalm.rotation * new Vector3(0.02f, -0.02f, 0);
            } else {
                handPalm.position += handPalm.rotation * new Vector3(-0.02f, -0.02f, 0);
            }
        }
        bool collisionsIgnored = false;

        private void IgnoreStaticCollisions(Rigidbody myHand, GameObject obj, bool ignore = true) {
            Collider[] myHandColliders = myHand.GetComponentsInChildren<Collider>();
            Collider[] objColliders = obj.GetComponentsInChildren<Collider>();

            for (int i = 0; i < objColliders.Length; i++) {
                for (int j = 0; j < myHandColliders.Length; j++) {

                    Physics.IgnoreCollision(objColliders[i], myHandColliders[j], ignore);
                }
            }
        }

        private void IgnoreRigidbodyCollisions(Rigidbody myBody, Rigidbody myHand) {
            Collider[] myBodyColliders = myBody.GetComponentsInChildren<Collider>();
            Collider[] myHandColliders = myHand.GetComponentsInChildren<Collider>();

            for (int i = 0; i < myBodyColliders.Length; i++) {
                for (int j = 0; j < myHandColliders.Length; j++) {
                    Physics.IgnoreCollision(myBodyColliders[i], myHandColliders[j]);
                }
            }
        }

        private void IgnoreHandBodyCollisions() {
            Rigidbody hipRigidbody = ivr.GetComponent<Rigidbody>();
            if (hipRigidbody != null) {
                Rigidbody handRigidbody = handObj.GetComponent<Rigidbody>();
                IgnoreRigidbodyCollisions(hipRigidbody, handRigidbody);
            }
        }

        IEnumerator TmpDisableCollisions(Rigidbody handRigidbody, GameObject grabbedObj) {
            SetAllColliders(handObj, false);
            yield return new WaitForSeconds(0.2f);
            SetAllColliders(handObj, true);
        }

        public override void UpdateMovements() {
            if (!collisionsIgnored) {
                IgnoreHandBodyCollisions();
                collisionsIgnored = true;
            }
            HandUpdate();
            CheckLetGo();
#if INSTANTVR_ADVANCED
            CheckHandPose();
#endif
        }

        private void HandUpdate() {
            hand.transform.localPosition = Vector3.zero;
            handPhysics.UpdateKR();

            if (thumb != null && digits != null) {
                thumb.Update(thumbCurl);
                digits[1].Update(indexCurl);
                digits[2].Update(middleCurl);
                digits[3].Update(ringCurl);
                digits[4].Update(littleCurl);
                if (grabbedObject != null) {
                    handPhysics.UpdateJoinedObject(handPalm);
                }
            }
        }

        #region HandMessages
#if INSTANTVR_ADVANCED

        private bool fingersClosed = false;

        private void CheckHandPose() {
            if (fingersClosed) {
                if (indexFinger.input < 0.5f && middleFinger.input < 0.5f && ringFinger.input < 0.5f & littleFinger.input < 0.5f) {
                    fingersClosed = false;
                    ivr.gameObject.SendMessage("OnFingersOpened", this, SendMessageOptions.DontRequireReceiver);
                }
            } else {
                if (indexFinger.input > 0.5f || middleFinger.input > 0.5f || ringFinger.input > 0.5f || littleFinger.input > 0.5f) {
                    fingersClosed = true;
                    ivr.gameObject.SendMessage("OnFingersClosed", this, SendMessageOptions.DontRequireReceiver);
                }
            }

            bool isLeft = this.transform == ivr.leftHandTarget;
            InputDeviceIDs inputDeviceID = isLeft ? InputDeviceIDs.LeftHand : InputDeviceIDs.RightHand;

            if (interaction != InteractionType.None) {
                if (this.transform == ivr.leftHandTarget)
                    inputModule.ProcessPointer(InputDeviceIDs.LeftHand);
                else
                    inputModule.ProcessPointer(InputDeviceIDs.RightHand);
            }

            if (interaction == InteractionType.Pointing) {
                bool isPointing = (indexFinger.input < 0.1f && middleFinger.input > 0.5f && ringFinger.input > 0.5f & littleFinger.input > 0.5f);
                inputModule.EnableFingerPointing(isPointing, isLeft);

                if (focusPointObj == null)
                    return;


                if (!isPointing) {
                    focusPointObj.SetActive(false);
                    pointingAtObject = null;
                } else {
                    focusPoint = inputModule.GetFocusPoint(inputDeviceID);

                    focusPointObj.SetActive(true);
                    focusPointObj.transform.position = focusPoint;

                    IVR_Reticle reticle = focusPointObj.GetComponent<IVR_Reticle>();
                    if (reticle != null) {
                        reticle.gazePhase = Mathf.Clamp01(inputModule.GetGazeDuration(inputDeviceID) / autoActivation);
                    }

                    LineRenderer lineRenderer = focusPointObj.GetComponent<LineRenderer>();
                    if (lineRenderer != null) {
                        lineRenderer.enabled = true;
                        Transform indexFingerTip = indexFinger.transform.GetChild(0);

                        Vector3 fingerTipPosition = focusPointObj.transform.InverseTransformPoint(indexFingerTip.position);
                        lineRenderer.SetPosition(0, fingerTipPosition);
                    }
                    pointingAtObject = inputModule.GetFocusObject(inputDeviceID);
                }
            }
            if (interaction != InteractionType.None) {
                touchingObject = inputModule.GetTouchObject(inputDeviceID);
            }
        }
#endif
        #endregion

        [HideInInspector]
        public bool lettingGo = false;

        public void FixedUpdate() {
            DetermineVelocity(handRigidbody);
            if (grabbedObject != null && !lettingGo) {
                float totalCurl = indexCurl + middleCurl + ringCurl + littleCurl;
                if (totalCurl < 0.15F) {
                    lettingGo = false;
                    NetworkingLetGo();
                }
            }

#if INSTANTVR_EDGE
            FullHandPhysics handPhysics = handObj.GetComponent<FullHandPhysics>();
            if (handPhysics == null) {
                return;
            }

            handPhysics.ManualFixedUpdate(this.handPhysics.target); // transform);
#endif
        }

        [HideInInspector]
        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private Vector3 rigidbodyVelocity;
        private Vector3 rigidbodyAngularVelocity;

        private void DetermineVelocity(Rigidbody rigidbody) {
            if (rigidbody == null)
                return;

            // velocity is normally not calculated for kinematic rigidbodies :-|
            if (rigidbody.isKinematic) {
                rigidbody.velocity = (rigidbody.position - lastPosition) / Time.fixedDeltaTime;
                lastPosition = rigidbody.position;

                rigidbody.angularVelocity = (Quaternion.Inverse(lastRotation) * rigidbody.rotation).eulerAngles / Time.fixedDeltaTime;
                lastRotation = rigidbody.rotation;
            }

            rigidbodyVelocity = rigidbody.velocity;
            rigidbodyAngularVelocity = rigidbody.angularVelocity;
        }

        #region Touching
#if INSTANTVR_ADVANCED
        public void OnTouchStart(GameObject obj) {
            GrabCheck(obj);
            if (inputModule != null && interaction == InteractionType.Touching)
                inputModule.OnFingerTouchStart(this.transform == ivr.leftHandTarget, obj);
        }

        public void OnTouchEnd() {
            if (inputModule != null && interaction == InteractionType.Touching)
                inputModule.OnFingerTouchEnd(this.transform == ivr.leftHandTarget);
        }
#endif
        #endregion

        StoredRigidbody grabbedRBdata;
        Transform originalParent;

        #region Grabbing
        public const float kinematicMass = 1; // masses < kinematicMass will move kinematic when not colliding
        public const float maxGrabbingMass = 10; // max mass you can grab
        [HideInInspector]
        public bool grabbing = false;

        public void GrabCheck(GameObject obj) {
            if (grabbing || grabbedObject != null) {
                return;
            }
            grabbing = true;
            if (indexCurl + middleCurl + ringCurl + littleCurl > 1) {
                if (easyGrab || thumbCurl > 0) {
                    NetworkingGrab(obj);
                } else
                    grabbing = false;
            } else
                grabbing = false;
        }

        public override void NetworkingGrab(GameObject obj) {
            if (grabbedObject != null || obj == ivr.gameObject)
                return;

#if INSTANTVR_EDGE
            if (ivr.transform.parent != null) {
#if !IVR_PHOTON
                IVR_UnetAvatar networkAvatar = ivr.transform.parent.GetComponent<IVR_UnetAvatar>();
                if (networkAvatar != null) {
                    if (networkAvatar.isLocalPlayer) {
                        networkAvatar.CmdGrab(obj, transform == ivr.leftHandTarget);
                    }
#else
                IVR_PhotonAvatar punAvatar = ivr.transform.parent.GetComponent<IVR_PhotonAvatar>();
                if (punAvatar != null) {
                    punAvatar.PunGrab(obj, transform == ivr.leftHandTarget);
#endif
                } else
                    Grab(obj);
            } else
#endif
                Grab(obj);
        }

        public void Grab(GameObject obj, bool rangeCheck = true) {
            Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
            if (!obj.isStatic && objRigidbody != null)
                GrabRigidbody(objRigidbody, rangeCheck);
            else
                GrabStaticObject(obj.transform);
            grabbing = false;
        }

        public void GrabRigidbody(Rigidbody objRigidbody, bool rangeCheck = true) {
            Debug.Log("Grab Rigidbody " + objRigidbody);
            IVR_NoGrab noGrab = objRigidbody.GetComponentInChildren<IVR_NoGrab>();
            if (noGrab != null)
                return;

            if (objRigidbody.mass > maxGrabbingMass)
                return;

            if (objRigidbody == otherHand.handRigidbody || otherHand.grabbedObject == objRigidbody) {
                GrabRigidbodyJoint(objRigidbody);
                return;
            }

            IVR_Handle[] handles = objRigidbody.GetComponentsInChildren<IVR_Handle>();
            for (int i = 0; i < handles.Length; i++) {
                if ((transform == ivr.leftHandTarget && handles[i].hand == IVR_Handle.Hand.Right) ||
                    (transform == ivr.rightHandTarget && handles[i].hand == IVR_Handle.Hand.Left))
                    continue;

                Vector3 handlePosition = handles[i].transform.TransformPoint(handles[i].position);
                float grabDistance = Vector3.Distance(handPalm.position, handlePosition);

                if (grabDistance < handles[i].range || !rangeCheck) {
                    System.Type handleType = handles[i].GetType();

                    if (handleType == typeof(IVR_BarHandle)) {
                        GrabRigidbodyBarHandle((IVR_BarHandle)handles[i]);
                        return;
                    }
                }
            }

            Joint joint = objRigidbody.GetComponent<Joint>();
            if (joint != null || objRigidbody.constraints != RigidbodyConstraints.None) {

                GrabRigidbodyJoint(objRigidbody);
            } else {
                GrabRigidbodyParenting(objRigidbody);
            }
        }

        private void GrabRigidbodyBarHandle(IVR_BarHandle handle) {
            Debug.Log("Grab Rigidbody Bar Handle " + handle);
            Collider c = handle.GetComponentInChildren<Collider>();
            Rigidbody objRigidbody = c.attachedRigidbody;
            Transform objTransform = objRigidbody.transform;

            Joint joint = objRigidbody.GetComponent<Joint>();
            if (joint != null || objRigidbody.constraints != RigidbodyConstraints.None) {
                MoveHandToObject(objTransform, handle);

                // To add: if handle.rotation = true
                Vector3 handleWorldPosition = handle.transform.TransformPoint(handle.position);
                Vector3 handleLocalPosition = handObj.transform.InverseTransformPoint(handleWorldPosition);

                Quaternion handleWorldRotation = handle.transform.rotation * Quaternion.Euler(handle.rotation);
                Vector3 handleRotationAxis = handleWorldRotation * Vector3.up;
                Vector3 handleLocalRotationAxis = handObj.transform.InverseTransformDirection(handleRotationAxis);

                GrabRigidbodyJoint(objRigidbody, handleLocalPosition, handleLocalRotationAxis);
            } else {
                MoveObjectToHand(objTransform, handle);
                GrabRigidbodyParenting(objRigidbody);
            }

            grabLocation = handle.position;
            grabbedObject = objTransform.gameObject;
        }

        private void MoveHandToObject(Transform objTransform, IVR_Handle handle) {
            Quaternion handleWorldRotation = handle.transform.rotation * Quaternion.Euler(handle.rotation);
            Quaternion palm2handRot = Quaternion.Inverse(handPalm.rotation) * handObj.transform.rotation;//target.handBone.parent.rotation;
            handObj.transform.rotation = handleWorldRotation * palm2handRot;

            Vector3 handleWPos = handle.transform.TransformPoint(handle.position);
            Vector3 palm2handPos = handObj.transform.position - handPalm.position;
            handObj.transform.position = handleWPos + palm2handPos;
        }

        private void MoveObjectToHand(Transform objTransform, IVR_Handle handle) {
            objTransform.rotation = handPalm.rotation * Quaternion.Inverse(Quaternion.Euler(handle.rotation));

            Vector3 handleWPos = handle.transform.TransformPoint(handle.position);
            objTransform.Translate(handPalm.position - handleWPos, Space.World);
        }

        private void GrabRigidbodyJoint(Rigidbody objRigidbody) {
            Debug.Log("Grab Rigidbody Joint " + objRigidbody);
            GrabMassRedistribution(handRigidbody, objRigidbody);

            ConfigurableJoint joint = handObj.AddComponent<ConfigurableJoint>();
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.projectionMode = JointProjectionMode.PositionAndRotation;
            joint.projectionDistance = 0.01F;
            joint.projectionAngle = 1;

            Collider c = objRigidbody.transform.GetComponentInChildren<Collider>();
            joint.connectedBody = c.attachedRigidbody;

            Grabbed(objRigidbody.gameObject);
        }

        private void GrabRigidbodyJoint(Rigidbody objRigidbody, Vector3 anchorPoint, Vector3 rotationAxis) {
            Debug.Log("Grab Rigidbody Joint with anchor " + objRigidbody);
            GrabMassRedistribution(handRigidbody, objRigidbody);

            ConfigurableJoint joint = handObj.AddComponent<ConfigurableJoint>();
            Collider c = objRigidbody.transform.GetComponentInChildren<Collider>();
            joint.connectedBody = c.attachedRigidbody;

            joint.anchor = anchorPoint;
            joint.axis = rotationAxis;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked; // Free;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            storedCOM = objRigidbody.centerOfMass;
            objRigidbody.centerOfMass = joint.connectedAnchor;

            Grabbed(objRigidbody.gameObject);
        }

        private void GrabRigidbodyParenting(Rigidbody objRigidbody) {
            Debug.Log("Grab Rigidbody Parenting " + objRigidbody);
            GrabMassRedistribution(handRigidbody, objRigidbody);

            grabbedRBdata = new StoredRigidbody(objRigidbody);
            if (Application.isPlaying)
                Object.Destroy(objRigidbody);
            else
                Object.DestroyImmediate(objRigidbody, true);

            originalParent = objRigidbody.transform.parent;
            objRigidbody.transform.parent = handPalm;

            Grabbed(objRigidbody.gameObject);
        }

        public void GrabStaticObject(Transform objTransform) {
            Debug.Log("Grab Static Object " + objTransform);
            IVR_DoGrab doGrab = objTransform.GetComponentInChildren<IVR_DoGrab>();
            if (doGrab != null) {
                GrabStaticJoint(objTransform);
                return;
            }

            IVR_Handle[] handles = objTransform.GetComponentsInChildren<IVR_Handle>();
            for (int i = 0; i < handles.Length; i++) {
                if ((transform == ivr.leftHandTarget && handles[i].hand == IVR_Handle.Hand.Right) ||
                    (transform == ivr.rightHandTarget && handles[i].hand == IVR_Handle.Hand.Left))
                    continue;

                Vector3 handlePosition = handles[i].transform.TransformPoint(handles[i].position);

                if (Vector3.Distance(handPalm.position, handlePosition) < handles[i].range) {

                    System.Type handleType = handles[i].GetType();
                    if (handleType == typeof(IVR_BarHandle)) {
                        GrabStaticBarHandle(handles[i]);
                    }
                }
            }
        }

        private void GrabStaticBarHandle(IVR_Handle handle) {
            Debug.Log("Grab Static Bar Handle " + handle);
            Transform objTransform = handle.transform;

            MoveHandToObject(objTransform, handle);
            GrabStaticJoint(objTransform);
        }

        private void GrabStaticJoint(Transform objTransform) {
            Debug.Log("Grab Static Joint " + objTransform);
            FixedJoint joint = handObj.AddComponent<FixedJoint>();

            Collider c = objTransform.GetComponentInChildren<Collider>();
            joint.connectedBody = c.attachedRigidbody;

            Grabbed(objTransform.gameObject);
        }

        private void Grabbed(GameObject obj) {
#if INSTANTVR_EDGE
            if (physicsMode == PhysicsMode.FullPhysics && !obj.isStatic) {
                SetColliderToTrigger(obj, true);
                FullHandPhysics fullHandPhysics = handRigidbody.GetComponent<FullHandPhysics>();
                fullHandPhysics.DeterminePhysicsMode(kinematicMass);
            }
#endif

            grabbedObject = obj;
            SendMessage("OnGrabbing", grabbedObject, SendMessageOptions.DontRequireReceiver);
            grabbedObject.SendMessage("OnGrabbed", this, SendMessageOptions.DontRequireReceiver);

        }
        #endregion

        #region Letting Go
        private void CheckLetGo() {
            if (grabbedObject != null) {
                bool fingersGrabbing = (indexCurl + middleCurl + ringCurl + littleCurl >= 2);
                bool pulledLoose = PulledLoose();
                if (!fingersGrabbing || pulledLoose) {
                    NetworkingLetGo();
                }
            }
        }

        private bool PulledLoose() {
            if (forearmLength <= 0)
                return false;

            float distance = Vector3.Distance(handObj.transform.position, forearm.position) - forearmLength; // handTarget.position creates instability
            return (distance > 0.15F);
        }

        public override void NetworkingLetGo() {
#if INSTANTVR_EDGE
            if (ivr.transform.parent != null) {
#if !IVR_PHOTON
                IVR_UnetAvatar networkAvatar = ivr.transform.parent.GetComponent<IVR_UnetAvatar>();
                if (networkAvatar != null) {
                    if (networkAvatar.isLocalPlayer && !lettingGo) {
                        lettingGo = true;
                        networkAvatar.CmdLetGo(grabbedObject, transform == ivr.leftHandTarget);
                    }
#else
                IVR_PhotonAvatar punAvatar = ivr.transform.parent.GetComponent<IVR_PhotonAvatar>();
                if (punAvatar != null) {
                    punAvatar.PunLetGo(grabbedObject, transform == ivr.leftHandTarget);
#endif
                } else
                    LetGo(grabbedObject);
            } else
#endif
                LetGo(grabbedObject);
        }

        public void LetGo(GameObject obj) {
#if INSTANTVR_EDGE
            FullHandPhysics fullHandPhysics = handRigidbody.GetComponent<FullHandPhysics>();
            if (fullHandPhysics != null)
                FullHandPhysics.SetKinematic(handRigidbody, true);
#endif
            Joint joint = handObj.GetComponent<Joint>();
            if (joint != null) {
                Object.DestroyImmediate(joint);

            } else {
                if (grabbedObject.transform.parent == handObj.transform || grabbedObject.transform.parent == handPalm) {
                    grabbedObject.transform.parent = originalParent;
                }
            }

            if (grabbedObject != null) {
                LetGoGrabbedObject();
            }

        }

        private void LetGoGrabbedObject() {
            Debug.Log("Let Go Grabbed Object");
            SetAllColliders(grabbedObject, true);
#if INSTANTVR_EDGE
            if (handPhysics != null)
                SetColliderToTrigger(grabbedObject, false);
#endif
            Rigidbody grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();
            if (!grabbedObject.isStatic && grabbedRigidbody == null) {
                grabbedRigidbody = grabbedObject.AddComponent<Rigidbody>();
                if (grabbedRBdata != null) {
                    grabbedRBdata.CopyToRigidbody(grabbedRigidbody);
                    grabbedRBdata = null;
                }
            }

            if (grabbedRigidbody != null) {
                GrabMassRestoration(handRigidbody, grabbedRigidbody);

                Joint[] joints = grabbedObject.GetComponents<Joint>();
                for (int i = 0; i < joints.Length; i++) {
                    if (joints[i].connectedBody == handRigidbody)
                        Object.Destroy(joints[i]);
                }
                grabbedRigidbody.centerOfMass = storedCOM;

                grabbedRigidbody.velocity = rigidbodyVelocity;
                grabbedRigidbody.angularVelocity = rigidbodyAngularVelocity;
            }

            StartCoroutine(TmpDisableCollisions(handRigidbody, grabbedObject));

#if INSTANTVR_EDGE
            FullHandPhysics fullHandPhysics = handRigidbody.GetComponent<FullHandPhysics>();
            if (fullHandPhysics != null)
                fullHandPhysics.DeterminePhysicsMode(kinematicMass);
#endif
            SendMessage("OnLettingGo", grabbedObject, SendMessageOptions.DontRequireReceiver);
            grabbedObject.SendMessage("OnLetGo", this, SendMessageOptions.DontRequireReceiver);

            GameObject obj = grabbedObject;
            grabbedObject = null;
            lettingGo = false;

            if (otherHand.grabbedObject == obj || otherHand.grabbedObject == handObj) {
                otherHand.NetworkingLetGo();
                otherHand.NetworkingGrab(obj);
            }
        }
        #endregion

        private void GrabMassRedistribution(Rigidbody handRigidbody, Rigidbody grabbedRigidbody) {
            //handRigidbody.mass = grabbedRigidbody.mass;
            //grabbedRigidbody.mass *= 0.01f;
        }

        private void GrabMassRestoration(Rigidbody handRigidbody, Rigidbody grabbedRigidbody) {
            //grabbedRigidbody.mass += handRigidbody.mass;
            //handRigidbody.mass = 1f;
        }

        void LateUpdate() {
            if (bodyPull)
                BodyPull();
        }

        private void SetAllColliders(GameObject obj, bool enabled) {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
                c.enabled = enabled;
        }

        private static void SetColliderToTrigger(GameObject obj, bool b) {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            for (int j = 0; j < colliders.Length; j++)
                colliders[j].isTrigger = b;
        }

        private void BodyPull() {
            if (grabbedObject != null && grabbedObject.isStatic) {
                Vector3 handPull = hand.transform.position - handTarget.position;
                Debug.DrawLine(hand.transform.position, handTarget.position, Color.yellow);

                ivr.Move(handPull, verticalBodyPull);
            }
        }
    }

    public class IVR_HandColliderHandler : MonoBehaviour {

        //private Transform thumb;
        private Transform[] digits;

        private IVR_HandMovements handMovements;

        public void Initialize(bool isLeftHand, IVR_HandMovements handMovements) {
            this.handMovements = handMovements;

            if (handMovements.animator == null)
                return;

            digits = new Transform[5];
            if (isLeftHand) {
                //thumb = handTarget.animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
                digits[1] = handMovements.animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
                digits[2] = handMovements.animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                digits[3] = handMovements.animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
                digits[4] = handMovements.animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
            } else {
                //thumb = handTarget.animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
                digits[1] = handMovements.animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
                digits[2] = handMovements.animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
                digits[3] = handMovements.animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
                digits[4] = handMovements.animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
            }

        }
        /* We just do simple grabbing at the momennt
            void OnCollisionStay (Collision otherCollider) {
                Transform thisTransform, otherTransform;

                if (handTarget.grabbedObject == null) {
                    bool fingersCollided = false;
                    bool thumbCollided = false;

                    int ncontacts = otherCollider.contacts.Length;
                    for (int i = 0; i < ncontacts; i++ ) {
                        thisTransform = otherCollider.contacts[i].thisCollider.transform;
                        otherTransform = otherCollider.contacts[i].otherCollider.transform;
                        if (thisTransform == thumb || otherTransform == thumb)
                            thumbCollided = true;
                        for (int j = 1; j < digits.Length; j++) {
                            Transform finger = digits[j];
                            if (thisTransform == finger || otherTransform == finger) {
                                fingersCollided = true;
                            }
                        }
                    }

                    bool grabbed = false;
                    // We are touching it both sides
                    if (fingersCollided || thumbCollided) { // || = easy grab, && = realistic grab
                        if (handTarget.indexInput + handTarget.middleInput + handTarget.ringInput + handTarget.littleInput > 0)
                        if (!grabbed) {
                            handTarget.Grab(otherCollider.gameObject);
                            grabbed = true;
                        }
                    }
                }
            }
        */

        void OnKinematicCollisionEnter(GameObject gameObject) {
            GrabOnCollision(gameObject);
        }

        void OnCollisionEnter(Collision collision) {
            GrabOnCollision(collision.gameObject);
        }

        void OnCollisionStay(Collision collision) {
            GrabOnCollision(collision.gameObject);
        }

        public void GrabOnCollision(GameObject grabbedGameObject) {
            handMovements.GrabCheck(grabbedGameObject);
        }
    }
}