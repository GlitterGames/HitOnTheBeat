using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonInstanciate : MonoBehaviourPun
{
    public GameObject playerAvatar;
    public Floor[] f;

    // Start is called before the first frame update
    void Start()
    {
        //instanciamos el main character.
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 pos = new Vector3(f[id].transform.position.x, 0.5f, f[id].transform.position.z);
        GameObject g = PhotonNetwork.Instantiate(this.playerAvatar.name, pos, Quaternion.identity);
        FindObjectOfType<VirtualCameraController>().SetTarget(g.transform);
    }
}
