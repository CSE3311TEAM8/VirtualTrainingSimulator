/* InstantVR Head Movements Edge
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.3.1
 * date: February 13, 2016
 *
 */

using System.Collections;
using UnityEngine;

namespace IVR {

    public class HeadMovementsEdge : HeadMovements {
        public bool focusSimulation;

        public Transform headBone;

        public EyeMovements leftEyeMovements, rightEyeMovements;
        public SkinnedMeshRenderer smRenderer;

        public bool lookingAtCharacter = false;

        public float lastBlink;

        public override void StartMovements(InstantVR ivr) {
            base.StartMovements(ivr);

            Animator animator = ivr.characterTransform.GetComponent<Animator>();
            headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform leftEyeBone = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            Transform rightEyeBone = animator.GetBoneTransform(HumanBodyBones.RightEye);

            leftEyeMovements.Init(headBone, leftEyeBone, smRenderer, this);
            rightEyeMovements.Init(headBone, rightEyeBone, smRenderer, this);

            StartCoroutine(ScanForCharacters());
        }

        public override void UpdateMovements() {
            leftEyeMovements.LookAt(focusPoint);
            rightEyeMovements.LookAt(focusPoint);

            if (leftEyeMovements != null)
                leftEyeMovements.Update();
            if (rightEyeMovements != null)
                rightEyeMovements.Update();

            base.UpdateMovements();
        }

        public void UpdateFocusPoint(bool raycastHit, RaycastHit hit) {
            float tSinceLastFocus = Time.realtimeSinceStartup - lastFocus;

            if (raycastHit) {
                InstantVR character = CharacterInSightline(headcam.position, hit.point);
                if (character != null) {
                    FocusOnCharacter(character);
                    lookingAtObject = character.gameObject;
                    lookingAtCharacter = true;
                    lastFocus = Time.realtimeSinceStartup;
                } else {
                    lookingAtCharacter = false;

                    if (hit.rigidbody != null) {
                        focusPoint = hit.point;
                        lookingAtObject = hit.transform.gameObject;
                        lastFocus = Time.realtimeSinceStartup;
                    }

                    Vector3 focusDirection = focusPoint - headcam.position;
                    float angle = Vector3.Angle(headcam.forward, focusDirection);
                    if (tSinceLastFocus > 2 || angle > 10) {
                        focusPoint = hit.point;
                        lookingAtObject = null;
                        lastFocus = Time.realtimeSinceStartup;
                    }
                }
            } else if (tSinceLastFocus > 1) {
                focusPoint = headcam.position + headcam.forward * 10;
                lookingAtObject = null;
            }
        }

        public Vector3 DeriveLookDirection(Quaternion headRotation) {
            float angle = Quaternion.Angle(Quaternion.identity, headRotation);
            Quaternion lookRotation = Quaternion.LerpUnclamped(Quaternion.identity, headRotation, 1 + angle / 120);
            return lookRotation * ivr.characterTransform.forward;
        }


        private InstantVR CharacterInSightline(Vector3 rayStart, Vector3 rayEnd) {
            float smallestDistance = int.MaxValue;
            InstantVR closestCharacter = null;
            for (int i = 0; i < allCharacters.Length; i++) {
                if (allCharacters[i] != ivr) {
                    Vector3 headPosition = allCharacters[i].headTarget.position;
                    float distanceToHead = (headPosition - rayStart).magnitude;
                    float distanceToRayHit = Vector3.Cross(rayEnd - rayStart, headPosition - rayStart).magnitude;
                    if (distanceToRayHit < 0.5f && distanceToHead < smallestDistance) {
                        closestCharacter = allCharacters[i];
                        smallestDistance = distanceToHead;
                    }
                }
            }
            return closestCharacter;
        }

        private void FocusOnCharacter(InstantVR character) {
            Animator animator = character.GetComponentInChildren<Animator>();
            Transform leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            focusPoint = leftEye.position; // to start with
        }

        private InstantVR[] allCharacters;

        private IEnumerator ScanForCharacters() {
            while (true) {
                allCharacters = FindObjectsOfType<InstantVR>();
                yield return new WaitForSeconds(1);
            }
        }

    }
}