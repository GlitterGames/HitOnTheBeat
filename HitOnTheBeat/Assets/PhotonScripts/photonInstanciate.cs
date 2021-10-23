using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class photonInstanciate : MonoBehaviourPun
{
    private const int PLAYERS = 2;
    public GameObject casilla;
    public GameObject playerAvatar;
    public Floor[] f = new Floor[PLAYERS];
    Vector3[] positions = new Vector3[PLAYERS];

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
                    go.transform.localScale = t.localScale;
                    go.GetComponent<Floor>().row = i;
                    go.GetComponent<Floor>().id = go.GetComponent<PhotonView>().ViewID;
                }
                else if (t.gameObject.name == "hexagon1")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    go.transform.localScale = t.localScale;
                    go.GetComponent<Floor>().row = i;
                    go.GetComponent<Floor>().id = go.GetComponent<PhotonView>().ViewID;
                    f[0] = go.GetComponent<Floor>();
                }
                else if (t.gameObject.name == "hexagon2")
                {
                    GameObject go = PhotonNetwork.Instantiate(casilla.name, t.position, Quaternion.identity);
                    go.transform.localScale = t.localScale;
                    go.GetComponent<Floor>().row = i;
                    go.GetComponent<Floor>().id = go.GetComponent<PhotonView>().ViewID;
                    f[1] = go.GetComponent<Floor>();
                }
            }
        }

        positions[0] = new Vector3 (f[0].transform.position.x, 0, f[0].transform.position.z);
        positions[1] = new Vector3 (f[1].transform.position.x, 0, f[1].transform.position.z);
        GameObject g = PhotonNetwork.Instantiate(this.playerAvatar.name, positions[0], Quaternion.identity);
        photonView.RPC("SetPlayerRPC", RpcTarget.Others, f[0].gameObject.GetPhotonView().ViewID, f[1].gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    public void SetPlayerRPC(int f1, int f2)
    {
        GameObject go = PhotonNetwork.Instantiate(this.playerAvatar.name, positions[1], Quaternion.identity);
        f[0] = PhotonView.Find(f1).GetComponent<Floor>();
        f[1] = PhotonView.Find(f2).GetComponent<Floor>();
    }
}
