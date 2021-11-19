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
    public byte maxPlayersInRoom = 2;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    private bool IsLoading = false;
    public string roomName;
    public int roomSize;
    public GameObject roomListingPrefab;
    public Transform roomsPanel;
    

    
    public void Start()
    {
        if (!FindObjectOfType<PlayerSelector>()) DontDestroyOnLoad(Instantiate(playerSelector,
             playerSelector.transform.position, playerSelector.transform.rotation));


        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbakcs
   

    public void CreateRoom()
    {
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
  /*  public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        foreach (RoomInfo room in roomList)
        {
            ListRoom(room);
        }
    }
    public void RemoveRoomListings()
    {
        while (roomsPanel.childCount != 0)
        {
            Destroy(roomsPanel.GetChild(0).gameObject);
        }
    }
    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.SetRoom();

        }
    }*/
    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }
    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);

    }

    public void JoinLobbyOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    public void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            
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
}
