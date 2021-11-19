using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonInstanciate : MonoBehaviourPunCallbacks
{
    public static PhotonInstanciate instance;
    public GameObject[] playerAvatar = new GameObject[2];
    public Floor[] f;
    [HideInInspector]
    public GameObject my_player;
    public GameObject ritmo;

    private PlayerSelector playerSelector;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //instanciamos el main character.
        playerSelector = FindObjectOfType<PlayerSelector>();
        int typePlayer = playerSelector.selectedPlayer;
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 pos = new Vector3(f[id].transform.position.x, 0.4f, f[id].transform.position.z);
        my_player = PhotonNetwork.Instantiate(playerAvatar[typePlayer].name, pos, Quaternion.Euler(0,180,0));
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(ritmo.name, pos, Quaternion.identity);
    }

    private void Start()
    {
        //Asignamos la cámara al jugador.
        FindObjectOfType<VirtualCameraController>().SetTarget(my_player.transform);

        //Actualizamos la lista de jugadores del master.
        if (PhotonNetwork.IsMasterClient) FindObjectOfType<GameManager>().UpdatePlayers();
        else my_player.GetPhotonView().RPC("UpdatePlayersRPC", RpcTarget.All);
    }
    public void OnGoBack()
    {
        FindObjectOfType<SceneTransitioner>().GoToLobbyScene(0);
    }
}
