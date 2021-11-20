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
public class LobbyPrivado : MonoBehaviourPunCallbacks
{
    public GameObject playerSelector;
    public byte maxPlayersInRoom ;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    private bool IsLoading = false;
    public string roomName;
    public int roomSize;
    public Button buscarPartida;
    public Button crearSala;
    public Button unirseSala;
    public Button empezarPartida;
    public Text PlayerCounter;
    



    public void Start()
    {

        if (!FindObjectOfType<PlayerSelector>()) DontDestroyOnLoad(Instantiate(playerSelector,
             playerSelector.transform.position, playerSelector.transform.rotation));


        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbakcs
   

    public void CreateRoom()
    {
        buscarPartida.interactable = false;
        unirseSala.interactable = false;
        crearSala.interactable = false;

        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
            PhotonNetwork.CreateRoom(roomName, roomOps);

        Debug.Log("Sala creada");

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Ya existe una sala con ese nombre");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("unido a sala"); 
         



    }
  public void JoinRandom()
    {
        buscarPartida.interactable = false;
        unirseSala.interactable = false;
        crearSala.interactable = false;
        if (!PhotonNetwork.JoinRandomRoom())
        {
            Debug.Log("Fallo al unirse a la sala");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log( "No existen salas a las que unirse, creando una nueva...");
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions()
         {MaxPlayers = maxPlayersInRoom}))
         {
           Debug.Log("Sala creada con éxito");
         }
         else
         {
            Debug.Log( "Fallo al crear la sala");
         }
    }
    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;

    }
    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
        maxPlayersInRoom = (byte)roomSize;

    }

    public void JoinLobbyOnClick()
    {
        buscarPartida.interactable = false;
        unirseSala.interactable = false;
        crearSala.interactable = false;
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    public void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerCounter.text = playerCount.ToString();


            if (!IsLoading && playerCount >= minPlayersInRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    empezarPartida.interactable = true;
                }
            }
        }
        
    }
   
    public void EnviarEmpezarPartida()
    {
        Debug.Log("Enviando mensaje ");
        photonView.RPC("EnviarEmpezarPartidaRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void EnviarEmpezarPartidaRPC()
    {
        Debug.Log("Mensaje RPC");
        LoadMap();
    }
    public void LoadMap()
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
}
