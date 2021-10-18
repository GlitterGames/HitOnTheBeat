using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photonInstanciate : MonoBehaviour
{
    public GameObject playerAvatar;
    public Floor f;

    // Start is called before the first frame update
    void Awake()
    {
        Vector3 posicionInicial;
        posicionInicial = new Vector3(f.transform.position.x, transform.position.y, f.transform.position.z);
        transform.position = posicionInicial;
        Photon.Pun.PhotonNetwork.Instantiate(this.playerAvatar.name, transform.position, Quaternion.identity);
        
    }
}
