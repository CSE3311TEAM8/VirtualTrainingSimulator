/* InstantVR Kinematics
 * author: Pascal Serrarnes
 * email: support@passervr.com
 * version: 3.7.0
 * date: December 30, 2016

Changes: 
 * - Implemented touching objects with basic physics
 */

using UnityEngine;

namespace IVR {
    public class IVR_Kinematics : MonoBehaviour {

        [HideInInspector]
        public Transform target;
        protected Transform constrainedTarget;
        protected Rigidbody kinematicRigidbody;

        private GameObject constrainedGameObject;

        public virtual Transform Kinematize(GameObject gameObject) {
            if (constrainedTarget == null) {
                constrainedGameObject = new GameObject();
                constrainedGameObject.name = "ConstrainedTarget";
                constrainedTarget = constrainedGameObject.transform;
                constrainedTarget.localScale = Vector3.one;
            }
            constrainedTarget.position = target.position;
            constrainedTarget.rotation = target.rotation;

            kinematicRigidbody = gameObject.GetComponent<Rigidbody>();
            if (kinematicRigidbody != null)
                kinematicRigidbody.isKinematic = true;

            return constrainedTarget;
        }

        public virtual void Kinematize(Rigidbody rigidbody) {
            kinematicRigidbody = rigidbody;

            if (kinematicRigidbody != null) {
                kinematicRigidbody.isKinematic = true;
            }

            BasicHandPhysics kp = rigidbody.GetComponent<BasicHandPhysics>();
            if (kp == null) {
                kp = rigidbody.gameObject.AddComponent<BasicHandPhysics>();
            }
            kp.handMovements = this.GetComponent<IVR_HandMovements>();
        }

        protected Vector3 linearVelocity;
        private Vector3 angularVelocity;

        public virtual void Unkinematize() {

            if (constrainedTarget != null) {
                Destroy(constrainedTarget.gameObject);
            }

            if (kinematicRigidbody != null) {
                kinematicRigidbody.isKinematic = false;
                kinematicRigidbody.AddForce(linearVelocity, ForceMode.VelocityChange);
                kinematicRigidbody.AddTorque(angularVelocity, ForceMode.VelocityChange);
            }
        }

        protected Vector3 lastConstrainedTargetPosition;
        private Quaternion prevRotation;

        public virtual void UpdateKR() {
            if (constrainedTarget != null) {
                constrainedTarget.position = target.position;
                constrainedTarget.rotation = target.rotation;

                linearVelocity = (constrainedTarget.position - lastConstrainedTargetPosition) / Time.deltaTime;
                angularVelocity = (constrainedTarget.rotation * Quaternion.Inverse(prevRotation)).eulerAngles / Time.deltaTime;

                lastConstrainedTargetPosition = constrainedTarget.position;
                prevRotation = constrainedTarget.rotation;
            }

            if (kinematicRigidbody != null) {
                kinematicRigidbody.transform.position = target.position;
                kinematicRigidbody.transform.rotation = target.rotation;

                linearVelocity = (kinematicRigidbody.position - lastConstrainedTargetPosition) / Time.deltaTime;
                angularVelocity = (kinematicRigidbody.rotation * Quaternion.Inverse(prevRotation)).eulerAngles / Time.deltaTime;

                lastConstrainedTargetPosition = kinematicRigidbody.position;
                prevRotation = kinematicRigidbody.rotation;
            }

        }

        private Rigidbody joinedRigidbody = null;
        private Vector3 target2objPosition = Vector3.zero;
        private Quaternion target2objRotation = Quaternion.identity;

        public virtual void Join(GameObject obj, Transform target) {
            joinedRigidbody = obj.GetComponent<Rigidbody>();
            if (joinedRigidbody != null) {
                joinedRigidbody.isKinematic = true;

                target2objPosition = -target.position + obj.transform.position;
                target2objRotation = Quaternion.Inverse(target.rotation) * obj.transform.rotation;
            }
        }

        public virtual void Unjoin() {
            if (joinedRigidbody != null) {
                joinedRigidbody.isKinematic = false;
            }
        }

        public virtual void UpdateJoinedObject(Transform target) {
            if (joinedRigidbody != null) {
                joinedRigidbody.rotation = target.rotation * target2objRotation;
                joinedRigidbody.position = target.position + (target.rotation * target2objPosition);
            }
        }

        void OnDestroy() {
            if (constrainedGameObject != null)
                Destroy(constrainedGameObject);
        }

    }
}