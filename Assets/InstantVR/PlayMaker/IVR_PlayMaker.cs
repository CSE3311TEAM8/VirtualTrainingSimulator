/* InstantVR PlayMaker support
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.4.1
 * date: March 19, 2016
 * 
 * - Moved game controller scripts to IVR_Input
 */

#if PLAYMAKER
using UnityEngine;
using IVR;

namespace HutongGames.PlayMaker.Actions {
    public enum BodySide { Left, Right };

    [ActionCategory("InstantVR")]
    [Tooltip("Avatar walking movement")]
    public class CharacterMove : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(InstantVR))]
        [Tooltip("The instantVR gameObject to move.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The movement vector. Always local space and per second.")]
        public FsmVector3 moveVector;

        private GameObject previousGo; // remember so we can get new controller only when it changes.
        private InstantVR ivrCharacter;

        public override void Reset() {
            gameObject = null;
            moveVector = new FsmVector3 { UseVariable = true };
        }

        public override void OnUpdate() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            if (go != previousGo) {
                ivrCharacter = go.GetComponent<InstantVR>();
                previousGo = go;
            }

            if (ivrCharacter != null) {// && ivrWalking != null) {
                ivrCharacter.Move(moveVector.Value);
            }
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Avatar rotation")]
    public class CharacterRotate : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(InstantVR))]
        [Tooltip("The instantVR gameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The rotation angle. Always over up(=Y) axis and per second.")]
        public FsmFloat angle;

        private GameObject previousGo; // remember so we can get new controller only when it changes.
        private InstantVR ivrCharacter;

        public override void Reset() {
            gameObject = null;
            angle = 0;
        }

        public override void OnUpdate() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            if (go != previousGo) {
                ivrCharacter = go.GetComponent<InstantVR>();
                previousGo = go;
            }

            if (ivrCharacter != null) {
                ivrCharacter.Rotate(angle.Value);
            }
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Sends an Event when the avatar collides with the environment.")]
    public class CharacterCollision : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(InstantVR))]
        [Tooltip("The InstantVR gameObject of the avatar who may collide.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event to send when the avatar collides.")]
        public FsmEvent collisionStartEvent;

        [Tooltip("Event to send when the avatar no longer collides.")]
        public FsmEvent collisionEndEvent;

        [Tooltip("Set to True when the avatar collides.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset() {
            base.Reset();
            gameObject = null;
            collisionStartEvent = null;
            collisionEndEvent = null;
            storeResult = null;
        }

        private GameObject previousGo; // remember so we can get new controller only when it changes.
        private InstantVR ivrCharacter;
        public override void OnUpdate() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            if (go != previousGo) {
                ivrCharacter = go.GetComponent<InstantVR>();
                previousGo = go;
            }

            if (ivrCharacter.collided && !storeResult.Value)
                Fsm.Event(collisionStartEvent);
            else if (!ivrCharacter.collided && storeResult.Value)
                Fsm.Event(collisionEndEvent);

            storeResult.Value = ivrCharacter.collided;
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Get limb information")]
    public class GetBoneInformation : FsmStateAction {

        [RequiredField]
        [CheckForComponent(typeof(InstantVR))]
        [Tooltip("The instantVR character.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Which body bone?")]
        public HumanBodyBones bone;

        [UIHint(UIHint.Variable)]
        [Tooltip("The bone position")]
        public FsmVector3 position;
        [UIHint(UIHint.Variable)]
        [Tooltip("The bone rotation")]
        public FsmQuaternion rotation;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        private GameObject previousGo; // remember so we can get new controller only when it changes.
        private InstantVR ivrCharacter;
        private Animator animator;
        private Transform hand;

        public override void Reset() {
            base.Reset();
            bone = HumanBodyBones.Head;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            everyFrame = true;

            hand = null;
        }

        public override void OnEnter() {
            DoGetBoneInfo();

            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoGetBoneInfo();
        }

        void DoGetBoneInfo() {
            if (hand == null)
                hand = GetBone();

            if (hand != null) {
                position.Value = hand.position;
                rotation.Value = hand.rotation;
            }
        }

        private Transform GetBone() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
                return null;

            if (go != previousGo) {
                ivrCharacter = go.GetComponent<InstantVR>();
                if (ivrCharacter != null) {
                    animator = ivrCharacter.characterTransform.GetComponent<Animator>();
                }
                previousGo = go;
            }

            if (animator != null) {
                return animator.GetBoneTransform(bone);
            }
            return null;
        }
    }

    [ActionCategory("InstantVR")]
    [Tooltip("Get eye focus point")]
    public class GetFocusPoint : FsmStateAction {
        [RequiredField]
        [CheckForComponent(typeof(InstantVR))]
        [Tooltip("The InstantVR gameObject which is looking around")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("The focus point")]
        public FsmVector3 focusPoint;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        private GameObject previousGo; // remember so we can get new controller only when it changes.
        private InstantVR ivrCharacter;
        private IVR.HeadMovements headMovements;

        public override void Reset() {
            base.Reset();
            gameObject = null;
            focusPoint = Vector3.zero;
            everyFrame = true;
        }

        public override void OnEnter() {
            DoGetFocusPoint();

            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate() {
            DoGetFocusPoint();
        }

        private void DoGetFocusPoint() {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            if (go != previousGo) {
                ivrCharacter = go.GetComponent<InstantVR>();
                previousGo = go;
            }
            if (headMovements == null)
                headMovements = ivrCharacter.headTarget.GetComponent<IVR.HeadMovements>();

            if (headMovements != null)
                focusPoint.Value = headMovements.focusPoint;
        }
    }

}

#endif