/* InstantVR Advanced Editor
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.1
 * date: May 14, 2017
 * 
 * - Removed leap Motion Check, this is now handled by Configuration
 */

using UnityEngine;
using UnityEditor;
using System.IO;

namespace IVR {
    [InitializeOnLoad]
    public class IVR_Advanced_Editor {
        static IVR_Advanced_Editor() {
            CheckIVRAdvancedDefine();
            CheckPlayMakerDefine();
        }

        private static void CheckIVRAdvancedDefine() {
            InstantVR_Editor.GlobalDefine("INSTANTVR_ADVANCED");
        }

        #region PlayMaker
        private static void CheckPlayMakerDefine() {
            if (isPlayMakerInstalled()) {
                InstantVR_Editor.GlobalDefine("PLAYMAKER");
            } else {
                InstantVR_Editor.GlobalUndefine("PLAYMAKER");
            }
        }

        private static bool isPlayMakerInstalled() {
            string path = Application.dataPath + "/PlayMaker/Editor/PlayMakerEditor.dll";
            return File.Exists(path);
        }
        #endregion

    }
}