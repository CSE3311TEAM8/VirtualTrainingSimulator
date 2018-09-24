/* InstantVR Full Hand Physics
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 7, 2017
 *
 * - improved physics
 */

#if INSTANTVR_EDGE

//#define DEBUG_FORCE

using UnityEngine;

namespace IVR {

    public class FullHandPhysics : BasicHandPhysics {

        public enum PhysicsMode {
            Kinematic,
            NonKinematic,
            HybridKinematic
        }
        public PhysicsMode mode = PhysicsMode.HybridKinematic;

        Transform handTarget;
        [HideInInspector]
        private Rigidbody handRigidbody;

        [HideInInspector]
        private Transform forearm;
        [HideInInspector]
        private float forearmLength;

        private GameObject touchingObject;
        public int maxStrength = 100;
        public int maxAcceleration = 200;

        private bool colliding;
        public bool hasCollided = false;


        public void Initialize(Transform handTarget, Transform forearm, float forearmLength) {
            this.forearm = forearm;
            this.forearmLength = forearmLength;

            if (enabled) {
                handRigidbody = GetComponent<Rigidbody>();
                if (handRigidbody != null) {
                    Kinematize(handRigidbody, mode);
                    handRigidbody.maxAngularVelocity = 20;
                }
            }
        }

        #region Update
        public void FixedUpdate() {
            CalculateVelocity();
        }

        public virtual void ManualFixedUpdate(Transform _handTarget) {
            handTarget = _handTarget;

            if (hasCollided && !colliding) {
                touchingObject = null;
            }

            if (hasCollided && touchingObject == null) { // Object may be destroyed
                hasCollided = false;
                //Debug.Break();
            }

            if (!hasCollided)
                handMovements.OnTouchEnd();

            if (handRigidbody == null)
                return;

            float distance = Vector3.Distance(handRigidbody.transform.position, forearm.position) - forearmLength;
            if (forearmLength > 0 && distance > 0.05F) {
                SetKinematic(handRigidbody, true);
            }

            UpdateRigidbody();

            colliding = false;
        }

        public void UpdateRigidbody() {
            if (handRigidbody == null || handTarget == null)
                return;

            if (mode == PhysicsMode.NonKinematic && handRigidbody.isKinematic)
                SetKinematic(handRigidbody, false);
            if (!hasCollided && !handRigidbody.useGravity && mode != PhysicsMode.NonKinematic) {
                SetKinematic(handRigidbody, true);
            }

            if (handRigidbody != null) {
                if (handRigidbody.isKinematic) {
                    UpdateKinematicRigidbody();
                } else {
                    UpdateNonKinematicRigidbody();
                }
            }
        }

        private void UpdateKinematicRigidbody() {
            Vector3 d = handTarget.position - handRigidbody.position;

            Quaternion rot = Quaternion.Inverse(handRigidbody.rotation) * handTarget.rotation;
            float angle;
            Vector3 axis;
            rot.ToAngleAxis(out angle, out axis);

            handRigidbody.MovePosition(handRigidbody.position + d);
            if (angle != 0 && Mathf.Abs(axis.magnitude) != Mathf.Infinity)
                handRigidbody.MoveRotation(handRigidbody.rotation * Quaternion.AngleAxis(angle, axis)); // * handTarget.hand.target2BoneRotation);
        }

        private void UpdateNonKinematicRigidbody() {
            Vector3 torque = CalculateTorque();
            ApplyTorqueAtPosition(torque, handMovements.handPalm.position);

            Vector3 force = CalculateForce();
            ApplyForce(force);
            

            //if (!hasCollided && !handRigidbody.useGravity && mode != PhysicsMode.NonKinematic) {
            //    SetKinematic(handRigidbody, true);
            //}
        }
        #endregion

