using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class photonInstanciate : MonoBehaviourPun
{
    public GameObject playerAvatar;
    public Floor [] f = new Floor [2];

    public int idPlayer = 0;
    // Start is called before the first frame update
    void Awake()
    {
        Vector3[] positions = { new Vector3 { x = f[0].transform.position.x, y = 0, z = f[0].transform.position.z }, 
                                new Vector3 { x = f[1].transform.position.x, y = 0, z = f[1].transform.position.z} };
        Vector3 posicionInicial;
        posicionInicial = new Vector3(f[idPlayer].transform.position.x, transform.position.y, f[idPlayer].transform.position.z);
        transform.position = posicionInicial;
        Photon.Pun.PhotonNetwork.Instantiate(this.playerAvatar.name, transform.position, Quaternion.identity);
    }
}
