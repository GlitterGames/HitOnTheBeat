using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class photonInstanciate : MonoBehaviourPun
{
    private const int MAX_PLAYERS = 4;
    public GameObject casilla;
    public GameObject playerAvatar;
    public Floor[] f = new Floor[MAX_PLAYERS];

    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        List<Transform[]> casillas = new List<Transform[]>();
        casillas.Add(GameObject.Find("Row0").GetComponentsInChildren<Transform>());
        casillas.Add(GameObject.Find("Row1").GetComponentsInChildren<Transform>());
        casillas.Add(GameObject.Find("Row2").GetComponentsInChildren<Transform>());
        casillas.Add(GameObject.Find("Row3").GetComponentsInChildren<Transform>());
        casillas.Add(GameObject.Find("Row4").GetComponentsInChildren<Transform>());
        casillas.Add(GameObject.Find("Row5").GetComponentsInChildren<Transform>());
        for (int i = 0; i < casillas.Count; i++)
        {
            Transform[] transform = casillas[i];
            foreach (Transform t in transform)
            {
                if (t.gameObject.name == "hexagon")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    photonView.RPC("SetCasillaRPC", RpcTarget.All, t.localScale, i, go.GetPhotonView().ViewID);
                }
                else if (t.gameObject.name == "hexagon1")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    photonView.RPC("SetCasillaRPC", RpcTarget.All, t.localScale, i, go.GetPhotonView().ViewID);
                    f[0] = go.GetComponent<Floor>();
                }
                else if (t.gameObject.name == "hexagon2")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    photonView.RPC("SetCasillaRPC", RpcTarget.All, t.localScale, i, go.GetPhotonView().ViewID);
                    f[1] = go.GetComponent<Floor>();
                }
                else if (t.gameObject.name == "hexagon3")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    photonView.RPC("SetCasillaRPC", RpcTarget.All, t.localScale, i, go.GetPhotonView().ViewID);
                    f[2] = go.GetComponent<Floor>();
                }
                else if (t.gameObject.name == "hexagon4")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    photonView.RPC("SetCasillaRPC", RpcTarget.All, t.localScale, i, go.GetPhotonView().ViewID);
                    f[3] = go.GetComponent<Floor>();
                }
            }
        }

        Vector3 pos = new Vector3 (f[0].transform.position.x, 0.5f, f[0].transform.position.z);

        //instanciamos el main character.

        Photon.Realtime.Player[] jugadores = PhotonNetwork.PlayerList;

        GameObject g = PhotonNetwork.Instantiate(this.playerAvatar.name, pos, Quaternion.identity);
        g.GetComponent<PlayerController>().typeAnt = FloorDetectorType.West;
        g.GetComponent<PlayerController>().actualFloor = f[0];
        g.GetComponent<PlayerController>().newPos = g.transform.position;

        //Puede fallar.
        for (int i = 1; i < jugadores.Length; i++)
        {
            photonView.RPC("SetPlayerRPC", jugadores[i], f[i].gameObject.GetPhotonView().ViewID);
        }
    }

    [PunRPC]
    public void SetCasillaRPC(Vector3 scale, int row, int id)
    {
        Floor f = PhotonView.Find(id).GetComponent<Floor>();
        f.transform.localScale = scale;
        f.id = id;
        f.row = row;
    }

    //Instanciamos otros jugadores.
    [PunRPC]
    public void SetPlayerRPC(int fa)
    {
        Floor f = PhotonView.Find(fa).GetComponent<Floor>();
        Vector3 pos = new Vector3(f.transform.position.x, 0.5f, f.transform.position.z);
        GameObject go = PhotonNetwork.Instantiate(this.playerAvatar.name, pos, Quaternion.identity);
        go.GetComponent<PlayerController>().typeAnt = FloorDetectorType.West;
        go.GetComponent<PlayerController>().actualFloor = f;
        go.GetComponent<PlayerController>().newPos = transform.position;
    }
}
