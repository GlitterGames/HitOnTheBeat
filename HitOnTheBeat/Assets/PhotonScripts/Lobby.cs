using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine.UI;
using UnityEngine.TextCore;
using UnityEngine.SceneManagement;
public class Lobby : MonoBehaviourPunCallbacks
{
	//[SerializeField] private UIManager _uiManager;
    public Button ConnectBtn;
    public Button JoinRandomBtn;
    public Text Log;
    public byte maxPlayersInRoom = 4;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    public Text PlayerCounter;
    private bool IsLoading = false;

    /*public void Connect ()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Log.text += "\nEstamos conectados al servidor";
            }
            else
            {
                Log.text += "\nError al conectar al servidor";
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Ahora estamos conectados al servidor de la región: " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
        ConnectBtn.interactable = false;
        JoinRandomBtn.interactable = true;
    }*/
    #region Photon Callbakcs
    public void JoinRandom()
    {
        if(!PhotonNetwork.JoinRandomRoom())
        {
            Log.text += "\nFallo al unirse a la sala";
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.text += "\nNo existen salas a las que unirse, creando una nueva...";
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions()
         {MaxPlayers = maxPlayersInRoom}))
         {
             Log.text += "\nSala creada con éxito";
         }
         else
         {
             Log.text += "\nFallo al crear la sala";
         }
    }

    public override void OnJoinedRoom()
    {
        Log.text += "\nUnido a la sala";
		JoinRandomBtn.interactable = false;
	}
	#endregion
    public void FixedUpdate()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerCounter.text = playerCount + "/" + maxPlayersInRoom;
            if (!IsLoading && playerCount >= minPlayersInRoom)
            {
                LoadMap();
            }
        }
    }

    private void LoadMap()
    {
        IsLoading = true;
        PhotonNetwork.LoadLevel(2);
    }
	/*public void OnPhotonSerializeView(PhotonStream, PhotonMessageInfo)
	{
		throw new NotImplementedException();
	}
	[PunRPC]
	void Method()
	{

	}
    */
}
