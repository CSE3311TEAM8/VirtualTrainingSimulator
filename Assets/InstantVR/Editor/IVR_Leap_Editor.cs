/* InstantVR Leap extension editor
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 16, 2017
 * 
 * - Configuration support
 */

using UnityEngine;
using UnityEditor;

namespace IVR {

    [CustomEditor(typeof(IVR_Leap))]
    public class IVR_Leap_Editor : IVR_Extension_Editor {
        public override void OnInspectorGUI() {
#if !IVR_LEAP
            if (!IVR_Configuration.CheckExtensionLeap()) {
                EditorGUILayout.HelpBox("Leap Motion support is disabled. Please go to Edit Menu->Preferences->InstantVR and enable Leap Motion support.", MessageType.Warning, true);
            } else {
                EditorGUILayout.HelpBox("Leap Motion support is enabled.", MessageType.Info, true);
            }
#endif
            base.OnInspectorGUI();
        }

#if IVR_LEAP
        private InstantVR ivr;
        private IVR_Leap ivrLeap;
        private IVR_LeapHand leapLeftHand, leapRightHand;

        void OnDestroy() {
            if (ivr != null && ivrLeap == null) {
                if (ivr.leftHandTarget != null) {
                    leapLeftHand = ivr.leftHandTarget.GetComponent<IVR_LeapHand>();
                    if (leapLeftHand != null)
                        DestroyImmediate(leapLeftHand, true);
                }

                if (ivr.rightHandTarget != null) {
                    leapRightHand = ivr.rightHandTarget.GetComponent<IVR_LeapHand>();
                    if (leapRightHand != null)
                        DestroyImmediate(leapRightHand, true);
                }
            }
        }

        void OnEnable() {
            ivrLeap = (IVR_Leap)target;
            if (!ivrLeap)
                return;

            ivr = ivrLeap.GetComponent<InstantVR>();

            leapLeftHand = ivr.leftHandTarget.GetComponent<IVR_LeapHand>();
            if (leapLeftHand == null) {
                leapLeftHand = ivr.leftHandTarget.gameObject.AddComponent<IVR_LeapHand>();
                leapLeftHand.extension = ivrLeap;
            }

            leapRightHand = ivr.rightHandTarget.GetComponent<IVR_LeapHand>();
            if (leapRightHand == null) {
                leapRightHand = ivr.rightHandTarget.gameObject.AddComponent<IVR_LeapHand>();
                leapRightHand.extension = ivrLeap;
            }

            IVR_Extension[] extensions = ivr.GetComponents<IVR_Extension>();
            if (ivrLeap.priority == -1)
                ivrLeap.priority = extensions.Length - 1;
            for (int i = 0; i < extensions.Length; i++) {
                if (ivrLeap == extensions[i]) {
                    while (i < ivrLeap.priority) {
                        MoveUp(leapLeftHand);
                        MoveUp(leapRightHand);
                        ivrLeap.priority--;
                        //Debug.Log ("Leap Move up to : " + i + " now: " + ivrLeap.priority);
                    }
                    while (i > ivrLeap.priority) {
                        MoveDown(leapLeftHand);
                        MoveDown(leapRightHand);
                        ivrLeap.priority++;
                        //Debug.Log ("Leap Move down to : " + i + " now: " + ivrLeap.priority);
                    }
                }
            }
        }
#endif
    }
}