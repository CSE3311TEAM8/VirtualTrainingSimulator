/* InstantVR SteamVR hand controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.3
 * date: June 25, 2017
 * 
 * - Fixed controllers not tracker when avatar is started second time
 */

using UnityEngine;

namespace IVR {

    public class IVR_SteamVRControllerHand : IVR_HandController {
#if IVR_STEAMVR

        [HideInInspector]
        private IVR_SteamVRController steamVRextension;
        [HideInInspector]
        private GameObject controller;
        [HideInInspector]
        public SteamVR_TrackedObject steamTracker;
        [HideInInspector]
        private bool trackerIdDetermined = false;


        private Vector3 palm2Wrist;

        public override void StartController(InstantVR ivr) {
            steamVRextension = ivr.GetComponent<IVR_SteamVRController>();
            extension = steamVRextension;
            base.StartController(ivr);

            IVR_UnityVRHead unityVRhead = ivr.headTarget.GetComponent<IVR_UnityVRHead>();

            if (unityVRhead != null) {
                steamTracker = CreateSteamController(ivr, transform);

                if (transform == ivr.leftHandTarget) {
                    if (steamVRextension.steamManager != null)
                        steamVRextension.steamManager.left = steamTracker.gameObject;
                    palm2Wrist = new Vector3(-0.03F, 0.06F, -0.15F);
                } else {
                    if (steamVRextension.steamManager != null)
                        steamVRextension.steamManager.right = steamTracker.gameObject;
                    palm2Wrist = new Vector3(0.03F, 0.06F, -0.15F);
                }

                SetBodyRotation();
            } else
                enabled = false;

            controllerInput = Controllers.GetController(0);
        }

        public static SteamVR_TrackedObject CreateSteamController(InstantVR ivr, Transform transform) {
            IVR_UnityVRHead unityVRhead = ivr.headTarget.GetComponent<IVR_UnityVRHead>();

            GameObject controllerObj = new GameObject();
            if (transform == ivr.leftHandTarget) {
                controllerObj.name = "Steam Controller Left";
            } else {
                controllerObj.name = "Steam Controller Right";
            }

            controllerObj.transform.parent = unityVRhead.cameraRoot.transform;
            controllerObj.transform.position = transform.position;
            controllerObj.transform.rotation = transform.rotation;

            SteamVR_TrackedObject trackedObject = controllerObj.AddComponent<SteamVR_TrackedObject>();

            return trackedObject;
        }

        private void SwitchSteamTracker(SteamVR_TrackedObject.EIndex index, bool isLeft) {
            IVR_UnityVRHead unityVRhead = ivr.headTarget.GetComponent<IVR_UnityVRHead>();

            Transform otherSteamTrackerTransform;
            if (isLeft) {
                otherSteamTrackerTransform = unityVRhead.cameraRoot.transform.Find("Steam Controller Right");
                if (steamVRextension.steamManager != null)
                    steamVRextension.steamManager.right = otherSteamTrackerTransform.gameObject;
            } else {
                otherSteamTrackerTransform = unityVRhead.cameraRoot.transform.Find("Steam Controller Left");
                if (steamVRextension.steamManager != null)
                    steamVRextension.steamManager.left = otherSteamTrackerTransform.gameObject;
            }
            SteamVR_TrackedObject otherSteamTracker = otherSteamTrackerTransform.GetComponent<SteamVR_TrackedObject>();
            otherSteamTracker.index = index;
        }

