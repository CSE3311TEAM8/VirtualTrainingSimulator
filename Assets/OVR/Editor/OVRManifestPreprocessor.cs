/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;

class OVRManifestPreprocessor 
{
	[MenuItem("Tools/Oculus/Create store-compatible AndroidManifest.xml", false, 100000)]	
	static void GenerateManifestForSubmission() 
	{
		string srcFile = Application.dataPath + "/OVR/Editor/AndroidManifest.OVRSubmission.xml";

		if (!File.Exists(srcFile))
		{
			Debug.LogError ("Cannot find Android manifest template for submission." +
				" Please delete the OVR folder and reimport the Oculus Utilities.");
			return;
		}

		string manifestFolder = Application.dataPath + "/Plugins/Android";

		if (!Directory.Exists(manifestFolder))
			Directory.CreateDirectory(manifestFolder);

		string dstFile = manifestFolder + "/AndroidManifest.xml";

		if (File.Exists(dstFile))
		{
			Debug.LogWarning("Cannot create Oculus store-compatible manifest due to conflicting file: \""
				+ dstFile + "\". Please remove it and try again.");
			return;
		}

		File.Copy(srcFile, dstFile);
    }
}
