/* InstantVR Steam VR Controller extension editor
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: July 25, 2016
 *
 * - Configuration support
 */

using UnityEditor;

namespace IVR {

    [CustomEditor(typeof(IVR_SteamVRController))]
    public class IVR_SteamVRController_Editor : IVR_Extension_Editor {

        public override void OnInspectorGUI() {
#if !IVR_STEAMVR
            if (!IVR_Configuration.CheckExtensionSteamVR()) {
                EditorGUILayout.HelpBox("Steam VR support is disabled. Please go to Edit Menu->Preferences->InstantVR and enable Steam VR support.", MessageType.Warning, true);
            } else {
                EditorGUILayout.HelpBox("Steam VR support is enabled.", MessageType.Info, true);
            }
#endif
            base.OnInspectorGUI();
        }

#if IVR_STEAMVR

        private InstantVR ivr;
        private IVR_SteamVRController ivrSteamVRcontroller;

        private IVR_SteamVRControllerHand steamLeftHand, steamRightHand;


        public void OnDestroy() {
            if (ivrSteamVRcontroller == null && ivr != null) {
                steamLeftHand = ivr.leftHandTarget.GetComponent<IVR_SteamVRControllerHand>();
                if (steamLeftHand != null)
                    DestroyImmediate(steamLeftHand, true);

                steamRightHand = ivr.rightHandTarget.GetComponent<IVR_SteamVRControllerHand>();
                if (steamRightHand != null)
                    DestroyImmediate(steamRightHand, true);
            }
        }

        public void OnEnable() {
            ivrSteamVRcontroller = (IVR_SteamVRController)target;
            ivr = ivrSteamVRcontroller.GetComponent<InstantVR>();

            if (ivr != null) {
                steamLeftHand = ivr.leftHandTarget.GetComponent<IVR_SteamVRControllerHand>();
                if (steamLeftHand == null) {
                    steamLeftHand = ivr.leftHandTarget.gameObject.AddComponent<IVR_SteamVRControllerHand>();
                    steamLeftHand.extension = ivrSteamVRcontroller;
                }

                steamRightHand = ivr.rightHandTarget.GetComponent<IVR_SteamVRControllerHand>();
                if (steamRightHand == null) {
                    steamRightHand = ivr.rightHandTarget.gameObject.AddComponent<IVR_SteamVRControllerHand>();
                    steamRightHand.extension = ivrSteamVRcontroller;
                }

                IVR_Extension[] extensions = ivr.GetComponents<IVR_Extension>();
                if (ivrSteamVRcontroller.priority == -1)
                    ivrSteamVRcontroller.priority = extensions.Length - 1;
                for (int i = 0; i < extensions.Length; i++) {
                    if (ivrSteamVRcontroller == extensions[i]) {
                        while (i < ivrSteamVRcontroller.priority) {
                            MoveUp(steamLeftHand);
                            MoveUp(steamRightHand);
                            ivrSteamVRcontroller.priority--;
                            //Debug.Log ("Steam Move up to : " + i + " now: " + ivrRift.priority);
                        }
                        while (i > ivrSteamVRcontroller.priority) {
                            MoveDown(steamLeftHand);
                            MoveDown(steamRightHand);
                            ivrSteamVRcontroller.priority++;
                            //Debug.Log ("Steam Move down to : " + i + " now: " + ivrRift.priority);
                        }
                    }
                }
            }
        }
#endif
    }
}