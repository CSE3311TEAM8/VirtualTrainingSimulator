/* InstantVR Photon/PUN Networking Manager Starter
 * author: Pascal Serrarens
 * email: support@passervr.com
 * version: 3.8.0
 * date: April 17, 2017
 *
 * - Fixes for Photon script not loading issue
 */

using UnityEngine;

#if !IVR_PHOTON
public class PUNManagerStarter : MonoBehaviour {
#else
public class PUNManagerStarter : Photon.PunBehaviour {
#endif
    public string roomName;
    public string gameVersion;

    public int sendRate = 25;

    public GameObject playerPrefab;

#if !IVR_PHOTON
    public string error = "Error: Photon Networking SDK is not installed.";
    public void Start() {
        Debug.LogError("Photon Networking SDK is not installed.");
    }
#else
    private GameObject _playerObject;
    public GameObject playerObject {
        get {
            return _playerObject;
        }
    }

    public void Start () {
        PhotonNetwork.sendRate = sendRate;
        PhotonNetwork.sendRateOnSerialize = sendRate;
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }
    
    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master " + PhotonNetwork.countOfPlayersInRooms);

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
        Debug.LogError("Could not joint the " + roomName + " room");
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined "+ roomName +" room");
        _playerObject = PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, playerPrefab.transform.rotation, 0);
    }
#endif
}
