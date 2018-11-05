/* InstantVR Oculus Touch extension
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 4, 2017
 * 
 * - Configuration support
 */

using UnityEngine;

namespace IVR {

    [HelpURL("http://passervr.com/documentation/instantvr-extensions/oculus-touch/")]
    public class IVR_Touch : IVR_Extension {
#if IVR_OCULUS

        public override void StartExtension(InstantVR ivr) {
            base.StartExtension(ivr);
        }
#endif
    }
}
