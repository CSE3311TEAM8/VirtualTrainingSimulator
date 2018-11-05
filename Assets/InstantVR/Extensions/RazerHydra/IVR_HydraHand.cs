/* InstantVR Hydra hand controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - included IVR_HYDRA define check
 */

using UnityEngine;

namespace IVR {

    public class IVR_HydraHand : IVR_HandController {
#if (UNITY_STANDALONE_WIN && IVR_HYDRA)

        [HideInInspector] private InstantVR.BodySide bodySide;
        public Vector3 Sensitivity = new Vector3(0.0008f, 0.0008f, 0.0008f);

        [HideInInspector] private IVR_Hydra ivrHydra;

        [HideInInspector]
        private IVR_Hydra.HydraController hydraController;
        [HideInInspector]
        private IVR_HydraHand otherHydraController;

        [HideInInspector]
        private IVR_LeapHand leapHand;

        [HideInInspector]
        private ControllerInput controllerInput;

        [HideInInspector] private Vector3 calibrationPosition, localCalibrationPosition;
        [HideInInspector] private Quaternion calibrationRotation;

        private bool started = false;

        void Start() { }

        public override void StartController(InstantVR ivr) {
            ivrHydra = ivr.GetComponent<IVR_Hydra>();
            extension = ivrHydra;

            controllerInput = Controllers.GetController(0);

            if (transform == ivr.leftHandTarget) {
                bodySide = InstantVR.BodySide.Left;
                otherHydraController = ivr.rightHandTarget.GetComponent<IVR_HydraHand>();

                localCalibrationPosition = new Vector3(-0.1f, 1.1f, 0.15f);
                calibrationPosition = (ivr.transform.position - ivr.transform.position) + localCalibrationPosition;
                calibrationRotation = Quaternion.Euler(30, 0, 90);
            } else {
                bodySide = InstantVR.BodySide.Right;
                otherHydraController = ivr.leftHandTarget.GetComponent<IVR_HydraHand>();

                localCalibrationPosition = new Vector3(0.1f, 1.1f, 0.15f);
                calibrationPosition = (ivr.transform.position - ivr.transform.position) + localCalibrationPosition;
                calibrationRotation = Quaternion.Euler(30, 0, 270);
            }

            base.StartController(ivr);

            startPosition = calibrationPosition;
            startRotation = calibrationRotation;

            extrapolation = false;
            started = true;

            leapHand = GetComponent<IVR_LeapHand>();

            SetBodyRotation();
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
            if (ivrHydra != null) {
                hydraController = ivrHydra.GetController(bodySide);
                UpdateHydra();
                UpdateInput();
            }
        }

        private void UpdateHydra() {
            if (hydraController != null) {

                if (started || (!started && hydraController.m_Trigger > 0.01f)) {
                    controllerPosition = Vector3.Scale(hydraController.Position, Sensitivity);
                    if (transform == ivr.leftHandTarget) {
                        controllerPosition += new Vector3(-0.04f, 0, 0);
                        controllerRotation = hydraController.Rotation * Quaternion.Euler(0, 0, 90);
                    } else {
                        controllerPosition += new Vector3(0.04f, 0, 0);
                        controllerRotation = hydraController.Rotation * Quaternion.Euler(0, 0, -90);
                    }
                }

                if (!started && hydraController.m_Trigger > 0.01f) { // start tracking
                    started = true;

                    extension.trackerPosition = calibrationPosition - this.controllerPosition;
                    extension.trackerRotation = Quaternion.identity;

                    Calibrate(false);

                    if (otherHydraController.started) {
                        Vector3 otherTrackerPosition = extension.trackerPosition;
                        Vector3 thisTrackerPosition = calibrationPosition - controllerPosition;
                        extension.trackerPosition = (thisTrackerPosition + otherTrackerPosition) / 2;
                    }
                }

                if (hydraController.m_Docked) {
                    tracking = false;
                } else {
                    tracking = true;
                    if (!started) {
                        controllerPosition = calibrationPosition - extension.trackerPosition;
                        controllerRotation = calibrationRotation * Quaternion.Inverse(extension.trackerRotation);

                    }
                    if (!selected && leapHand != null && leapHand.isSelected())
                        CalibrateWithSelected();
                }

                if (tracking && selected) {
                    base.UpdateController();
                }
            }
        }

        private void CalibrateWithSelected() {
            extension.trackerPosition = transform.position - ivr.transform.position - controllerPosition;
        }

        public override void OnTargetReset() {
            Calibrate(false);
            tracking = false;
            started = false;
        }

        private void UpdateInput() {
            if (controllerInput != null) {
                if (transform == ivr.leftHandTarget)
                    UpdateInputSide(controllerInput.left);
                else
                    UpdateInputSide(controllerInput.right);
            }
        }

        private void UpdateInputSide(ControllerInputSide controllerInput) {
            if (hydraController != null) {
                controllerInput.stickHorizontal = Mathf.Clamp(controllerInput.stickHorizontal + hydraController.m_JoystickX, -1, 1);
                controllerInput.stickVertical = Mathf.Clamp(controllerInput.stickVertical + hydraController.m_JoystickY, -1, 1);
                controllerInput.stickButton |= hydraController.GetButton(IVR_Hydra.HydraButtons.JOYSTICK);

                controllerInput.up |= (controllerInput.stickVertical > 0.3F);
                controllerInput.down |= (controllerInput.stickVertical < -0.3F);
                controllerInput.left |= (controllerInput.stickHorizontal < -0.3F);
                controllerInput.right |= (controllerInput.stickHorizontal > 0.3F);

                controllerInput.trigger1 = Mathf.Max(controllerInput.trigger1, hydraController.GetButton(IVR_Hydra.HydraButtons.BUMPER) ? 1 : 0);
                controllerInput.trigger2 = Mathf.Max(controllerInput.trigger2, hydraController.m_Trigger);
                controllerInput.option |= hydraController.GetButton(IVR_Hydra.HydraButtons.START);

                controllerInput.buttons[0] |= hydraController.GetButton(IVR_Hydra.HydraButtons.ONE);
                controllerInput.buttons[1] |= hydraController.GetButton(IVR_Hydra.HydraButtons.TWO);
                controllerInput.buttons[2] |= hydraController.GetButton(IVR_Hydra.HydraButtons.THREE);
                controllerInput.buttons[3] |= hydraController.GetButton(IVR_Hydra.HydraButtons.FOUR);
            }
        }
#endif
    }
}