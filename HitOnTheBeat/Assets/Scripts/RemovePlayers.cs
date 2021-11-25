using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RemovePlayers : MonoBehaviourPunCallbacks
{
    //Singletone
    public static RemovePlayers instance;

    public GameManager gm;
    public bool endGame = false;
    public PlayerController.Ultimate tipoUltimate;
    public int tipoSkin;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if(gm.jugadores.Count>0) FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("SetPlayerCamera",
            collider.GetComponent<PhotonView>().Owner, gm.jugadores[0].photonView.ViewID);

        //Cuando solo queda un único jugador.
        if (gm.jugadores.Count == 1)
        {
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoEndGameRPC", RpcTarget.AllViaServer, (int) gm.jugadores[0].tipoUltimate, gm.jugadores[0].tipoSkin, FindObjectOfType<Ritmo>().numBeats);
            StartCoroutine(ExitMaster());
        }
        //Cuando dos jugadores son eliminados a la vez.
        else if(gm.jugadores.Count < 1)
        {
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoEndGameRPC", RpcTarget.AllViaServer, (int)tipoUltimate, tipoSkin, FindObjectOfType<Ritmo>().numBeats);
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