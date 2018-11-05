/* InstantVR Network Avatar
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.11
 *
 * - fixes in grabbing
 */

#if !IVR_PHOTON

using UnityEngine;
using UnityEngine.Networking;

namespace IVR {
    [RequireComponent(typeof(NetworkIdentity))]
    public class IVR_UnetAvatar : NetworkBehaviour {

        public GameObject avatarFP;
        public GameObject avatarTP;
        private bool isFirstPerson;

        protected InstantVR instantiatedAvatar = null;

        protected IVR_HandMovements leftHandMovements, rightHandMovements;

        protected NetworkManager nwManager;
        protected NetworkIdentity identity;


        private enum Modes {
            Uninitialized,
            Player,
            Spectator
        }
        [SyncVar]
        private Modes mode = Modes.Uninitialized;

        public override void OnStartServer() {
            RegisterNetworkingHandlers();

            OnStartClient();
        }

        public override void OnStartClient() {
            nwManager = FindObjectOfType<NetworkManager>();
            identity = GetComponent<NetworkIdentity>();
            if (!identity.localPlayerAuthority)
                Debug.LogWarning("Network Avatar Identity need to have Local Player Authority = true");

            switch (mode) {
                case Modes.Player:
                    InstantiateThirdPerson(this.transform, avatarTP);
                    break;
                default:
                    break;
            }
        }

        public bool spectatorMode;
        public override void OnStartLocalPlayer() {
            identity = GetComponent<NetworkIdentity>();

            if (identity.hasAuthority) {
                if (spectatorMode)
                    mode = Modes.Spectator;
                else
                    mode = Modes.Player;

                NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();

                switch (mode) {
                    case Modes.Player:
                        if (startPositions.Length > 0) {
                            transform.position = startPositions[0].transform.position;
                            transform.rotation = startPositions[0].transform.rotation;
                        }
                        InstantiateFirstPerson(avatarFP);
                        CmdInstantiateThirdPersonOnClients(mode);
                        break;
                    case Modes.Spectator:
                        GameObject cameraObject = new GameObject("Spectator");
                        cameraObject.AddComponent<Camera>();
                        break;
                    default:
                        break;
                }
            }
        }

        public int sendRate = 25;
        private float lastSend;
        //public void UpdateNetworking() {
        //    if (identity == null || !identity.isLocalPlayer)
        //        return;

        //    if (Time.time > lastSend + 1 / sendRate) {
        //        Debug.Log("send");
        //        UnetAvatar.WriteAvatarPose2Server(identity, this, Time.time);
        //        lastSend = Time.time;
        //    }
        //}

        public void FixedUpdate() {
            if (identity != null) {
                if (identity.isLocalPlayer) {
                    if (instantiatedAvatar != null) {
                        if (Time.time > lastSend + 1 / sendRate) {
                            SyncClient2Server(identity, instantiatedAvatar, leftHandMovements, rightHandMovements);
                            lastSend = Time.time;
                        }
                    }
                }
            }
        }

        private void SyncClient2Server(NetworkIdentity identity, InstantVR instantiatedAvatar, IVR_HandMovements leftHandMovements, IVR_HandMovements rightHandMovements) {
            short msgType = MsgType.Highest + 1;

            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(msgType);
            {
                writer.Write(identity.netId);
                WriteAvatarPose(writer, instantiatedAvatar, leftHandMovements, rightHandMovements);
            }
            writer.FinishMessage();

            identity.connectionToServer.SendWriter(writer, Channels.DefaultReliable);
        }

        #region Server

        [Server]
        protected void RegisterNetworkingHandlers() {
            short msgType = MsgType.Highest + 1;

            NetworkServer.RegisterHandler(msgType, OnAvatarPose);

        }

