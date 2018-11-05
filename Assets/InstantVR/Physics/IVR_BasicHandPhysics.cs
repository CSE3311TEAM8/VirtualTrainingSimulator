/* Basic physics
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 7, 2017
 *
 * - Support for NoPhysics mode
 */

using UnityEngine;

namespace IVR {

    public class BasicHandPhysics : MonoBehaviour {
        public IVR_HandMovements handMovements;

#if INSTANTVR_ADVANCED
        public virtual void OnTriggerEnter(Collider other) {
            Rigidbody objRigidbody = other.attachedRigidbody;
            if (objRigidbody != null)
                handMovements.OnTouchStart(objRigidbody.gameObject);
            else
                handMovements.OnTouchStart(other.gameObject);
        }

        public void OnTriggerStay(Collider other) {
            Rigidbody objRigidbody = other.attachedRigidbody;
            if (objRigidbody != null)
                handMovements.GrabCheck(objRigidbody.gameObject);
            else
                handMovements.GrabCheck(other.gameObject);
        }

        public virtual void OnCollisionEnter(Collision collision) {
            Rigidbody objRigidbody = collision.rigidbody;
            if (objRigidbody != null)
                handMovements.OnTouchStart(objRigidbody.gameObject);
            else
                handMovements.OnTouchStart(collision.gameObject);
        }

        public virtual void OnCollisionExit(Collision collision) {
            handMovements.OnTouchEnd();
        }
#endif
    }
}