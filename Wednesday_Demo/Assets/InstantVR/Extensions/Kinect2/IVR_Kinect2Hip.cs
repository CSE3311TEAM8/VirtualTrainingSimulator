/* InstantVR Kinect 2 hip controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - Included IVR_KINECT define check
 */


using UnityEngine;

namespace IVR {

    public class IVR_Kinect2Hip : IVR_Controller {
#if (UNITY_STANDALONE_WIN && IVR_KINECT)

        private IVR_Kinect2 ivrKinect;

        void Start() { }

        public void StartTarget(IVR_Kinect2 ivrKinect, bool present) {
            this.ivrKinect = ivrKinect;

            extrapolation = true;
        }

        public override void UpdateController() {
            if (!ivrKinect.present || !enabled)
                return;

            if (!ivrKinect.GetNewFrame())
                return;

            if (ivrKinect.BoneTracking(IVR_Kinect2.BoneType.HipCenter)) {
                Vector3 hipsPos = ivrKinect.GetBonePos(IVR_Kinect2.BoneType.HipCenter);
                Vector3 hipLeftPos = new Vector3(
                    ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipLeft].Position.X,
                    ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipCenter].Position.Y,
                    -ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipLeft].Position.Z
                    );
                Vector3 hipRightPos = new Vector3(
                    ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipRight].Position.X,
                    ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipCenter].Position.Y,
                    -ivrKinect.bodyData.Joints[(int)IVR_Kinect2.BoneType.HipRight].Position.Z
                    );

                this.controllerPosition = hipsPos;
                Vector3 direction = hipRightPos - hipLeftPos;
                if (direction.magnitude > 0)
                    this.controllerRotation = Quaternion.FromToRotation(Vector3.right, direction);

                if (!tracking && ivrKinect.Tracking)
                    tracking = true;

                base.UpdateController();
            } else {
                tracking = false;
            }
        }
#endif
    }
}