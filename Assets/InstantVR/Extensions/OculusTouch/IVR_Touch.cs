/* 
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
