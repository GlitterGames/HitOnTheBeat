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
using TMPro;
public class Lobby : MonoBehaviourPunCallbacks
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
    public Button empezarPartida;
    public TMP_Text PlayerCounter;
    public EfectosSonido efectosSonido;
    public GameObject CanvasLobby;
    public GameObject CanvasRoom;
    public GameObject CanvasRoomPrivada;
    public GameObject gameObjectCrearSalaPrivada;
    public GameObject gameObjectUnirseSalaPrivada;
    public Button botonCrearSalaPrivada;
    public Button botonUnirseSalaPrivada;

    public Text Jugador1;
    public Text Jugador2;
    public Text Jugador3;
    public Text Jugador4;



    public void Start()
    {

        if (!FindObjectOfType<PersistenceData>()) DontDestroyOnLoad(Instantiate(playerSelector,
             playerSelector.transform.position, playerSelector.transform.rotation));

        efectosSonido = GetComponent<EfectosSonido>();
    }

    #region Photon Callbakcs
   

    public void CreateRoom()
    {
        efectosSonido.PlayEffect(0);
        buscarPartida.interactable = false;
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
        CanvasLobby.SetActive(false);
        CanvasRoom.SetActive(true);
        CanvasRoomPrivada.SetActive(false);
        

}
   
    void OnPlayerLeftRoom(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
        }
    }
    void OnPhotonPlayerConnected()
    {
        UpdatePlayerList();
    }
    public void UpdatePlayerList()
    {
         Player[] jugadores= PhotonNetwork.PlayerList;
       /* foreach(Player player in PhotonNetwork.playerList)
        { 
            jugadores.Add(player);
        }*/


            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Jugador1.text = PhotonNetwork.PlayerList[0].NickName;
                Jugador2.text = null;
                Jugador3.text = null;
                Jugador4.text = null;
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Jugador1.text = PhotonNetwork.PlayerList[0].NickName;
                Jugador2.text = PhotonNetwork.PlayerList[1].NickName;
                Jugador3.text = null;
                Jugador4.text = null;

        }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                Jugador1.text = PhotonNetwork.PlayerList[0].NickName;
                Jugador2.text = PhotonNetwork.PlayerList[1].NickName;
                Jugador3.text = PhotonNetwork.PlayerList[2].NickName;
                Jugador4.text = null;

        }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                Jugador1.text = PhotonNetwork.PlayerList[0].NickName;
                Jugador2.text = PhotonNetwork.PlayerList[1].NickName;
                Jugador3.text = PhotonNetwork.PlayerList[2].NickName;
                Jugador4.text = PhotonNetwork.PlayerList[3].NickName;

            }
        
       
       
    }
    public void JoinRandom()
    {
        efectosSonido.PlayEffect(0);
        buscarPartida.interactable = false;
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
           Debug.Log("Sala creada con �xito");
         }
         else
         {
            Debug.Log( "Fallo al crear la sala");
         }
    }
    public void OnRoomNameChanged(string nameIn)
    {
        botonCrearSalaPrivada.interactable = true;
        roomName = nameIn;

    }
    public void AbandonarSala()
    {
        PhotonNetwork.LeaveRoom();
        CanvasLobby.SetActive(true);
        CanvasRoom.SetActive(false);
        CanvasRoomPrivada.SetActive(false);
        buscarPartida.interactable = true;
        crearSala.interactable = true;

    }
    public void IrASalaPrivada()
    {
        CanvasLobby.SetActive(false);
        CanvasRoomPrivada.SetActive(true);
       
        


    }
    

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
        maxPlayersInRoom = (byte)roomSize;

    }

    public void JoinLobbyOnClick()
    {
        efectosSonido.PlayEffect(0);
        buscarPartida.interactable = false;
        crearSala.interactable = false;
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    public void FixedUpdate()
    {
        if (roomName=="")
        {
            botonCrearSalaPrivada.interactable = false;
            botonUnirseSalaPrivada.interactable = false;
        }
        else
        {
            botonCrearSalaPrivada.interactable = true;
            botonUnirseSalaPrivada.interactable = true;
        }
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
            UpdatePlayerList();
        }
        
    }
   
    public void EnviarEmpezarPartida()
    {
        efectosSonido.PlayEffect(2);
        photonView.RPC("EnviarEmpezarPartidaRPC", RpcTarget.AllViaServer);
    }
    
    [PunRPC]
    public void EnviarEmpezarPartidaRPC()
    {
        LoadMap();
    }
    public void LoadMap()
    {
        IsLoading = true;
        FindObjectOfType<SceneTransitioner>().StartTransition(2, 0.5f, "Cargando partida...");
    }

    public void OnGoBack()
    {
        efectosSonido.PlayEffect(1);
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        FindObjectOfType<SceneTransitioner>().StartTransition(0, 0f);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        PhotonNetwork.Disconnect();
    }
}
