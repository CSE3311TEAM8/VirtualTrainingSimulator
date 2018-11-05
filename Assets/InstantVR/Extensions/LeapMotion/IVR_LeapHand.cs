/* InstantVR Leap controller
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.6
 * date: August 9, 2017
 *
 * - Added Hybrid finger tracking mode for improved grabbing
 * - Fixed the use of the extenstion's controller reference
 * - Code cleanup
 */

using System.Collections.Generic;
using UnityEngine;

namespace IVR {

    public class IVR_LeapHand : IVR_HandController {

#if UNITY_STANDALONE_WIN && IVR_LEAP
        public enum LeapMode {
            HandTracking,
            FingerTracking,
            HybridTracking
        }

        [HideInInspector]
        private IVR_Leap ivrLeap;
        [HideInInspector]
        public Leap.Hand leapHand;

        private long lastTimeStamp = 0;

        [HideInInspector]
        private IVR_HandMovements handMovements;

        public override void StartController(InstantVR ivr) {
            ivrLeap = ivr.GetComponent<IVR_Leap>();
            extension = ivrLeap;

            base.StartController(ivr);

            extrapolation = false;

            if (ivrLeap != null)
                isHeadMounted = ivrLeap.IsHeadMounted;

            handMovements = GetComponent<IVR_HandMovements>();
        }

        [HideInInspector]
        public bool isHeadMounted;

        public override void UpdateController() {
            if (leapHand == null) {
                tracking = false;
                return;
            } 
            Leap.Frame frame;
            frame = ivrLeap.controller.Frame();
            if (frame.Timestamp <= lastTimeStamp)
                return;

            lastTimeStamp = frame.Timestamp;

            controllerPosition = ToVector3(leapHand.WristPosition) / 1000;

            Vector3 palmNormal = ToVector3(leapHand.PalmNormal);
            Vector3 handDirection = ToVector3(leapHand.Direction);
            controllerRotation = Quaternion.LookRotation(handDirection, -palmNormal);
            base.UpdateController();

            tracking = true;
            if (!selected)
                return;

            HybridTracking(palmNormal);
        }

        private void FingerTracking(Vector3 palmNormal) {
            List<Leap.Finger> fingers = leapHand.Fingers;
            for (int i = 0; i < fingers.Count; i++) {
                Leap.Finger finger = fingers[i];
                Leap.Bone proximal = finger.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL);
                Leap.Bone distal = finger.Bone(Leap.Bone.BoneType.TYPE_DISTAL);

                float angle = proximal.Direction.AngleTo(distal.Direction);
                Vector3 axis = ToVector3(proximal.Direction.Cross(distal.Direction));
                float dot = proximal.Direction.Dot(distal.Direction);
                float value = Mathf.Clamp01((1.0f - dot) - 0.5f);
                switch (finger.Type) {
                    case Leap.Finger.FingerType.TYPE_THUMB:
                        if ((transform == ivr.leftHandTarget && Vector3.Angle(axis, palmNormal) < 90) ||
                            (transform == ivr.rightHandTarget && Vector3.Angle(axis, palmNormal) > 90))
                            angle = -angle * 2F;
                        handMovements.thumbCurl = angle * Mathf.Rad2Deg / 60;
                        break;
                    case Leap.Finger.FingerType.TYPE_INDEX:
                        handMovements.indexCurl = value;
                        break;
                    case Leap.Finger.FingerType.TYPE_MIDDLE:
                        handMovements.middleCurl = value;
                        break;
                    case Leap.Finger.FingerType.TYPE_RING:
                        handMovements.ringCurl = value;
                        break;
                    case Leap.Finger.FingerType.TYPE_PINKY:
                        handMovements.littleCurl = value;
                        break;
                }
            }
        }

        private void HandTracking() {
            handMovements.thumbCurl = leapHand.GrabStrength;
            handMovements.indexCurl = leapHand.GrabStrength;
            handMovements.middleCurl = leapHand.GrabStrength;
            handMovements.ringCurl = leapHand.GrabStrength;
            handMovements.littleCurl = leapHand.GrabStrength;
        }

        private void HybridTracking(Vector3 palmNormal) {
            if (leapHand.GrabStrength > 0.6F) {
                // use grabstrength to improve holding objects
                HandTracking();
            } else {
                FingerTracking(palmNormal);
            }
            //Debug.Log(handMovements.indexCurl);
        }

        private static Vector3 ToVector3(Leap.Vector vector) {
            return new Vector3(vector.x, vector.y, -vector.z);
        }
#endif
    }
}