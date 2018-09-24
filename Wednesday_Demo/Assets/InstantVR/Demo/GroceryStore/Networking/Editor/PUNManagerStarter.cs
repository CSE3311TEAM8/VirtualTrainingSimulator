using UnityEditor;
using UnityEngine;
using System.IO;

namespace IVR {

#if IVR_PHOTON
    [CustomEditor(typeof(PUNManagerStarter))]
#endif
    public class PUNManagerStarter_Editor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            InstantVR_Edge_Editor.CheckPhotonDefine();
#if !IVR_PHOTON
    EditorGUILayout.HelpBox("Photon Unity Networking not found. Please download the package from the Unity Asset Store", MessageType.Warning, true);
#endif
        }
    }
}