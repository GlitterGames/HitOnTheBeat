using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonInstanciate : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public struct Skins
    {
        public List<GameObject> skins;
    }
    public static PhotonInstanciate instance;
    [SerializeField]
    public List<Skins> playerAvatar;
    public Floor[] f;
    [HideInInspector]
    public GameObject my_player;
    public GameObject ritmo;

    private PersistenceData playerSelector;

    private EfectosSonido efectosSonido;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        //instanciamos el main character.
        playerSelector = FindObjectOfType<PersistenceData>();
        int typePlayer = playerSelector.selectedPlayer;
        int skinPlayer = playerSelector.selectedSkin;
        int id = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 pos = new Vector3(f[id].transform.position.x, 0.4f, f[id].transform.position.z);
        string namePrefab = playerAvatar[typePlayer].skins[skinPlayer].name;
        my_player = PhotonNetwork.Instantiate(namePrefab, pos, Quaternion.Euler(0,180,0));
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(ritmo.name, pos, Quaternion.identity);
        MenuMusicPlayer.Instance.StopMusic();
    }

    private void Start()
    {
        //Asignamos la cámara al jugador.
        FindObjectOfType<CameraTargetSwitcher>().target = my_player.transform;
        FindObjectOfType<CameraTargetSwitcher>().SwitchToTarget();

        //Actualizamos la lista de jugadores del master.
        if (PhotonNetwork.IsMasterClient) FindObjectOfType<GameManager>().UpdatePlayers();
        else my_player.GetPhotonView().RPC("UpdatePlayersRPC", RpcTarget.All);
    }
    public void OnGoBack()
    {
        PhotonNetwork.LeaveRoom(true);
    }
}
