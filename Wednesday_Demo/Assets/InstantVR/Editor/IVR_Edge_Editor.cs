/* InstantVR Edge Editor
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.6.0
 * date: September 23, 2016
 * 
 */

 using UnityEditor;
using UnityEngine;
using System.IO;

namespace IVR {
    [InitializeOnLoad]
    public class InstantVR_Edge_Editor {
        static InstantVR_Edge_Editor() {
            CheckIVREdgeDefine();
            CheckPhotonDefine();
            IVR_Configuration.CheckExtensions();
        }

        private static void CheckIVREdgeDefine() {
            InstantVR_Editor.GlobalDefine("INSTANTVR_EDGE");
        }


        public static void CheckPhotonDefine() {
            if (isPhotonInstalled()) {
                InstantVR_Editor.GlobalDefine("IVR_PHOTON");
            } else {
                InstantVR_Editor.GlobalUndefine("IVR_PHOTON");
            }
        }

        private static bool isPhotonInstalled() {
            string path = Application.dataPath + "/Plugins/Photon3Unity3D.dll";
            return File.Exists(path);
        }
    }
}