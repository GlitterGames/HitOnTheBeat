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
    public GameObject playerSelector;
    public Button JoinRandomBtn;
    public Text Log;
    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    public Text PlayerCounter;
    private bool IsLoading = false;

    public void Start()
    {
        if (!FindObjectOfType<PlayerSelector>()) DontDestroyOnLoad(Instantiate(playerSelector,
             playerSelector.transform.position, playerSelector.transform.rotation));
    }

    #region Photon Callbakcs
    public void JoinRandom()
    {
        if(!PhotonNetwork.JoinRandomRoom())
        {
            Log.text = "Fallo al unirse a la sala";
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Log.text = "No existen salas a las que unirse, creando una nueva...";
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions()
         {MaxPlayers = maxPlayersInRoom}))
         {
            Log.text = "Sala creada con éxito";
         }
         else
         {
            Log.text = "Fallo al crear la sala";
         }
    }

    public override void OnJoinedRoom()
    {
        Log.text = "Unido a la sala";
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
        FindObjectOfType<SceneTransitioner>().StartTransition(2, 0.5f);
    }

    public void OnGoBack()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        FindObjectOfType<SceneTransitioner>().StartTransition(0, 0);
    }

        public void GoSettings()
    {
         FindObjectOfType<SceneTransitioner>().StartTransition(5, 0.5f);;
    }
}
