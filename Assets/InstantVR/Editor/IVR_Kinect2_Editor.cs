/* InstantVR Kinect 2 extension editor
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 1, 2017
 *
 * - Configuration support
 */


using UnityEditor;

namespace IVR {

    [CustomEditor(typeof(IVR_Kinect2))]
    public class IVR_Kinect2_Editor : IVR_Extension_Editor {

        public override void OnInspectorGUI() {
            if (!IVR_Configuration.CheckExtensionKinect()) {
                EditorGUILayout.HelpBox("Kinect 2 support is disabled. Please go to Edit Menu->Preferences->InstantVR and enable Kinect 2 support.", MessageType.Warning, true);
            } else {
                EditorGUILayout.HelpBox("Kinect 2 support is enabled.", MessageType.Info, true);
            }
            base.OnInspectorGUI();
        }

#if IVR_KINECT

        private InstantVR ivr;
        private IVR_Kinect2 ivrKinect;

        private IVR_Kinect2Head kinectHead;
        private IVR_Kinect2Hand kinectLeftHand, kinectRightHand;
        private IVR_Kinect2Hip kinectHip;
        private IVR_Kinect2Foot kinectLeftFoot, kinectRightFoot;

        void OnDestroy() {
            if (ivrKinect == null && ivr != null) {
                kinectHead = ivr.headTarget.GetComponent<IVR_Kinect2Head>();
                if (kinectHead != null)
                    DestroyImmediate(kinectHead, true);

                kinectLeftHand = ivr.leftHandTarget.GetComponent<IVR_Kinect2Hand>();
                if (kinectLeftHand != null)
                    DestroyImmediate(kinectLeftHand, true);

                kinectRightHand = ivr.rightHandTarget.GetComponent<IVR_Kinect2Hand>();
                if (kinectRightHand != null)
                    DestroyImmediate(kinectRightHand, true);

                kinectHip = ivr.hipTarget.GetComponent<IVR_Kinect2Hip>();
                if (kinectHip != null)
                    DestroyImmediate(kinectHip, true);

                kinectLeftFoot = ivr.leftFootTarget.GetComponent<IVR_Kinect2Foot>();
                if (kinectLeftFoot != null)
                    DestroyImmediate(kinectLeftFoot, true);

                kinectRightFoot = ivr.rightFootTarget.GetComponent<IVR_Kinect2Foot>();
                if (kinectRightFoot != null)
                    DestroyImmediate(kinectRightFoot, true);
            }
        }

        void OnEnable() {
            ivrKinect = (IVR_Kinect2)target;
            ivr = ivrKinect.GetComponent<InstantVR>();

            if (ivr != null) {
                kinectHead = ivr.headTarget.GetComponent<IVR_Kinect2Head>();
                if (kinectHead == null) {
                    kinectHead = ivr.headTarget.gameObject.AddComponent<IVR_Kinect2Head>();
                    kinectHead.extension = ivrKinect;
                }

                kinectLeftHand = ivr.leftHandTarget.GetComponent<IVR_Kinect2Hand>();
                if (kinectLeftHand == null) {
                    kinectLeftHand = ivr.leftHandTarget.gameObject.AddComponent<IVR_Kinect2Hand>();
                    kinectLeftHand.extension = ivrKinect;
                }

                kinectRightHand = ivr.rightHandTarget.GetComponent<IVR_Kinect2Hand>();
                if (kinectRightHand == null) {
                    kinectRightHand = ivr.rightHandTarget.gameObject.AddComponent<IVR_Kinect2Hand>();
                    kinectRightHand.extension = ivrKinect;
                }

                kinectHip = ivr.hipTarget.GetComponent<IVR_Kinect2Hip>();
                if (kinectHip == null) {
                    kinectHip = ivr.hipTarget.gameObject.AddComponent<IVR_Kinect2Hip>();
                    kinectHip.extension = ivrKinect;
                }

                kinectLeftFoot = ivr.leftFootTarget.GetComponent<IVR_Kinect2Foot>();
                if (kinectLeftFoot == null) {
                    kinectLeftFoot = ivr.leftFootTarget.gameObject.AddComponent<IVR_Kinect2Foot>();
                    kinectLeftFoot.extension = ivrKinect;
                }

                kinectRightFoot = ivr.rightFootTarget.GetComponent<IVR_Kinect2Foot>();
                if (kinectRightFoot == null) {
                    kinectRightFoot = ivr.rightFootTarget.gameObject.AddComponent<IVR_Kinect2Foot>();
                    kinectRightFoot.extension = ivrKinect;
                }

                IVR_Extension[] extensions = ivr.GetComponents<IVR_Extension>();
                if (ivrKinect.priority == -1)
                    ivrKinect.priority = extensions.Length - 1;
                for (int i = 0; i < extensions.Length; i++) {
                    if (ivrKinect == extensions[i]) {
                        while (i < ivrKinect.priority) {
                            MoveUp(kinectHead);
                            MoveUp(kinectLeftHand);
                            MoveUp(kinectRightHand);
                            MoveUp(kinectHip);
                            MoveUp(kinectLeftFoot);
                            MoveUp(kinectRightFoot);
                            ivrKinect.priority--;
                            //Debug.Log ("Kinect 2 Move up to : " + i + " now: " + ivrKinect.priority);
                        }
                        while (i > ivrKinect.priority) {
                            MoveDown(kinectHead);
                            MoveDown(kinectLeftHand);
                            MoveDown(kinectRightHand);
                            MoveDown(kinectHip);
                            MoveDown(kinectLeftFoot);
                            MoveDown(kinectRightFoot);
                            ivrKinect.priority++;
                            //Debug.Log ("Kinect 2 Move down to : " + i + " now: " + ivrKinect.priority);
                        }
                    }
                }
            }
        }


        private void CheckIVRAdvancedDefine() {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains("INSTANTVR_ADVANCED")) {
                string newScriptDefines = scriptDefines + " INSTANTVR_ADVANCED";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newScriptDefines);
            }
        }
#endif
    }

#if IVR_KINECT
    [CustomEditor(typeof(IVR_Kinect2Head))]
    public class IVR_Kinect2Head_Editor : IVR_Controller_Editor { }
    [CustomEditor(typeof(IVR_Kinect2Hand))]
    public class IVR_Kinect2Hand_Editor : IVR_Controller_Editor { }
    [CustomEditor(typeof(IVR_Kinect2Hip))]
    public class IVR_Kinect2Hip_Editor : IVR_Controller_Editor { }
    [CustomEditor(typeof(IVR_Kinect2Foot))]
    public class IVR_Kinect2Foot_Editor : IVR_Controller_Editor { }
#endif
}

