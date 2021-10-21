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
    public Button ConnectBtn;
    public Button JoinRandomBtn;
    public Text Log;
    public byte maxPlayersInRoom = 4;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    public Text PlayerCounter;
    private bool IsLoading = false;

    #region Photon Callbakcs
    public void JoinRandom()
    {
        if(!PhotonNetwork.JoinRandomRoom())
        {
            Debug.Log( "Fallo al unirse a la sala");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log( "No existen salas a las que unirse, creando una nueva...");
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions()
         {MaxPlayers = maxPlayersInRoom}))
         {
             Debug.Log( "Sala creada con éxito");
         }
         else
         {
             Debug.Log( "Fallo al crear la sala");
         }
    }

    public override void OnJoinedRoom()
    {
       Debug.Log("Unido a la sala");
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
}
