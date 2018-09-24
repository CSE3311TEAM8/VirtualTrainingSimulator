/* InstantVR Handle
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.3.0
 * date: February 5, 2016
 *
 * - added namespace
 */

using UnityEngine;

namespace IVR {

    public class IVR_BarHandle : IVR_Handle {

        void OnDrawGizmos() {
            Matrix4x4 m = Matrix4x4.identity;
            Vector3 p = transform.TransformPoint(position);
            Quaternion q = Quaternion.Euler(rotation);
            m.SetTRS(p, transform.rotation * q, Vector3.one);
            Gizmos.color = Color.yellow;
            Gizmos.matrix = m;

            Gizmos.DrawCube(Vector3.zero, new Vector3(0.03f, 0.10f, 0.04f));
            //	Gizmos.DrawWireSphere(Vector3.zero, range);
        }

        /*
        public override void Grab(GameObject handObj, Transform handTransform, Transform handPalm) {
            Vector3 handlePosition = this.transform.TransformPoint(this.position);

            if (Vector3.Distance(handPalm.position, handlePosition) < this.range) {

                Vector3 handleWPos = this.transform.TransformPoint(this.position);
                Quaternion handleWRot = this.transform.rotation * Quaternion.Euler(this.rotation);

                Vector3 newPosition;
                Quaternion newRotation;
                IVR_HandMovements.HandGrabPosition(handObj.transform, handPalm, handleWPos, handleWRot, out newPosition, out newRotation);

                handTransform.position = newPosition;
                handTransform.rotation = newRotation;

                base.Grab(handObj, handTransform, handPalm);
            } else {
                base.Grab(handObj, handTransform, handPalm);
            }
        }
        */
    }
}