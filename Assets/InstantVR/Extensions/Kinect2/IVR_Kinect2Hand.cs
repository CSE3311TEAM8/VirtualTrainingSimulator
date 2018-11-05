/* InstantVR Kinect 2 hand controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - Included IVR_KINECT define check
 */

using UnityEngine;

namespace IVR {

    public class IVR_Kinect2Hand : IVR_HandController {
#if (UNITY_STANDALONE_WIN && IVR_KINECT)

        public bool handTracking = true;

        [HideInInspector]
        private IVR_Kinect2 ivrKinect;
        [HideInInspector]
        private IVR_Kinect2.BoneType wristJoint, handJoint;
        [HideInInspector]
        private IVR_Kinect2.BoneType elbowJoint;

        private IVR_HandMovements handMovements;

        void Start() { }

        public void StartTarget(IVR_Kinect2 ivrKinect, bool present, InstantVR ivr) {
            this.ivrKinect = ivrKinect;

            extrapolation = false;

            if (this.transform == ivr.leftHandTarget) {
                wristJoint = IVR_Kinect2.BoneType.WristLeft;
                handJoint = IVR_Kinect2.BoneType.HandLeft;
                elbowJoint = IVR_Kinect2.BoneType.ElbowLeft;
            } else {
                wristJoint = IVR_Kinect2.BoneType.WristRight;
                handJoint = IVR_Kinect2.BoneType.HandRight;
                elbowJoint = IVR_Kinect2.BoneType.ElbowRight;
            }
        }

        public override void StartController(InstantVR ivr) {
            base.StartController(ivr);

            if (this.transform == ivr.leftHandTarget)
                startRotation = Quaternion.AngleAxis(90, Vector3.forward);
            else
                startRotation = Quaternion.AngleAxis(270, Vector3.forward);

            handMovements = gameObject.GetComponent<IVR_HandMovements>();
        }

        Vector3 lastWristPos;
        Vector3 lastHandPos;
        Vector3 lastElbowPos;

        public override void UpdateController() {
            if (!ivrKinect.present || !enabled)
                return;

            Vector3 wristPos, handPos;
            Vector3 elbowPos;

            if (ivrKinect.GetNewFrame()) {
                wristPos = ivrKinect.GetBonePos(wristJoint);
                handPos = ivrKinect.GetBonePos(handJoint);
                elbowPos = ivrKinect.GetBonePos(elbowJoint);

                lastWristPos = wristPos;
                lastHandPos = handPos;
                lastElbowPos = elbowPos;
            } else {
                wristPos = lastWristPos;
                handPos = lastHandPos;
                elbowPos = lastElbowPos;
            }

            Vector3 handDirection = handPos - wristPos;
            Vector3 forearmDirection = wristPos - elbowPos;
            if (handDirection.magnitude > 0) {

                if (this.transform == ivr.leftHandTarget) {
                    if (Vector3.Angle(handDirection, forearmDirection) < 45)
                        controllerRotation = Quaternion.LookRotation(handDirection, Vector3.left);
                    else
                        controllerRotation = Quaternion.LookRotation(forearmDirection, Vector3.left);
                } else {
                    if (Vector3.Angle(handDirection, forearmDirection) < 45)
                        controllerRotation = Quaternion.LookRotation(handDirection, Vector3.right);
                    else
                        controllerRotation = Quaternion.LookRotation(forearmDirection, Vector3.right);
                }

            } else
                controllerRotation = Quaternion.identity;

            controllerPosition = wristPos;

            if (!tracking && ivrKinect.Tracking)
                tracking = true;

            base.UpdateController();
        
            if (selected) {
                if (handTracking && handMovements != null) {
                    bool handLasso, handClosed;
                    if (this.transform == ivr.leftHandTarget) {
                        handLasso = ivrKinect.bodyData.HandLeftState == Windows.Kinect.HandState.Lasso;
                        handClosed = ivrKinect.bodyData.HandLeftState == Windows.Kinect.HandState.Closed;
                    } else {
                        handLasso = ivrKinect.bodyData.HandRightState == Windows.Kinect.HandState.Lasso;
                        handClosed = ivrKinect.bodyData.HandRightState == Windows.Kinect.HandState.Closed;
                    }

                    handMovements.thumbCurl = (handLasso || handClosed) ? 1 : 0;
                    handMovements.indexCurl = handClosed ? 1 : 0;
                    handMovements.middleCurl = handClosed ? 1 : 0;
                    handMovements.ringCurl = (handLasso || handClosed) ? 1 : 0;
                    handMovements.littleCurl = (handLasso || handClosed) ? 1 : 0;
                }
            }

        }
#endif
    }

}