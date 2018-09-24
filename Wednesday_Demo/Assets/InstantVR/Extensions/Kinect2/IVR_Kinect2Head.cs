/* InstantVR Kinect 2 head controller
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - Included IVR_KINECT define check
 */

using UnityEngine;

namespace IVR {

    public class IVR_Kinect2Head : IVR_Controller {
#if (UNITY_STANDALONE_WIN && IVR_KINECT)
        public bool headRotation = true;

        [HideInInspector]
        private IVR_Kinect2 ivrKinect;
        [HideInInspector]
        private IVR_UnityVRHead riftHead;
//        [HideInInspector]
//        private IVR_SteamVRHead steamVRHead;

        void Start() {
            // This dummy code is here to ensure the checkbox is present in editor
        }

        public void StartTarget(IVR_Kinect2 ivrKinect, bool present) {
            this.ivrKinect = ivrKinect;

            extrapolation = true;

            riftHead = GetComponent<IVR_UnityVRHead>();
            //steamVRHead = GetComponent<IVR_SteamVRHead>();
        }

        public override void UpdateController() {
            if (!ivrKinect.present || !enabled || !ivrKinect.GetNewFrame())
                return;

            Vector3 neckPos = ivrKinect.GetBonePos(IVR_Kinect2.BoneType.ShoulderCenter);
            if (neckPos != Vector3.zero) {
                controllerPosition = neckPos;

                if (headRotation == true) {
                    Vector3 headPos = ivrKinect.GetBonePos(IVR_Kinect2.BoneType.Head);

                    Vector3 direction = headPos - neckPos;
                    controllerRotation = Quaternion.LookRotation(Vector3.forward, direction);
                }

                ivrKinect.Tracking = true;
                tracking = ivrKinect.Tracking;

                if (!selected) {
                    if (riftHead != null && riftHead.isSelected() && riftHead.positionalTracking)
                        CalibrateWithSelected();
                    //else if (steamVRHead != null && steamVRHead.isTracking() && steamVRHead.isSelected())
                    //    CalibrateWithSelected();
                }

                base.UpdateController();
            }
        }

        private void CalibrateWithSelected() {
            extension.trackerPosition = this.transform.position - ivr.transform.position - this.controllerPosition;
        }

        public override void OnTargetReset() {
            extension.trackerPosition = startPosition - this.controllerPosition;
        }
#endif
    }
}