using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonInstanciate : MonoBehaviour
{
    public GameObject[] playerAvatar = new GameObject[2];
    public GameObject ritmoSystem;
    public Floor[] f;
    private GameObject my_player;

    private PlayerSelector playerSelector;

    // Start is called before the first frame update
    void Awake()
    {
        //instanciamos el main character.
        playerSelector = FindObjectOfType<PlayerSelector>();
        int i = playerSelector.i;
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 pos = new Vector3(f[id].transform.position.x, 0.6f, f[id].transform.position.z);
        my_player = PhotonNetwork.Instantiate(playerAvatar[i].name, pos, Quaternion.Euler(0,180,0));
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(this.ritmoSystem.name, ritmoSystem.transform.position, Quaternion.identity);
    }

    private void Start()
    {
        //Asignamos la cámara al jugador.
        FindObjectOfType<VirtualCameraController>().SetTarget(my_player.transform);

        //Actualizamos la lista de jugadores del master.
        if (PhotonNetwork.IsMasterClient) FindObjectOfType<GameManager>().UpdatePlayers();
        else my_player.GetPhotonView().RPC("UpdatePlayersRPC", PhotonNetwork.MasterClient);
    }
}
