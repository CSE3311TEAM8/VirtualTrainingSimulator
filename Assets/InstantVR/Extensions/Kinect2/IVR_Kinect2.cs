/* InstantVR Kinect 2 extension
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 11, 2017
 * 
 * - Included IVR_KINECT define check
 */

 using UnityEngine;
using Windows.Kinect;

namespace IVR {

    [HelpURL("http://passervr.com/documentation/instantvr-extensions/microsoft-kinect-one-2/")]
    public class IVR_Kinect2 : IVR_Extension {
#if (UNITY_STANDALONE_WIN && IVR_KINECT)

        public Windows.Kinect.Body bodyData;
        private bool newFrameAvailable = false;

        private bool tracking = false;
        public bool Tracking { get { return tracking; } set { tracking = value; } }

        private Vector3 referencePosition = Vector3.zero;
        public Vector3 ReferencePosition
        {
            get { return referencePosition; }
            set { referencePosition = value; }
        }
        private Quaternion referenceRotation = Quaternion.identity;
        public Quaternion ReferenceRotation
        {
            get { return referenceRotation; }
            set { referenceRotation = value; }
        }

        private Windows.Kinect.KinectSensor sensor;
        private Windows.Kinect.BodyFrameReader reader;
        private Body[] data = null;
        private int bodyID = -1;

        public override void StartExtension(InstantVR ivr) {
            base.StartExtension(ivr);
            StartKinect();
        }

        private void StartKinect() {
            InstantVR ivr = GetComponent<InstantVR>();
            if (ivr == null)
                return;

            try {
                sensor = KinectSensor.GetDefault();
            }
            catch (System.Exception) {
                Debug.Log("Kinect2 dll is not found. Please install Kinect SDK 2.");
            }
            if (sensor != null) {
                reader = sensor.BodyFrameSource.OpenReader();

                if (!sensor.IsOpen) {
                    sensor.Open();
                }
                present = CheckKinectAvailability();
            }

            IVR_Kinect2Head kinectHead = ivr.headTarget.GetComponent<IVR_Kinect2Head>();
            kinectHead.StartTarget(this, present);

            IVR_Kinect2Hand kinectLeftHand = ivr.leftHandTarget.GetComponent<IVR_Kinect2Hand>();
            kinectLeftHand.StartTarget(this, present, ivr);

            IVR_Kinect2Hand kinectRightHand = ivr.rightHandTarget.GetComponent<IVR_Kinect2Hand>();
            kinectRightHand.StartTarget(this, present, ivr);

            IVR_Kinect2Hip kinectHip = ivr.hipTarget.GetComponent<IVR_Kinect2Hip>();
            kinectHip.StartTarget(this, present);

            IVR_Kinect2Foot kinectLeftFoot = ivr.leftFootTarget.GetComponent<IVR_Kinect2Foot>();
            kinectLeftFoot.StartTarget(this, present, ivr);

            IVR_Kinect2Foot kinectRightFoot = ivr.rightFootTarget.GetComponent<IVR_Kinect2Foot>();
            kinectRightFoot.StartTarget(this, present, ivr);
        }

        private bool CheckKinectAvailability() {
            return true;
        }

        public bool GetNewFrame() {
            bool r = newFrameAvailable;
            if (newFrameAvailable && data != null && bodyID > -1)
                bodyData = data[bodyID];

            return (r && data != null && bodyID > -1);
        }

        public Vector3 GetBonePos(BoneType boneIndex) {
            return
                new Vector3(
                    bodyData.Joints[(int)boneIndex].Position.X,
                    bodyData.Joints[(int)boneIndex].Position.Y,
                    -bodyData.Joints[(int)boneIndex].Position.Z
                );
        }

        public Quaternion GetBoneRot(BoneType boneIndex) {
            return
                new Quaternion(
                    bodyData.JointOrientations[(JointType) boneIndex].Orientation.X,
                    bodyData.JointOrientations[(JointType) boneIndex].Orientation.Y,
                    bodyData.JointOrientations[(JointType) boneIndex].Orientation.Z,
                    bodyData.JointOrientations[(JointType) boneIndex].Orientation.W
                );
        }

        public bool BoneTracking(BoneType boneIndex) {
            return (
                bodyData.Joints[(int) boneIndex].TrackingState == TrackingState.Tracked ||
                bodyData.Joints[(int) boneIndex].TrackingState == TrackingState.Inferred);
        }

        public override void UpdateExtension() {
            base.UpdateExtension();

            newFrameAvailable = false;
            if (reader != null) {
                BodyFrame frame = reader.AcquireLatestFrame();
                if (frame != null) {
                    if (data == null) {
                        data = new Body[sensor.BodyFrameSource.BodyCount];
                    }

                    frame.GetAndRefreshBodyData(data);
                    newFrameAvailable = true;

                    for (int bodyNr = 0; bodyNr < data.Length; bodyNr++) {
                        if (data[bodyNr] == null)
                            continue;

                        if (data[bodyNr].IsTracked) {
                            if (bodyID == -1)
                                bodyID = bodyNr;
                        } else {
                            if (bodyNr == bodyID) {
                                bodyID = -1;
                            }
                        }
                    }

                    frame.Dispose();
                    frame = null;
                }
            }
        }

        void OnApplicationQuit() {
            if (reader != null) {
                reader.Dispose();
                reader = null;
            }

            if (sensor != null) {
                if (sensor.IsOpen) {
                    sensor.Close();
                }

                sensor = null;
            }
        }
#endif
        public enum BoneType : int {
            HipCenter = 0,
            Spine = 1,
            ShoulderCenter = 2,
            Head = 3,
            ShoulderLeft = 4,
            ElbowLeft = 5,
            WristLeft = 6,
            HandLeft = 7,
            ShoulderRight = 8,
            ElbowRight = 9,
            WristRight = 10,
            HandRight = 11,
            HipLeft = 12,
            KneeLeft = 13,
            AnkleLeft = 14,
            FootLeft = 15,
            HipRight = 16,
            KneeRight = 17,
            AnkleRight = 18,
            FootRight = 19,
            Count = 20
        }
    }
}