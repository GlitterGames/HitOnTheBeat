using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NCPrivada : MonoBehaviourPunCallbacks
{
    //[SerializeField] private UIManager _uiManager;
    public GameObject conectando;
    private SceneTransitioner st;
    void Start()
    {
        Debug.Log("Start");
    }
    

    public void Awake()
    {
        st = FindObjectOfType<SceneTransitioner>();
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
      PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado a la lobby general.");
        //PhotonNetwork.CreateRoom(_name.text);
        SceneManager.LoadScene(1);
        st.EndTransition();
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

    public void OnExit()
    {
        Application.Quit();
    }
}
