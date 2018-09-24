/* InstantVR SteamVRcontroller extension
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.4
 * date: July 3, 2017
 * 
 * - Removed SteamVR_UpdatePoses dependency
 */

using UnityEngine;

namespace IVR {

    [HelpURL("http://passervr.com/documentation/instantvr-extensions/htc-vive/")]
    public class IVR_SteamVRController : IVR_Extension {
#if IVR_STEAMVR
        [HideInInspector]
        public SteamVR_ControllerManager steamManager;
#endif
        }
}