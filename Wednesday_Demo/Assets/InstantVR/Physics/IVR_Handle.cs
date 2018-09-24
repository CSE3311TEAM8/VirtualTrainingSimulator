/* InstantVR Handle
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.2.3
 * date: December 29, 2015
 * 
 */

 using UnityEngine;

public class IVR_Handle : MonoBehaviour {
	public Vector3 position = Vector3.zero;
	public Vector3 rotation = Vector3.zero;

	public float range = 0.2f;

    public enum Hand {
        Both,
        Left,
        Right
    }
    public Hand hand;

    protected Joint grabJoint;
    
    protected StoredRigidbody grabbedRBdata;
    protected Transform originalParent;

    public virtual void Grab(GameObject handObj, Transform handTransform, Transform handPalm) {
        if (gameObject.isStatic) {
            grabJoint = handObj.AddComponent<FixedJoint>();
            grabJoint.connectedBody = GetComponent<Rigidbody>();
        } else {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null) {
                grabbedRBdata = new StoredRigidbody(rigidbody);
                Destroy(rigidbody);
            } else {
                grabbedRBdata = null;
            }

            originalParent = transform.parent;
            transform.parent = handTransform;
        }
    }

    public virtual void LetGo(GameObject handObj) {
        if (grabJoint != null)
            Destroy(grabJoint);

        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = gameObject.AddComponent<Rigidbody>();
        if (grabbedRBdata != null)
            grabbedRBdata.CopyToRigidbody(rigidbody);

        if (originalParent != null) {
            transform.parent = originalParent;
            originalParent = null;
        }

        Rigidbody handRigidbody = handObj.GetComponent<Rigidbody>();
        if (handRigidbody != null) {
            rigidbody.velocity = handRigidbody.velocity;
            rigidbody.angularVelocity = handRigidbody.angularVelocity;
        }
    }
}

public class StoredRigidbody {
    public float mass = 1;
    public float drag;
    public float angularDrag = 0.05F;
    public bool useGravity = true;
    public bool isKinematic;
    public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
    public CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Discrete;
    public RigidbodyConstraints constraints = RigidbodyConstraints.None;

    public StoredRigidbody(Rigidbody rb) {
        mass = rb.mass;
        drag = rb.drag;
        angularDrag = rb.angularDrag;
        useGravity = rb.useGravity;
        isKinematic = rb.isKinematic;
        interpolation = rb.interpolation;
        collisionDetectionMode = rb.collisionDetectionMode;
        constraints = rb.constraints;
    }

    public void CopyToRigidbody(Rigidbody rb) {
        if (rb != null) {
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
            rb.interpolation = interpolation;
            rb.collisionDetectionMode = collisionDetectionMode;
            rb.constraints = constraints;
        }
    }
}
