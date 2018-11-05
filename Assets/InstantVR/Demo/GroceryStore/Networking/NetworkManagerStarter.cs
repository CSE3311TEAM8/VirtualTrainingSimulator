/* InstantVR Network Avatar
 * Copyright (c) 2016 by Passer VR
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 7, 2017
 *
 * - code cleanup
 */

#if !IVR_PHOTON

using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerStarter : MonoBehaviour {

    public bool forceHost = false;
    public string url = null;

    public void Start() {
        NetworkManager nwMan = GetComponent<NetworkManager>();

        if (url != null)
            nwMan.networkAddress = url;

        if (nwMan.networkAddress == Network.player.ipAddress ||
            nwMan.networkAddress.ToLower() == "localhost" ||
            nwMan.networkAddress == "127.0.0.1" ||
            forceHost) {
            nwMan.StartServer();
        } else {
            nwMan.StartClient();
        }
    }
}

#endif