/* InstantVR Hand Physics
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 7, 2017
 *
 * - improved physics
 */

#if INSTANTVR_EDGE
using UnityEngine;
using IVR;

public class IVR_KinematicPhysics : IVR_Kinematics {
    public float strength = 100;

    public FullHandPhysics.PhysicsMode physicsMode = FullHandPhysics.PhysicsMode.HybridKinematic;

    public override void Kinematize(Rigidbody rigidbody) {
        FullHandPhysics handPhysics = rigidbody.GetComponent<FullHandPhysics>();
        if (handPhysics == null) {
            handPhysics = rigidbody.gameObject.AddComponent<FullHandPhysics>();
        }

        handPhysics.handMovements = this.GetComponent<IVR_HandMovements>();
        handPhysics.maxStrength = (int) this.strength;
        handPhysics.maxAcceleration = (int) this.strength;
        handPhysics.mode = physicsMode;
    }

    public override void Join(GameObject obj, Transform handPalm) {
    }

    public override void UpdateJoinedObject(Transform target) {
    }
}
#endif