        [ServerCallback]
        public void OnAvatarPose(NetworkMessage msg) {
            NetworkReader reader = msg.reader;

            NetworkInstanceId netId = reader.ReadNetworkId();
            GameObject obj = NetworkServer.FindLocalObject(netId);
            InstantVR ivrFP = obj.GetComponentInChildren<InstantVR>();
            if (ivrFP != null) {
                IVR_HandMovements leftHandMovements = ivrFP.leftHandTarget.GetComponent<IVR_HandMovements>();
                IVR_HandMovements rightHandMovements = ivrFP.rightHandTarget.GetComponent<IVR_HandMovements>();

                byte targetMask = ReadAvatarPose(reader, ivrFP, leftHandMovements, rightHandMovements);

                short msgType = MsgType.Highest + 2;
                NetworkWriter writer = new NetworkWriter();
                writer.StartMessage(msgType);
                {
                    writer.Write(netId);
                    WriteAvatarPose(targetMask, writer, ivrFP, leftHandMovements, rightHandMovements);
                }
                writer.FinishMessage();

                NetworkServer.SendWriterToReady(null, writer, Channels.DefaultUnreliable);
            }
        }

        private const byte HeadTargetBit = 1 << 1;
        private const byte LeftHandTargetBit = 1 << 2;
        private const byte RightHandTargetBit = 1 << 3;
        private const byte HipsTargetBit = 1 << 4;
        private const byte LeftFootTargetBit = 1 << 5;
        private const byte RightFootTargetBit = 1 << 6;
        public static void WriteAvatarPose(NetworkWriter writer, InstantVR ivr, IVR_HandMovements leftHandMovements, IVR_HandMovements rightHandMovements) {
            byte targetMask = DetermineActiveTargets(ivr);
            WriteAvatarPose(targetMask, writer, ivr, leftHandMovements, rightHandMovements);
        }
        public static void WriteAvatarPose(byte targetMask, NetworkWriter writer, InstantVR ivr, IVR_HandMovements leftHandMovements, IVR_HandMovements rightHandMovements) {
            writer.Write(targetMask);

            // InstantVR Transform is always sent
            SendTarget(writer, ivr.transform);
            if ((targetMask & HeadTargetBit) != 0)
                SendTarget(writer, ivr.headTarget);
            if ((targetMask & LeftHandTargetBit) != 0)
                SendTarget(writer, ivr.leftHandTarget);
            if ((targetMask & RightHandTargetBit) != 0)
                SendTarget(writer, ivr.rightHandTarget);
            if ((targetMask & HipsTargetBit) != 0)
                SendTarget(writer, ivr.hipTarget);
            if ((targetMask & LeftFootTargetBit) != 0)
                SendTarget(writer, ivr.leftFootTarget);
            if ((targetMask & RightFootTargetBit) != 0)
                SendTarget(writer, ivr.rightFootTarget);

            if ((targetMask & LeftHandTargetBit) != 0)
                WriteAvatarHandPose(writer, leftHandMovements);
            if ((targetMask & RightHandTargetBit) != 0)
                WriteAvatarHandPose(writer, rightHandMovements);
        }

        private static void WriteAvatarHandPose(NetworkWriter writer, IVR_HandMovements handMovements) {
            if (handMovements != null) {
                writer.Write(true);

                writer.Write(handMovements.thumbCurl);
                writer.Write(handMovements.indexCurl);
                writer.Write(handMovements.middleCurl);
                writer.Write(handMovements.ringCurl);
                writer.Write(handMovements.littleCurl);
            } else {
                writer.Write(false);
            }
        }

