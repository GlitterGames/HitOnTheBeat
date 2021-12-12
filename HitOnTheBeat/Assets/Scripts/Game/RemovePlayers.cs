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
    public string winnerName;
    public PlayerController.Ultimate winnerUltimate;
    public int winnerSkin;

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

        CheckEndGame();
    }
    
    public override void OnLeftRoom()
    {
        FindObjectOfType<CameraTargetSwitcher>().SwitchToFreeCamera();
        if(endGame)
            FindObjectOfType<SceneTransitioner>().GoToVictoryScene(0);
        else
            FindObjectOfType<SceneTransitioner>().StartTransition(1, 1f, "Abandonando partida...");
    }

    //Cuando un jugador abandona la sala.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient || endGame) return;

        int pc = gm.jugadores.FindIndex((pc) => pc.GetIdPlayer() == otherPlayer.ActorNumber - 1);
        gm.RemovePlayer(pc);
        CheckEndGame();
    }

    //Cambio de master.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (endGame) return;
        //Si sale el master, acaba la partida.
        PlayerController winner = FindObjectOfType<PhotonInstanciate>().my_player.GetComponent<PlayerController>();
        winner.DoEndGameRPC("MASTER DESCONECTADO",
            (int)winner.tipoUltimate, winner.tipoSkin, FindObjectOfType<Ritmo>().numBeats);
    }

    IEnumerator ExitMaster()
    {
        yield return new WaitForSeconds(2);

        PhotonNetwork.LeaveRoom(true);
    }

    private void CheckEndGame()
    {
        //Cuando solo queda un único jugador.
        if (gm.jugadores.Count == 1)
        {
            endGame = true;
            PlayerController winner = gm.jugadores[0];
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoEndGameRPC", RpcTarget.AllViaServer, "GANADOR: " + winner.photonView.Owner.NickName,
                (int)winner.tipoUltimate, winner.tipoSkin, FindObjectOfType<Ritmo>().numBeats);
            //StartCoroutine(ExitMaster());
        }
        //Cuando dos jugadores son eliminados a la vez.
        else if (gm.jugadores.Count < 1)
        {
            endGame = true;
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("DoEndGameRPC", RpcTarget.AllViaServer, "GANADOR: " + winnerName,
                (int)winnerUltimate, winnerSkin, FindObjectOfType<Ritmo>().numBeats);
            //StartCoroutine(ExitMaster());
        }
    }
}