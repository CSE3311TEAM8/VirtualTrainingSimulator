/* InstantVR Leap extension
 * Copyright (c) 2017 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.6
 * date: August 9, 2017
 * 
 * - Added OnControllerConnect for late connections
 */

using System.Collections.Generic;
using UnityEngine;

namespace IVR {

    [HelpURL("http://passervr.com/documentation/instantvr-extensions/leap-motion/")]
    public class IVR_Leap : IVR_Extension {
#if UNITY_STANDALONE_WIN && IVR_LEAP
        public bool IsHeadMounted = true;

        private Leap.Controller _controller;
        public Leap.Controller controller { get { return _controller;  } }

        [HideInInspector]
        private Transform headcam;

        [HideInInspector]
        private IVR_LeapHand leapLeftHand, leapRightHand;

        public override void StartExtension(InstantVR ivr) {
            base.StartExtension(ivr);
            StopLeap();

            headcam = ivr.GetComponentInChildren<Camera>().transform;

            _controller = new Leap.Controller();
            if (_controller.IsConnected)
                InitializeFlags();
            else
                _controller.Device += OnControllerConnect;

            leapLeftHand = ivr.leftHandTarget.GetComponent<IVR_LeapHand>();
            leapRightHand = ivr.rightHandTarget.GetComponent<IVR_LeapHand>();
        }

        private void OnControllerConnect(object sender, Leap.LeapEventArgs args) {
            InitializeFlags();
            _controller.Device -= OnControllerConnect;
        }

        private void InitializeFlags() {
            if (IsHeadMounted)
                _controller.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
            else
                _controller.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
        }

        public override void UpdateExtension() {
            base.UpdateExtension();

            if (_controller == null)
                return;

            Leap.Frame frame = _controller.Frame();
            List<Leap.Hand> hands = frame.Hands;

            leapLeftHand.leapHand = null;
            leapRightHand.leapHand = null;

            for (int i = 0; i < hands.Count; i++) {
                if (hands[i].IsLeft) {
                    leapLeftHand.leapHand = hands[i];
                    break;
                }
            }

            for (int i = 0; i < hands.Count; i++) {
                if (hands[i].IsRight) {
                    leapRightHand.leapHand = hands[i];
                    break;
                }
            }

            if (IsHeadMounted) {
                Quaternion headcamRotation = Quaternion.Inverse(ivr.transform.rotation) * headcam.rotation;
                Vector3 headcamPosition = headcam.position - ivr.transform.position;
                trackerPosition = Quaternion.Inverse(ivr.transform.rotation) * (headcamPosition + headcamRotation * new Vector3(0, 0, 0.09F));
                trackerEulerAngles = (headcamRotation * Quaternion.Euler(270, 0, 180)).eulerAngles;
            }

        }

        void OnApplicationQuit() {
            StopLeap();
        }

        void OnDestroy() {
            StopLeap();
        }

        private void StopLeap() {
            if (_controller == null)
                return;

            if (_controller.IsConnected)
                _controller.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);

            _controller.StopConnection();
            _controller = null;
        }
#endif
    }
}