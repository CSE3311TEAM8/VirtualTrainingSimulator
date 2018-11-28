/* InstantVR Oculus Touch hand controller
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 4, 2017
 * 
 * - Moved OVRManager creating from IVR_UnityVRHead to here
 * - Configuration support
 */

using UnityEngine;

namespace IVR {

    public class IVR_TouchHand : IVR_HandController {
#if IVR_OCULUS
        [HideInInspector]
        private GameObject controller;

        [HideInInspector]
        private OVRInput.Controller touchControllerID;

        private bool positionalTracking;

        private Vector3 palm2Wrist;

        public override void StartController(InstantVR ivr) {
            extension = ivr.GetComponent<IVR_Touch>();
            base.StartController(ivr);

            if (transform == ivr.leftHandTarget) {
                touchControllerID = OVRInput.Controller.LTouch;
                palm2Wrist = new Vector3(-0.04F, 0.02F, -0.13F);
            } else {
                touchControllerID = OVRInput.Controller.RTouch;
                palm2Wrist = new Vector3(0.04F, 0.02F, -0.13F);
            }

            CreateController();
            SetBodyRotation();

            controllerInput = Controllers.GetController(0);
        }

        GameObject controllerObject;
        private void CreateController() {
            IVR_UnityVRHead ivrUnityHead = ivr.headTarget.GetComponent<IVR_UnityVRHead>();
            if (ivrUnityHead == null || ivrUnityHead.cameraRoot == null)
                return;

            OVRManager ovrManager = ivrUnityHead.GetComponent<OVRManager>();
            if (ovrManager == null)
                ivrUnityHead.gameObject.AddComponent<OVRManager>();

            controllerObject = new GameObject();
            controllerObject.transform.parent = ivrUnityHead.cameraRoot.transform;
        }

        private void SetBodyRotation() {
            IVR_AnimatorHip hipAnimator = ivr.hipTarget.GetComponent<IVR_AnimatorHip>();
            if (hipAnimator != null) {
                if (hipAnimator.rotationMethod == IVR_AnimatorHip.Rotations.Auto) {
                    hipAnimator.rotationMethod = IVR_AnimatorHip.Rotations.HandRotation;
                }
            }
        }


        public override void UpdateController() {
            if (!enabled)
                return;

            if ((OVRInput.GetConnectedControllers() & touchControllerID) != touchControllerID)
                return;

            UpdateTransform();
            UpdateInput();
        }

        private void UpdateTransform() {
            controllerObject.transform.localPosition = OVRInput.GetLocalControllerPosition(touchControllerID);
            controllerObject.transform.localRotation = OVRInput.GetLocalControllerRotation(touchControllerID);

            controllerPosition = controllerObject.transform.position;
            controllerRotation = controllerObject.transform.rotation;

            if (OVRInput.GetControllerPositionTracked(touchControllerID)) {
                positionalTracking = true;
            } else {
                positionalTracking = false;
            }

            if (selected) {
                transform.rotation = CalculateHandRotation(controllerObject.transform.rotation);
                if (positionalTracking)
                    transform.position = CalculateHandPosition(controllerObject.transform.position, controllerObject.transform.rotation);
            }

            tracking = true;
        }

        private Vector3 CalculateHandPosition(Vector3 controllerPosition, Quaternion controllerRotation) {
            return controllerPosition + transform.rotation * palm2Wrist;
        }

        private Quaternion CalculateHandRotation(Quaternion controllerRotation) {
            if (transform == ivr.leftHandTarget) {
                return controllerRotation * Quaternion.Euler(0, 0, 90);
            } else {
                return controllerRotation * Quaternion.Euler(0, 0, -90);
            }
        }

        private ControllerInput controllerInput;

        private void UpdateInput() {
            if (transform == ivr.leftHandTarget)
                SetControllerInput(controllerInput.left);
            else
                SetControllerInput(controllerInput.right);
        }

        public Valve.VR.VRControllerState_t controllerState;

        private void SetControllerInput(ControllerInputSide controllerInputSide) {
            controllerInputSide.stickHorizontal = Mathf.Clamp(controllerInputSide.stickHorizontal + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, touchControllerID).x, -1, 1);
            controllerInputSide.stickVertical = Mathf.Clamp(-controllerInputSide.stickVertical + OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, touchControllerID).y, -1, 1);
            controllerInputSide.stickButton |= OVRInput.Get(OVRInput.Button.PrimaryThumbstick, touchControllerID);
            controllerInputSide.stickTouch = OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, touchControllerID);

            controllerInputSide.up |= (controllerInputSide.stickVertical > 0.3F);
            controllerInputSide.down |= (controllerInputSide.stickVertical < -0.3F);
            controllerInputSide.left |= (controllerInputSide.stickHorizontal < -0.3F);
            controllerInputSide.right |= (controllerInputSide.stickHorizontal > 0.3F);

            controllerInputSide.trigger1 = Mathf.Max(controllerInputSide.trigger1, OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, touchControllerID) * 0.8F + 0.2F);
            controllerInputSide.trigger1 = OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, touchControllerID) ? controllerInputSide.trigger1 : 0;
            controllerInputSide.trigger2 = Mathf.Max(controllerInputSide.trigger2, OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, touchControllerID));

            controllerInputSide.buttons[0] |= OVRInput.Get(OVRInput.Button.One, touchControllerID);
            controllerInputSide.buttons[1] |= OVRInput.Get(OVRInput.Button.Two, touchControllerID);
        }
#endif
    }
}
