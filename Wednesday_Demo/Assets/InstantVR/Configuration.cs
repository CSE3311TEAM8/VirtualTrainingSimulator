/* InstantVR Configuration
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.6
 * date: August 14, 2017
 * 
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace IVR {

    public static class IVR_Configuration {
        private static bool prefsLoaded = false;

        private static bool steamVRSupport = true;
        private static bool oculusSupport = true;
        private static bool leapSupport = true;
        private static bool kinectSupport = true;
        private static bool hydraSupport = true;

        [PreferenceItem("InstantVR")]
        public static void PreferencesGUI() {

            // Load the preferences
            if (!prefsLoaded) {
                steamVRSupport = EditorPrefs.GetBool("SteamVRSupportKey", true);
                oculusSupport = EditorPrefs.GetBool("OculusSupportKey", true);
                leapSupport = EditorPrefs.GetBool("LeapSupportKey", true);
                kinectSupport = EditorPrefs.GetBool("KinectSupportKey", true);
                hydraSupport = EditorPrefs.GetBool("HydraSupportKey", true);
                prefsLoaded = true;
            }

            // Preferences GUI
            steamVRSupport = EditorGUILayout.Toggle("SteamVR Support", steamVRSupport);
            oculusSupport = EditorGUILayout.Toggle("Oculus Support", oculusSupport);

            if (!isLeapAvailable()) {
                EditorGUI.BeginDisabledGroup(true);
                leapSupport = EditorGUILayout.Toggle("Leap Motion Support", leapSupport);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.HelpBox("Leap Motion Core Assets are not available. Please download the Core Assets using the button below and import them into this project.", MessageType.Warning, true);
                if (GUILayout.Button("Download Leap Motion Unity Core Assets"))
                    Application.OpenURL("https://developer.leapmotion.com/unity");
            } else
                leapSupport = EditorGUILayout.Toggle("Leap Motion Support", leapSupport);

            kinectSupport = EditorGUILayout.Toggle("Kinect Support", kinectSupport);
            hydraSupport = EditorGUILayout.Toggle("Hydra Support", hydraSupport);


            // Save the preferences
            if (GUI.changed) {
                EditorPrefs.SetBool("SteamVRSupportKey", steamVRSupport);
                EditorPrefs.SetBool("OculusSupportKey", oculusSupport);
                EditorPrefs.SetBool("LeapSupportKey", leapSupport);
                EditorPrefs.SetBool("KinectSupportKey", kinectSupport);
                EditorPrefs.SetBool("HydraSupportKey", hydraSupport);

                CheckExtensions();
            }
        }

        public static void CheckExtensions() {
            if (!prefsLoaded) {
                steamVRSupport = EditorPrefs.GetBool("SteamVRSupportKey", true);
                oculusSupport = EditorPrefs.GetBool("OculusSupportKey", true);
                leapSupport = EditorPrefs.GetBool("LeapSupportKey", true);
                kinectSupport = EditorPrefs.GetBool("KinectSupportKey", true);
                hydraSupport = EditorPrefs.GetBool("HydraSupportKey", true);
                prefsLoaded = true;
            }

            CheckExtensionOculus();
            CheckExtensionSteamVR();
            CheckExtensionLeap();
            CheckExtensionKinect();
            CheckExtensionHydra();
        }

        #region SteamVR
        public static bool CheckExtensionSteamVR() {
            bool scriptAvailable = steamVrScriptAvailable();
            bool enabled = scriptAvailable && steamVRSupport;
            SetGlobalDefine("IVR_STEAMVR", enabled);
            return enabled;
        }

        private static bool steamVrScriptAvailable() {
            return File.Exists(Application.dataPath + "/InstantVR/Extensions/SteamVRcontroller/IVR_SteamVRController.cs");
        }
        #endregion

        #region Oculus
        public static bool CheckExtensionOculus() {
            bool scriptAvailable = oculusScriptAvailable();
            bool enabled = scriptAvailable && oculusSupport;
            SetGlobalDefine("IVR_OCULUS", enabled);
            return enabled;
        }

        private static bool oculusScriptAvailable() {
            return File.Exists(Application.dataPath + "/InstantVR/Extensions/OculusTouch/IVR_Touch.cs");
        }
        #endregion

        #region Leap
        public static bool CheckExtensionLeap() {
            bool scriptAvailable = leapScriptAvailable();
            bool pluginAvailable = isLeapAvailable();
            bool enabled = pluginAvailable && scriptAvailable && leapSupport;
            SetGlobalDefine("IVR_LEAP", enabled);
            return enabled;
        }

        private static bool leapScriptAvailable() {
            return File.Exists(Application.dataPath + "/InstantVR/Extensions/LeapMotion/IVR_Leap.cs");
        }

        private static bool isLeapAvailable() {
            string path1 = Application.dataPath + "/Plugins/x86/LeapC.dll";
            string path2 = Application.dataPath + "/Plugins/x86_64/LeapC.dll";
            if (File.Exists(path1) && File.Exists(path2))
                return true;

            // New location for Leap Core Assets v4.2 and higher
            path1 = Application.dataPath + "/LeapMotion/Core/Plugins/x86/LeapC.dll";
            path2 = Application.dataPath + "/LeapMotion/Core/Plugins/x86_64/LeapC.dll";
            return (File.Exists(path1) && File.Exists(path2));
        }
        #endregion

        #region Kinect
        public static bool CheckExtensionKinect() {
            bool scriptAvailable = kinectScriptAvailable();
            bool enabled = scriptAvailable && kinectSupport;
            SetGlobalDefine("IVR_KINECT", enabled);
            return enabled;
        }

        private static bool kinectScriptAvailable() {
            return File.Exists(Application.dataPath + "/InstantVR/Extensions/Kinect2/IVR_Kinect2.cs");
        }
        #endregion

        #region Hydra
        public static bool CheckExtensionHydra() {
            bool scriptAvailable = hydraScriptAvailable();
            bool enabled = scriptAvailable && hydraSupport;
            SetGlobalDefine("IVR_HYDRA", enabled);
            return enabled;
        }

        private static bool hydraScriptAvailable() {
            return File.Exists(Application.dataPath + "/InstantVR/Extensions/RazerHydra/IVR_Hydra.cs");
        }
        #endregion

        private static void SetGlobalDefine(string define, bool enabled) {
            if (enabled)
                GlobalDefine(define);
            else
                GlobalUndefine(define);
        }

        public static void GlobalDefine(string name) {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains(name)) {
                string newScriptDefines = scriptDefines + " " + name;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newScriptDefines);
            }
        }

        public static void GlobalUndefine(string name) {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (scriptDefines.Contains(name)) {
                int playMakerIndex = scriptDefines.IndexOf(name);
                string newScriptDefines = scriptDefines.Remove(playMakerIndex, name.Length);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newScriptDefines);
            }

        }
    }
}
#endif