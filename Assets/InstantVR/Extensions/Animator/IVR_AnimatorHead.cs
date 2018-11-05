/* InstantVR Animator
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.4
 * date: April 15, 2016
 * 
 * - added namespace
 */
using UnityEngine;

namespace IVR {

    public class IVR_AnimatorHead : IVR_Controller {

        public float headWeight = 0.9f;


        public override void StartController(InstantVR ivr) {
            extension = ivr.GetComponent<IVR_Animator>();
            base.StartController(ivr);
            tracking = true;

        }

        public override void UpdateController() {
            if (!enabled)
                return;

            controllerPosition = startPosition;
            controllerRotation = startRotation;

            base.UpdateController();
        }
    }
}