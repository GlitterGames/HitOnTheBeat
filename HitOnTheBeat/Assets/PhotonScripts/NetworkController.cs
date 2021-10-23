using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviourPunCallbacks
{
    //[SerializeField] private UIManager _uiManager;
    void Start()
    {
        Debug.Log("Start");
    }
    [SerializeField] private InputField _name;

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = _name.text;
        Debug.Log(_name + "estás conectado al servidor de la región: " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado a la lobby general.");
        //PhotonNetwork.CreateRoom(_name.text);
        SceneManager.LoadScene(1);
    }

        public void Connect ()
    {
        Debug.Log("Botón de connect pulsado");
        if (!PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("\nEstamos conectados al servidor");
            }
            else
            {
                 Debug.Log("\nError al conectar al servidor");
            }
        }
        else
        {
            Debug.Log("\nYa conectado");
        }
    }
}