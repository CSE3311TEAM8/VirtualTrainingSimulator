/* InstantVR PlayMake Hand Actions
 * author: Pascal Serrarnes
 * email: support@passervr.com
 * version: 3.3.0
 * date: Februari 5, 2016
 * 
 */

 using UnityEngine;

#if PLAYMAKER
using IVR;

namespace HutongGames.PlayMaker.Actions {
    [ActionCategory("InstantVR")]
    [Tooltip("Gets the hand pose")]
    public class GetHandPose : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(IVR_HandMovements))]
        public FsmOwnerDefault handObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the thumb is bent")]
        public FsmFloat thumbCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the index finger is bent")]
        public FsmFloat indexCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the middle finger is bent")]
        public FsmFloat middleCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the ring finger is bent")]
        public FsmFloat ringCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the little finger is bent")]
        public FsmFloat littleCurl;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        private IVR_HandMovements handMovements;

        public override void Reset() {
            thumbCurl = null;
            indexCurl = null;
            middleCurl = null;
            ringCurl = null;
            littleCurl = null;

            everyFrame = true;

            handMovements = null;
        }

        public override void OnEnter() {
            DoGetHandPose();

            if (!everyFrame) {
                Finish();
            }
        }

        public override void OnUpdate() {
            DoGetHandPose();
        }

        void DoGetHandPose() {
            if (handMovements == null)
                handMovements = FsmHandUtils.GetHandMovements(Fsm, handObject);

            if (handMovements != null) {
                thumbCurl.Value = handMovements.thumbCurl;
                indexCurl.Value = handMovements.indexCurl;
                middleCurl.Value = handMovements.middleCurl;
                ringCurl.Value = handMovements.ringCurl;
                littleCurl.Value = handMovements.littleCurl;                
            }
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Sets the hand pose")]
    public class SetHandPose : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(IVR_HandMovements))]
        public FsmOwnerDefault handObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the thumb is bent")]
        public FsmFloat thumbCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the index finger is bent")]
        public FsmFloat indexCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the middle finger is bent")]
        public FsmFloat middleCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the ring finger is bent")]
        public FsmFloat ringCurl;
        [UIHint(UIHint.Variable)]
        [Tooltip("The amount the little finger is bent")]
        public FsmFloat littleCurl;

        private IVR_HandMovements handMovements;

        public override void Reset() {
            thumbCurl = null;
            indexCurl = null;
            middleCurl = null;
            ringCurl = null;
            littleCurl = null;

            handMovements = null;
        }

        public override void OnUpdate() {
            if (handMovements == null)
                handMovements = FsmHandUtils.GetHandMovements(Fsm, handObject);

            if (handMovements != null) {
                handMovements.thumbCurl = thumbCurl.Value;
                handMovements.indexCurl = indexCurl.Value;
                handMovements.middleCurl = middleCurl.Value;
                handMovements.ringCurl = ringCurl.Value;
                handMovements.littleCurl = littleCurl.Value;
            }
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Gets object grabbed by the hand (if any)")]
    public class HandGrabbing : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(IVR_HandMovements))]
        public FsmOwnerDefault handObject;

        [Tooltip("Event to send when the hand grabs an object.")]
        public FsmEvent grabbedEvent;

        [Tooltip("Event to send when the hand lets go of an object.")]
        public FsmEvent letGoEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("The object grabbed by the hand (if any)")]
        public FsmGameObject grabbedObject;

        private IVR_HandMovements handMovements;

        public override void Reset() {
            handObject = null;
            handMovements = null;
        }

        public override void OnUpdate() {
            DoGetGrabbedObject();
        }
        void DoGetGrabbedObject() {
            if (handMovements == null)
                handMovements = FsmHandUtils.GetHandMovements(this.Fsm, handObject);

            if (handMovements != null) {
                if (grabbedObject.Value == null && handMovements.grabbedObject != null)
                    Fsm.Event(grabbedEvent);
                else if (grabbedObject.Value != null && handMovements.grabbedObject == null)
                    Fsm.Event(letGoEvent);

                grabbedObject.Value = handMovements.grabbedObject;
            }
        }
    }

    public class FsmHandUtils {
        public static IVR_HandMovements GetHandMovements(Fsm fsm, FsmOwnerDefault handObject) {
            GameObject obj = fsm.GetOwnerDefaultTarget(handObject);
            if (obj != null) {
                return obj.GetComponent<IVR_HandMovements>();
            }
            return null;
        }
    }
}
#endif