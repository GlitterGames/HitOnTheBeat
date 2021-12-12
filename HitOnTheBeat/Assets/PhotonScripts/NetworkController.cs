using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkController : MonoBehaviourPunCallbacks
{
    //[SerializeField] private UIManager _uiManager;
    private SceneTransitioner st;
    public EfectosSonido efectosSonido;

    public void Awake()
    {
        st = FindObjectOfType<SceneTransitioner>();
    }

    void Start()
    {
        MenuMusicPlayer.Instance.PlayMusic(0);
        efectosSonido = GetComponent<EfectosSonido>();
        _name.text = PlayerPrefs.GetString("name", "");
    }
    [SerializeField] private TMP_InputField _name;

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        if(_name.text=="") { PhotonNetwork.NickName = "Fulanito"; }
        else { PhotonNetwork.NickName = _name.text; }
        PlayerPrefs.SetString("name", _name.text);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(1);
        st.EndTransition();
    }

    public void Connect ()
    {
        efectosSonido.PlayEffect(0);
        st.StartTransition(1, "Conectando con el Servidor...");
        if (!PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.ConnectUsingSettings())
            {
                //Debug.Log("\nEstamos conectados al servidor");
            }
            else
            {
                //Debug.Log("\nError al conectar al servidor");
                st.EndTransition();
            }
        }
        else
        {
            //Debug.Log("\nYa conectado");
        }
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
