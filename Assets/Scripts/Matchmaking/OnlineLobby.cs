using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineLobby : MonoBehaviour {

    public UnityEngine.UI.Text playersList;
    public UnityEngine.UI.Text statusText;

	// Use this for initialization
	void Start () {
        ConnectToNetwork();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ConnectToNetwork()
    {
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings("v1");
    }

    void UpdatePlayersList()
    {
        playersList.text = PhotonNetwork.room.playerCount + " players in room.";
    }

    void UpdateStatusText()
    {
        if (!PhotonNetwork.connected)
        {
            statusText.text = "Not Connected.";
        }
        else if (PhotonNetwork.isMasterClient)
        {
            statusText.text = "Connected as Master Client";
        } else
        {
            statusText.text = "Connected";
        }
        UpdatePlayersList();
    }

    public void StartGame()
    {
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevel("Map1");
    }

    // Photon Callbacks
    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        UpdateStatusText();
    }

    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom("TestRoom");
        UpdateStatusText();
    }

    void OnPhotonCreateRoomFailed()
    {
        Debug.LogError("Couldn't create room.");
        UpdateStatusText();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully as player " + PhotonNetwork.player.ID);
        UpdateStatusText();
    }

    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        UpdatePlayersList();
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        UpdatePlayersList();
    }
}
