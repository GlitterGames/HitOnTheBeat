using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RemovePlayers : MonoBehaviourPunCallbacks
{
    public GameManager gm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("SetPlayerCamera",
            collider.GetComponent<PhotonView>().Owner, gm.jugadores[0].photonView.ViewID);

        if (gm.jugadores.Count == 1)
        {
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoUpdateWinner", RpcTarget.AllViaServer, (int) gm.jugadores[0].tipoPersonaje);
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoExitPlayer", RpcTarget.Others);
            StartCoroutine(ExitMaster());
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(4);
    }

    public void ExitPlayer()
    {
        PhotonNetwork.LeaveRoom(true);
    }

    IEnumerator ExitMaster()
    {
        yield return new WaitForSeconds(1);
        ExitPlayer();
    }
}