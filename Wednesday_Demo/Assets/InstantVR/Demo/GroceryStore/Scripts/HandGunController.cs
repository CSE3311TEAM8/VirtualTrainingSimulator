/* HandgunController
 * author: Pascal Serrarnes
 * email: unity@serrarens.nl
 * version: 1.2.2
 * date: February 24, 2017
 * 
 * - Added default to the inputDevice switch
 */
using UnityEngine;
using UnityEngine.EventSystems;
using IVR;

public class HandGunController : EventTrigger {

    private bool shooting = false;
    private bool grabbed = false;

    private IVR_HandMovements handMovements;
    
	private Transform handgun;
	private Vector3 nozzleLocation = new Vector3(0f,0.132f,0.09f);

    private Collider gunCollider;

    private Light nozzleFlash;
    private ParticleSystem nozzleSmoke;

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerClick(eventData);

        if (eventData.currentInputModule.GetType() == typeof(IVR_Interaction)) {
            InteractionEventData interactionData = (InteractionEventData) eventData;
            InstantVR ivr = eventData.currentInputModule.transform.GetComponent<InstantVR>();

            switch (interactionData.inputDevice) {
                case InputDeviceIDs.LeftHand:
                    if (ivr.leftHandMovements.GetType() == typeof(IVR_HandMovements)) {
                        IVR_HandMovements handMovements = (IVR_HandMovements) ivr.leftHandMovements;
                        handMovements.NetworkingGrab(this.gameObject);
                    }
                    break;
                case InputDeviceIDs.RightHand:
                    if (ivr.rightHandMovements.GetType() == typeof(IVR_HandMovements)) {
                        IVR_HandMovements handMovements = (IVR_HandMovements) ivr.rightHandMovements;
                        handMovements.NetworkingGrab(this.gameObject);
                    }
                    break;
                case InputDeviceIDs.Head:
                    this.transform.parent = ivr.headTarget;
                    Rigidbody rigidbody = this.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                    break;
                default:
                    break;
            }

        }
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerClick(eventData);

        if (eventData.currentInputModule.GetType() == typeof(IVR_Interaction)) {
            InteractionEventData interactionData = (InteractionEventData) eventData;
            InstantVR ivr = eventData.currentInputModule.transform.GetComponent<InstantVR>();

            switch (interactionData.inputDevice) {
                case InputDeviceIDs.LeftHand:
                    if (ivr.leftHandMovements.GetType() == typeof(IVR_HandMovements)) {
                        IVR_HandMovements handMovements = (IVR_HandMovements) ivr.leftHandMovements;
                        handMovements.NetworkingLetGo();
                    }
                    break;
                case InputDeviceIDs.RightHand:
                    if (ivr.rightHandMovements.GetType() == typeof(IVR_HandMovements)) {
                        IVR_HandMovements handMovements = (IVR_HandMovements) ivr.rightHandMovements;
                        handMovements.NetworkingLetGo();
                    }
                    break;
                case InputDeviceIDs.Head:
                    this.transform.parent = null;
                    Rigidbody rigidbody = this.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = false;
                    break;
                default:
                    break;
            }

        }
    }
    void OnGrabbed(IVR_HandMovements hand) {
        handMovements = hand;

		handgun = transform.GetChild(0);

        gunCollider = GetComponentInChildren<Collider>();
        nozzleFlash = GetComponentInChildren<Light>();
        nozzleSmoke = GetComponentInChildren<ParticleSystem>();

        grabbed = true;
    }

    void OnLetGo() {
        grabbed = false;
    }

    void FixedUpdate() {
        if (grabbed) {
            // is gun trigger pulled?
            if (handMovements.indexCurl > 0.5F) { 
                if (!shooting) {
#if INSTANTVR_EDGE
                    gunCollider.attachedRigidbody.isKinematic = false;
#endif
                    gunCollider.attachedRigidbody.AddForceAtPosition(-handgun.forward * 5, transform.TransformPoint(nozzleLocation), ForceMode.Impulse);

                    RaycastHit hit;
                    if (Physics.Raycast(transform.TransformPoint(nozzleLocation), handgun.forward, out hit)) {
                        if (hit.rigidbody != null) {
                            hit.rigidbody.AddForceAtPosition(handgun.forward * 5, hit.point, ForceMode.Impulse);
                        }
                    }
                    nozzleFlash.enabled = true;
                    nozzleSmoke.Play();
                    shooting = true;
                } else {
                    nozzleFlash.enabled = false;
                    Debug.DrawRay(transform.TransformPoint(nozzleLocation), handgun.forward * 5);
                }
            } else
                shooting = false;
        }
    }

}
