using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RemovePlayers : MonoBehaviourPunCallbacks
{
    public GameManager gm;
    public bool endGame = false;
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
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoEndGameRPC", RpcTarget.AllViaServer, (int) gm.jugadores[0].tipoUltimate, gm.jugadores[0].tipoSkin, FindObjectOfType<Ritmo>().numBeats);
            StartCoroutine(ExitMaster());
        }
    }
    
    public override void OnLeftRoom()
    {
        if(endGame)
            FindObjectOfType<SceneTransitioner>().GoToVictoryScene(0);
        else
            FindObjectOfType<SceneTransitioner>().GoToLobbyScene(0);
    }

    IEnumerator ExitMaster()
    {
        yield return new WaitForSeconds(2);

        PhotonNetwork.LeaveRoom(true);
    }
}