        #region Events
        public override void OnTriggerEnter(Collider collider) {
            bool otherHasKinematicPhysics = false;
            bool otherIsHumanoid = false;

            Rigidbody otherRigidbody = collider.attachedRigidbody;
            if (otherRigidbody != null) {
                FullHandPhysics kp = otherRigidbody.GetComponent<FullHandPhysics>();
                otherHasKinematicPhysics = (kp != null);
                InstantVR ivr = otherRigidbody.GetComponent<InstantVR>();
                otherIsHumanoid = (ivr != null);
            }

            if (handRigidbody != null && handRigidbody.isKinematic && (!collider.isTrigger || otherHasKinematicPhysics) && !otherIsHumanoid) {
                colliding = true;
                hasCollided = true;
                if (otherRigidbody != null)
                    touchingObject = otherRigidbody.gameObject;
                else
                    touchingObject = collider.gameObject;
                SetKinematic(handRigidbody, false);
            }

            if (hasCollided) {
                Rigidbody objRigidbody = collider.attachedRigidbody;
                if (objRigidbody != null) {
                    handMovements.OnTouchStart(objRigidbody.gameObject);
                } else
                    handMovements.OnTouchStart(collider.gameObject);
            }
        }

        public void OnTriggerExit() {
            if (!hasCollided)
                handMovements.OnTouchEnd();
        }

        public override void OnCollisionEnter(Collision collision) {
            colliding = true;
            base.OnCollisionEnter(collision);
        }

        public void OnCollisionStay() {
            colliding = true;
        }

        public override void OnCollisionExit(Collision collision) {
            if (handRigidbody != null && !handRigidbody.useGravity) {
                RaycastHit hit;
                if (!handRigidbody.SweepTest(handTarget.transform.position - handRigidbody.position, out hit)) {
                    hasCollided = false;
                    touchingObject = null;
                }
            }
            if (!hasCollided)
                handMovements.OnTouchEnd();
        }
#endregion

        public static void Kinematize(Rigidbody rigidbody, PhysicsMode mode) {
            if (rigidbody != null) {
                if (rigidbody.useGravity || mode == PhysicsMode.NonKinematic)
                    SetKinematic(rigidbody, false);
                else
                    SetKinematic(rigidbody, true);
            }
        }

        public static void Unkinematize(Rigidbody rigidbody) {
            SetKinematic(rigidbody, false);
        }

        public void DeterminePhysicsMode(float kinematicMass = 1) {
            mode = DeterminePhysicsMode(handRigidbody, kinematicMass);
        }

        public static PhysicsMode DeterminePhysicsMode(Rigidbody rigidbody, float kinematicMass = 1) {
            if (rigidbody == null)
                return PhysicsMode.Kinematic;

            PhysicsMode physicsMode;
            if (rigidbody.useGravity) {
                physicsMode = PhysicsMode.NonKinematic;
            } else {
                float mass = CalculateTotalMass(rigidbody);
                if (mass > kinematicMass)
                    physicsMode = PhysicsMode.NonKinematic;
                else
                    physicsMode = PhysicsMode.HybridKinematic;
            }
            return physicsMode;
        }

        public static float CalculateTotalMass(Rigidbody rigidbody) {
            if (rigidbody == null)
                return 0;

            float mass = rigidbody.gameObject.isStatic ? Mathf.Infinity : rigidbody.mass;
            Joint[] joints = rigidbody.GetComponents<Joint>();
            for (int i = 0; i < joints.Length; i++) {
                // Seems to result in cycle in spine in some cases
                //if (joints[i].connectedBody != null)
                //    mass += CalculateTotalMass(joints[i].connectedBody);
                //else
                mass = Mathf.Infinity;
            }
            return mass;
        }

        public Vector3 boneVelocity;
        private Vector3 lastPosition = Vector3.zero;
        private void CalculateVelocity() {
            if (lastPosition != Vector3.zero) {
                boneVelocity = (handRigidbody.position - lastPosition) / Time.fixedDeltaTime;
            }
            lastPosition = handRigidbody.position;
        }