        private static byte DetermineActiveTargets(InstantVR ivr) {
            byte targetMask = 0;

            if (ivr.HeadController != null && ivr.HeadController.GetType() != typeof(IVR_AnimatorHead))
                targetMask |= HeadTargetBit;
            if (ivr.leftHandMovements.selectedController != null
                && ivr.leftHandMovements.selectedController.GetType() != typeof(IVR_AnimatorHand)
                && ivr.leftHandMovements.selectedController.GetType() != typeof(IVR_TraditionalHand)
                )
                targetMask |= LeftHandTargetBit;
            if (ivr.rightHandMovements.selectedController != null
                && ivr.rightHandMovements.selectedController.GetType() != typeof(IVR_AnimatorHand)
                && ivr.rightHandMovements.selectedController.GetType() != typeof(IVR_TraditionalHand)
                )
                targetMask |= RightHandTargetBit;
            if (ivr.HipController != null && ivr.HipController.GetType() != typeof(IVR_AnimatorHip))
                targetMask |= HipsTargetBit;
            if (ivr.LeftFootController != null && ivr.LeftFootController.GetType() != typeof(IVR_AnimatorFoot))
                targetMask |= LeftFootTargetBit;
            if (ivr.RightFootController != null && ivr.RightFootController.GetType() != typeof(IVR_AnimatorFoot))
                targetMask |= RightFootTargetBit;

            return targetMask;
        }

        private static void SendTarget(NetworkWriter writer, Transform targetTransform) {
            writer.Write(targetTransform.position);
            writer.Write(targetTransform.rotation);
        }

        #endregion

        #region Client First Person
        [Client]
        protected void RegisterClientHandlers(InstantVR ivr, NetworkManager nwManager) {
            short msgType = MsgType.Highest + 2;

            nwManager.client.RegisterHandler(msgType, msg => OnAvatarTPPose(msg, instantiatedAvatar));
        }

        protected virtual void InstantiateFirstPerson(GameObject avatar) {
            GameObject instantiatedObject = (GameObject)Instantiate(avatar, transform.position, transform.rotation);
            instantiatedAvatar = instantiatedObject.GetComponent<InstantVR>();

            instantiatedObject.transform.parent = this.transform;

            this.gameObject.name = instantiatedObject.name;

            if (instantiatedAvatar != null) {
                leftHandMovements = instantiatedAvatar.leftHandTarget.GetComponent<IVR_HandMovements>();
                rightHandMovements = instantiatedAvatar.rightHandTarget.GetComponent<IVR_HandMovements>();

                if (identity.isLocalPlayer)
                    RegisterClientHandlers(instantiatedAvatar, nwManager);
            }
        }

        #endregion

        #region Client Third Person
        [Command]
        private void CmdInstantiateThirdPersonOnClients(Modes mode) {
            RpcInstantiateThirdPerson(mode);
        }

