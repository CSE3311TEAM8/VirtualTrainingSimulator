/* Eye Movements
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.0.0
 * date: December 31, 2014
 * 
 * - First version
 */

using UnityEngine;

namespace IVR {

    [System.Serializable]
    public class EyeMovements {
        private Transform headTransform;
        private Transform target;
        private HeadMovementsEdge headMovements;

        public  Transform eye;

        private SkinnedMeshRenderer smRenderer;

        public Transform upperLid, lowerLid;

        public Quaternion fromNormEye;
        private Quaternion fromNormUpperLid, fromNormLowerLid;

        // Blendshapes         // FACS
        public int blink;      // AU45
        public void Init(Transform headBone, Transform eyeBone, SkinnedMeshRenderer _smRenderer, HeadMovementsEdge _headMovements) {

            eye = eyeBone;
            smRenderer = _smRenderer;
            headMovements = _headMovements;
            headTransform = headMovements.transform;

            Quaternion forwardRotation = Quaternion.LookRotation(headTransform.forward);
            fromNormEye = Quaternion.Inverse(forwardRotation) * eye.rotation;

            if (upperLid != null)
                fromNormUpperLid = Quaternion.Inverse(forwardRotation) * upperLid.rotation;
            if (lowerLid != null)
                fromNormLowerLid = Quaternion.Inverse(forwardRotation) * lowerLid.rotation;

            eye.rotation = forwardRotation;

            GameObject obj = new GameObject();
            obj.hideFlags = HideFlags.HideInHierarchy;
            target = obj.transform;
        }

        public void LookAt(Vector3 position) {
            if (eye != null && target != null)
                target.position = position;
        }

        public void Update() {
            if (eye != null && target != null) {
                LookAtTarget(target.position);
                Vector3 eyeForward = eye.forward;

                eye.rotation *= fromNormEye;
                //Debug.DrawRay(eye.position, eyeForward * 10);

                if (upperLid != null) {
                    upperLid.LookAt(target.position);
                    upperLid.rotation *= fromNormUpperLid;
                }

                if (lowerLid != null) {
                    lowerLid.LookAt(target.position);
                    lowerLid.rotation *= fromNormLowerLid;
                }

                Blinking(eyeForward);
            }
        }

        const float maxEyeAngle = 30;
        private void LookAtTarget(Vector3 targetPosition) {
            Vector3 targetDirection = target.position - eye.position;
            targetDirection = Vector3.RotateTowards(headMovements.headBone.forward, targetDirection, maxEyeAngle * Mathf.Deg2Rad, 0);
            eye.LookAt(eye.position + targetDirection);
        }

        private float eyeClosingSpeed = 0.300f; // duration of a blink in seconds

        private void Blinking(Vector3 eyeForward) {
            if (smRenderer != null && blink < smRenderer.sharedMesh.blendShapeCount) {
                float blinkPhase = (Time.realtimeSinceStartup - headMovements.lastBlink) / eyeClosingSpeed;
                if (blinkPhase < 1) {
                    if (blinkPhase < 0.5f) {
                        smRenderer.SetBlendShapeWeight(blink, blinkPhase * 200);
                    } else {
                        smRenderer.SetBlendShapeWeight(blink, 100 - (blinkPhase * 200));
                    }
                } else {
                    smRenderer.SetBlendShapeWeight(blink, 0);
                }

                float angle = Vector3.Angle(eyeForward, headTransform.forward);
                float tSinceLastBlink = Time.realtimeSinceStartup - headMovements.lastBlink;
                if (angle > 10 && tSinceLastBlink > 2)
                    Blink(0.400f);
                if (tSinceLastBlink > 6 + Random.value)
                    Blink(0.300f);
            }
        }

        private void Blink(float blinkSpeed) {
            headMovements.lastBlink = Time.realtimeSinceStartup;
            eyeClosingSpeed = blinkSpeed;
        }
    }
}    
