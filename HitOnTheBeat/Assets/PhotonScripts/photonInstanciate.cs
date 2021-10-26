using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonInstanciate : MonoBehaviour
{
    public GameObject playerAvatar;
    public Floor[] f;
    private GameObject my_player;

    // Start is called before the first frame update
    void Awake()
    {
        //instanciamos el main character.
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 pos = new Vector3(f[id].transform.position.x, 0.5f, f[id].transform.position.z);
        my_player = PhotonNetwork.Instantiate(this.playerAvatar.name, pos, Quaternion.identity);
    }

    private void Start()
    {
        FindObjectOfType<VirtualCameraController>().SetTarget(my_player.transform);
        if (PhotonNetwork.IsMasterClient) FindObjectOfType<GameManager>().UpdatePlayers();
        else my_player.GetPhotonView().RPC("UpdatePlayersRPC", PhotonNetwork.MasterClient);
    }
}