        #region Force
        private Vector3 CalculateForce() {
            Vector3 locationDifference = handTarget.position - handRigidbody.position;
            Vector3 force = locationDifference * maxStrength;

            force += CalculateForceDamper();

            return force;
        }

        private const float damping = 0;
        private float lastDistanceTime;
        private Vector3 lastDistanceToTarget;
        private Vector3 CalculateForceDamper() {
            Vector3 distanceToTarget = handRigidbody.position - handTarget.position;

            float deltaTime = Time.fixedTime - lastDistanceTime;

            Vector3 damper = Vector3.zero;
            if (deltaTime < 0.1F) {
                Vector3 velocityTowardsTarget = (distanceToTarget - lastDistanceToTarget) / deltaTime;

                damper = -velocityTowardsTarget * damping;

                //Compensate for absolute rigidbody speed (specifically when on a moving platform)
                Vector3 residualVelocity = handRigidbody.velocity - velocityTowardsTarget;
                damper += residualVelocity * 10;
            }
            lastDistanceToTarget = distanceToTarget;
            lastDistanceTime = Time.fixedTime;

            return damper;
        }

        private void ApplyForce(Vector3 force) {
            if (float.IsNaN(force.magnitude))
                return;

            handRigidbody.AddForceAtPosition(force, handMovements.handPalm.position);
#if DEBUG_FORCE
            Debug.DrawRay(handMovements.handPalm.position, force / 10, Color.yellow);
#endif
        }
        #endregion

        #region Torque
        private Vector3 CalculateTorque() {
            Quaternion sollRotation = handTarget.rotation;
            Quaternion istRotation = handRigidbody.rotation;
            Quaternion dRot = sollRotation * Quaternion.Inverse(istRotation);

            float angle;
            Vector3 axis;
            dRot.ToAngleAxis(out angle, out axis);
            angle = Angles.Normalize(angle);

            Vector3 angleDifference = axis.normalized * (angle * Mathf.Deg2Rad);
            Vector3 torque = angleDifference * maxStrength * 0.1F;
            return torque;
        }

        private void ApplyTorqueAtPosition(Vector3 torque, Vector3 posToApply) {
            if (float.IsNaN(torque.magnitude))
                return;

            Vector3 torqueAxis = torque.normalized;
            Vector3 ortho = new Vector3(1, 0, 0);

            // prevent torqueAxis and ortho from pointing in the same direction
            if (((torqueAxis - ortho).sqrMagnitude < Mathf.Epsilon) || ((torqueAxis + ortho).sqrMagnitude < Mathf.Epsilon)) {
                ortho = new Vector3(0, 1, 0);
            }

            ortho = Vector3OrthoNormalize(torqueAxis, ortho);
            // calculate force 
            Vector3 force = Vector3.Cross(0.5f * torque, ortho);

            handRigidbody.AddForceAtPosition(force, posToApply + ortho);
            handRigidbody.AddForceAtPosition(-force, posToApply - ortho);

#if DEBUG_TORQUE
            UnityEngine.Debug.DrawRay(posToApply + ortho / 20, force / 10, Color.yellow);
            UnityEngine.Debug.DrawLine(posToApply + ortho / 20, posToApply - ortho / 20, Color.yellow);
            UnityEngine.Debug.DrawRay(posToApply - ortho / 20, -force / 10, Color.yellow);
#endif
        }

        private Vector3 Vector3OrthoNormalize(Vector3 a, Vector3 b) {
            Vector3 r = Vector3.Cross(a.normalized, b).normalized;
            return r;
        }

        #endregion

        public static void SetKinematic(Rigidbody rigidbody, bool b) {
            if (rigidbody == null)
                return;

            GameObject obj = rigidbody.gameObject;
            if (!obj.isStatic) {
                rigidbody.isKinematic = b;
                SetColliderToTrigger(obj, b);
            }
        }

        public static void SetColliderToTrigger(GameObject obj, bool b) {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            for (int j = 0; j < colliders.Length; j++)
                colliders[j].isTrigger = b;
        }
    }
}
#endif