        private void FindController() {

            SteamVR_TrackedObject.EIndex controllerIndex = (SteamVR_TrackedObject.EIndex)SteamVR_Controller.GetDeviceIndex((transform == ivr.leftHandTarget) ? SteamVR_Controller.DeviceRelation.Leftmost : SteamVR_Controller.DeviceRelation.Rightmost);
            if (controllerIndex == SteamVR_TrackedObject.EIndex.None)
                return;

            IVR_UnityVRHead unityVRhead = ivr.headTarget.GetComponent<IVR_UnityVRHead>();
            Transform otherSteamTrackerTransform;
            if (transform == ivr.leftHandTarget) {
                otherSteamTrackerTransform = unityVRhead.cameraRoot.transform.Find("Steam Controller Right");
            } else {
                otherSteamTrackerTransform = unityVRhead.cameraRoot.transform.Find("Steam Controller Left");
            }
            SteamVR_TrackedObject otherSteamTracker = otherSteamTrackerTransform.GetComponent<SteamVR_TrackedObject>();
            if (otherSteamTracker.index == controllerIndex) {
                // We already assigned this controller to the other hand
                return;
            }

            if (steamTracker.index != 0)
                SwitchSteamTracker(steamTracker.index, (transform == ivr.leftHandTarget));
            
            steamTracker.index = controllerIndex;
            if (steamVRextension.steamManager != null) {
                if (transform == ivr.leftHandTarget)
                    steamVRextension.steamManager.left = steamTracker.gameObject;
                else
                    steamVRextension.steamManager.right = steamTracker.gameObject;
            }

            trackerIdDetermined = true;
        }

        private void SetBodyRotation() {
            IVR_AnimatorHip hipAnimator = ivr.hipTarget.GetComponent<IVR_AnimatorHip>();
            if (hipAnimator != null) {
                if (hipAnimator.rotationMethod == IVR_AnimatorHip.Rotations.Auto) {
                    hipAnimator.rotationMethod = IVR_AnimatorHip.Rotations.HandRotation;
                }
            }
        }

        #region Update
        public override void UpdateController() {
            if (enabled) {
                if (!trackerIdDetermined) {
                    FindController();
                }

                UpdateTransform();
                UpdateInput();
            }
        }

        private void UpdateTransform() {

            if (steamTracker.isValid) {
                if (!tracking) {
                    Valve.VR.ETrackedDeviceClass deviceClass = SteamVR.instance.hmd.GetTrackedDeviceClass((uint) steamTracker.index);
                    bool isController = (deviceClass == Valve.VR.ETrackedDeviceClass.Controller);
                    if (!isController)
                        return;
                }

                tracking = true;
                Vector3 controllerPosition = steamTracker.transform.position + ivr.transform.rotation * new Vector3(0, 0.1333F, 0.1333F); //strange correction needed to get controller at right position
                Quaternion controllerRotation = steamTracker.transform.rotation;

                Quaternion localHandRotation = Quaternion.Inverse(ivr.transform.rotation) * steamTracker.transform.rotation;

                if (transform == ivr.leftHandTarget)
                    transform.rotation = ivr.transform.rotation * localHandRotation * Quaternion.Euler(45, 0, 90);
                else
                    transform.rotation = ivr.transform.rotation * localHandRotation * Quaternion.Euler(45, 0, -90);

                transform.position = controllerPosition  + controllerRotation * palm2Wrist;
            }
        }

        private ControllerInput controllerInput;

        private void UpdateInput() {
            if (transform == ivr.leftHandTarget)
                SetControllerInput(controllerInput.left);
            else
                SetControllerInput(controllerInput.right);
        }

        private void SetControllerInput(ControllerInputSide controllerInputSide) {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int) steamTracker.index);

            controllerInputSide.stickHorizontal += device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            controllerInputSide.stickVertical += device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;

            controllerInputSide.trigger2 += device.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip) ? 1 : 0;
            controllerInputSide.trigger1 += device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

            controllerInputSide.stickButton |= device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
            controllerInputSide.stickTouch |= device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
            controllerInputSide.option |= device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu);
            controllerInputSide.buttons[0] = device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu);

            if (controllerInputSide.stickButton) {
                controllerInputSide.stickHorizontal += device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                controllerInputSide.stickVertical += device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
            }

            controllerInputSide.up |= (controllerInputSide.stickButton && controllerInputSide.stickVertical > 0.3F);
            controllerInputSide.down |= (controllerInputSide.stickButton && controllerInputSide.stickVertical < -0.3F);
            controllerInputSide.left |= (controllerInputSide.stickButton && controllerInputSide.stickHorizontal < -0.3F);
            controllerInputSide.right |= (controllerInputSide.stickButton && controllerInputSide.stickHorizontal > 0.3F);
        }
        #endregion
#endif
    }
}
