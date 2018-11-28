/* InstantVR Gear VR controller
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.3
 * date: June 19, 2017
 * 
 */

using UnityEngine;

namespace IVR {

    public class IVR_GearVRController : MonoBehaviour {
#if IVR_OCULUS
        public InstantVR ivr;
        public bool isLeft;

        [HideInInspector]
        private GameObject controller;

        [HideInInspector]
        private OVRInput.Controller touchControllerID;

        public bool tracking;

        public void Start() {

            if (isLeft) {
                touchControllerID = OVRInput.Controller.LTrackedRemote;
            } else {
                touchControllerID = OVRInput.Controller.RTrackedRemote;
            }

            CreateController();

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


        public void Update() {
            if ((OVRInput.GetConnectedControllers() & touchControllerID) != touchControllerID)
                return;

            UpdateTransform();
            UpdateInput();
        }

        private void UpdateTransform() {
            controllerObject.transform.localPosition = OVRInput.GetLocalControllerPosition(touchControllerID);
            controllerObject.transform.localRotation = OVRInput.GetLocalControllerRotation(touchControllerID);

            transform.rotation = controllerObject.transform.rotation;
            if (OVRInput.GetControllerPositionTracked(touchControllerID))
                transform.position = controllerObject.transform.position;

            tracking = true;
        }

        private ControllerInput controllerInput;

        private void UpdateInput() {
            if (transform == ivr.leftHandTarget)
                SetControllerInput(controllerInput.left);
            else
                SetControllerInput(controllerInput.right);
        }

        private void SetControllerInput(ControllerInputSide controllerInputSide) {
            controllerInputSide.stickHorizontal = Mathf.Clamp(controllerInputSide.stickHorizontal + OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, touchControllerID).x, -1, 1);
            controllerInputSide.stickVertical = Mathf.Clamp(-controllerInputSide.stickVertical + OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, touchControllerID).y, -1, 1);
            controllerInputSide.stickButton |= OVRInput.Get(OVRInput.Button.PrimaryTouchpad, touchControllerID);
            controllerInputSide.stickTouch = OVRInput.Get(OVRInput.Touch.PrimaryTouchpad, touchControllerID);

            controllerInputSide.up |= (controllerInputSide.stickVertical > 0.3F);
            controllerInputSide.down |= (controllerInputSide.stickVertical < -0.3F);
            controllerInputSide.left |= (controllerInputSide.stickHorizontal < -0.3F);
            controllerInputSide.right |= (controllerInputSide.stickHorizontal > 0.3F);

            controllerInputSide.trigger1 = Mathf.Clamp(controllerInputSide.trigger1 + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, touchControllerID), -1, 1);

            controllerInputSide.buttons[0] |= OVRInput.Get(OVRInput.Button.One, touchControllerID);
            controllerInputSide.buttons[1] |= OVRInput.Get(OVRInput.Button.Back, touchControllerID);
        }
#endif
    }
}