        [ClientRpc]
        private void RpcInstantiateThirdPerson(Modes mode) {
            NetworkIdentity identity = GetComponent<NetworkIdentity>();
            if (identity == null)
                identity = avatarTP.AddComponent<NetworkIdentity>();

            if (!identity.hasAuthority) {
                switch (mode) {
                    case Modes.Player:
                        InstantiateThirdPerson(this.transform, avatarTP);
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtual void InstantiateThirdPerson(Transform parent, GameObject avatar) {
            if (avatar != null) {
                GameObject instantiatedObject = (GameObject)Instantiate(avatar, this.transform.position, this.transform.rotation);

                instantiatedAvatar = instantiatedObject.GetComponent<InstantVR>();

                instantiatedObject.transform.parent = parent;

                parent.name = instantiatedObject.name;

                if (instantiatedAvatar != null) {
                    //leftHandMovements = instantiatedAvatar.leftHandTarget.GetComponent<IVR_HandMovements>();
                    //rightHandMovements = instantiatedAvatar.rightHandTarget.GetComponent<IVR_HandMovements>();

                    if (!identity.isLocalPlayer)
                        RegisterClientHandlers(instantiatedAvatar, nwManager);
                }
            }
        }

        [ClientCallback]
        public void OnAvatarTPPose(NetworkMessage msg, InstantVR tpAvatar) {
            NetworkReader reader = msg.reader;

            NetworkInstanceId netId = reader.ReadNetworkId();

            GameObject obj = ClientScene.FindLocalObject(netId);
            NetworkIdentity id = obj.GetComponent<NetworkIdentity>();
            if (id.hasAuthority == false) {
                InstantVR ivrTP = obj.GetComponentInChildren<InstantVR>();
                IVR_HandMovements leftHandMovements = ivrTP.leftHandTarget.GetComponent<IVR_HandMovements>();
                IVR_HandMovements rightHandMovements = ivrTP.rightHandTarget.GetComponent<IVR_HandMovements>();
                ReadAvatarPose(reader, ivrTP, leftHandMovements, rightHandMovements);
            }
        }
        #endregion

        public static byte ReadAvatarPose(NetworkReader reader, InstantVR ivr, IVR_HandMovements leftHandMovements, IVR_HandMovements rightHandMovements) {
            byte targetMask = reader.ReadByte();

            // Humanoid Transform is always received
            ReceiveTarget(reader, ivr.transform);

            if ((targetMask & HeadTargetBit) != 0) {
                EnableHeadAnimator(ivr.headTarget, false);
                ReceiveTarget(reader, ivr.headTarget);
            } else
                EnableHeadAnimator(ivr.headTarget, true);

            if ((targetMask & LeftHandTargetBit) != 0) {
                EnableHandAnimator(ivr.leftHandTarget, false);
                ReceiveTarget(reader, ivr.leftHandTarget);
            } else
                EnableHandAnimator(ivr.leftHandTarget, true);

            if ((targetMask & RightHandTargetBit) != 0) {
                EnableHandAnimator(ivr.rightHandTarget, false);
                ReceiveTarget(reader, ivr.rightHandTarget);
            } else
                EnableHandAnimator(ivr.rightHandTarget, true);

            if ((targetMask & HipsTargetBit) != 0) {
                EnableHipsAnimator(ivr.hipTarget, false);
                ReceiveTarget(reader, ivr.hipTarget);
            } else
                EnableHipsAnimator(ivr.hipTarget, true);

            if ((targetMask & LeftFootTargetBit) != 0) {
                EnableFootAnimator(ivr.leftFootTarget, false);
                ReceiveTarget(reader, ivr.leftFootTarget);
            } else
                EnableFootAnimator(ivr.leftFootTarget, true);

            if ((targetMask & RightFootTargetBit) != 0) {
                EnableFootAnimator(ivr.rightFootTarget, false);
                ReceiveTarget(reader, ivr.rightFootTarget);
            } else
                EnableFootAnimator(ivr.rightFootTarget, true);

            if ((targetMask & LeftHandTargetBit) != 0) {
                bool leftHandIncluded = (bool)reader.ReadBoolean();
                if (leftHandIncluded) {
                    ReadAvatarHandPose(reader, leftHandMovements);
                }
            }

            if ((targetMask & RightHandTargetBit) != 0) {
                bool rightHandIncluded = (bool)reader.ReadBoolean();
                if (rightHandIncluded) {
                    ReadAvatarHandPose(reader, rightHandMovements);
                }
            }
            return targetMask;
        }

        private static void EnableHeadAnimator(Transform headTarget, bool enabled) {
            IVR_AnimatorHead animatorHead = headTarget.GetComponent<IVR_AnimatorHead>();
            if (animatorHead != null)
                animatorHead.enabled = enabled;
        }
        private static void EnableHandAnimator(Transform handTarget, bool enabled) {
            IVR_AnimatorHand animatorHand = handTarget.GetComponent<IVR_AnimatorHand>();
            if (animatorHand != null)
                animatorHand.enabled = enabled;
        }
        private static void EnableHipsAnimator(Transform hipsTarget, bool enabled) {
            IVR_AnimatorHip animatorHips = hipsTarget.GetComponent<IVR_AnimatorHip>();
            if (animatorHips != null)
                animatorHips.enabled = enabled;
        }
        private static void EnableFootAnimator(Transform footTarget, bool enabled) {
            IVR_AnimatorFoot animatorFoot = footTarget.GetComponent<IVR_AnimatorFoot>();
            if (animatorFoot != null)
                animatorFoot.enabled = enabled;
        }

        private static void ReadAvatarHandPose(NetworkReader reader, IVR_HandMovements handMovements) {
            float thumbCurl = (float)reader.ReadSingle();
            float indexCurl = (float)reader.ReadSingle();
            float middleCurl = (float)reader.ReadSingle();
            float ringCurl = (float)reader.ReadSingle();
            float littleCurl = (float)reader.ReadSingle();

            if (handMovements != null) {
                handMovements.thumbCurl = thumbCurl;
                handMovements.indexCurl = indexCurl;
                handMovements.middleCurl = middleCurl;
                handMovements.ringCurl = ringCurl;
                handMovements.littleCurl = littleCurl;
            }
        }

        private static void ReceiveTarget(NetworkReader reader, Transform targetTransform) {
            targetTransform.position = reader.ReadVector3();
            targetTransform.rotation = reader.ReadQuaternion();
        }


        public void OnDisconnectedFromServer(NetworkDisconnection info) {
            Destroy(this.gameObject);
        }

        public void OnDestroy() {
            if (instantiatedAvatar != null)
                Destroy(instantiatedAvatar.gameObject);
        }

        [Command]
        public void CmdGrab(GameObject obj, bool leftHanded) {
            Debug.Log("CmdGrab " + obj + " left: " + leftHanded);
            NetworkIdentity nwIdentity = obj.GetComponent<NetworkIdentity>();
            if (nwIdentity == null) {
                Debug.LogWarning("Grabbed object " + obj + " does not have a network identity. Its transform will not be synced across the network.");
                LocalClientGrab(obj, leftHanded);
            } else {
                RpcClientGrab(obj, leftHanded);
            }
        }

        [Command]
        public void CmdLetGo(GameObject obj, bool leftHanded) {
            Debug.Log("CmdLetGo " + obj + " left: " + leftHanded);
            NetworkIdentity nwIdentity = obj.GetComponent<NetworkIdentity>();
            if (nwIdentity == null) {
                Debug.LogWarning("Let go object " + obj + " does not have a network identity. Its transform will not be synced across the network.");
                LocalClientLetGo(obj, leftHanded);
            } else {
                RpcClientLetGo(obj, leftHanded);
            }
        }

        [ClientRpc]
        public void RpcClientGrab(GameObject obj, bool leftHanded) {
            Debug.Log("RpcClientGrab " + obj + " left: " + leftHanded);
            LocalClientGrab(obj, leftHanded);
        }

        private void LocalClientGrab(GameObject obj, bool leftHanded) {
            Debug.Log("LocalClientGrab " + obj + " left: " + leftHanded);
            if (obj != null) {
                IVR_HandMovements handMovements;
                if (leftHanded)
                    handMovements = instantiatedAvatar.leftHandTarget.GetComponent<IVR_HandMovements>();
                else
                    handMovements = instantiatedAvatar.rightHandTarget.GetComponent<IVR_HandMovements>();
                if (handMovements != null) {
                    handMovements.Grab(obj);
                }
            }
        }

        [ClientRpc]
        public void RpcClientLetGo(GameObject obj, bool leftHanded) {
            Debug.Log("RpcClientLetGo " + obj + " left: " + leftHanded);
            LocalClientLetGo(obj, leftHanded);
        }

        private void LocalClientLetGo(GameObject obj, bool leftHanded) {
            Debug.Log("LocalClientLetGo " + obj + " left: " + leftHanded);
            if (obj != null) {
                IVR_HandMovements handMovements;
                if (leftHanded)
                    handMovements = instantiatedAvatar.leftHandTarget.GetComponent<IVR_HandMovements>();
                else
                    handMovements = instantiatedAvatar.rightHandTarget.GetComponent<IVR_HandMovements>();
                if (handMovements != null) {
                    handMovements.LetGo(obj);
                }
            }
        }
    }
}
#endif