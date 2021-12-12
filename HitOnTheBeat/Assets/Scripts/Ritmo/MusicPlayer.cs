 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicPlayer : MonoBehaviourPun
{

    public AudioClip[] canciones;
    public AudioSource cancionActual;

    [HideInInspector] public Ritmo ritmo;
    private int numeroCambios = 0;
    private float segundosEspera;
    private float bpm;
    private GameManager gameManager;
    public float duracion;
  private int GetRandom()
    {
        return Random.Range(0, canciones.Length);
    }
    // Start is called before the first frame update
    void Start()
    {
        ritmo = GetComponent<Ritmo>();
        cancionActual = GetComponent<AudioSource>();
        if (!PhotonNetwork.IsMasterClient) return;
        gameManager = FindObjectOfType<GameManager>();
        int numeroCancion = GetRandom();
        duracionCancion(numeroCancion);
        StartCoroutine(WaitForSong(numeroCancion));
    }

    private void duracionCancion(int numeroCancion)
    {
        gameManager.duracion = canciones[numeroCancion].length;
    }
    IEnumerator WaitForSong(int numeroCancion)
    {
        yield return new WaitForSeconds(6f);
        StartSong(numeroCancion);
        StartCoroutine(ChangeBMP(numeroCancion, numeroCambios));
    }

    public void StartSong(int numeroCancion)
    {
        photonView.RPC("StartSongRPC", RpcTarget.AllViaServer, numeroCancion);
    }
    
    [PunRPC]
    private void StartSongRPC(int numeroCancion)
    {
        cancionActual.clip = canciones[numeroCancion];
        if (!cancionActual.isPlaying)
        {
            cancionActual.Play();
        }
    }

    IEnumerator ChangeBMP(int numeroCancion, int numeroCambios)
    {
        if (numeroCancion == 0)
        {
            switch(numeroCambios)
            {
                case 0:
                        segundosEspera = 19.83f;
                        bpm = 1.20f;
                    break;
                case 1:
                        segundosEspera = 9.14f;
                        bpm = 1.14f;
                    break;
                case 2:
                        segundosEspera = 8.72f;
                        bpm = 1.09f;
                    break;
                case 3:
                        segundosEspera = 8.38f;
                        bpm = 1.04f;
                    break;
                case 4:
                        segundosEspera = 8;
                        bpm = 1;
                    break;
                case 5:
                        segundosEspera = 7.68f;
                        bpm = 0.96f;
                    break;
                case 6:
                        segundosEspera = 7.39f;
                        bpm = 0.92f;
                    break;
                case 7:
                        segundosEspera = 51.52f;
                        bpm = 0.88f;
                    break;
                default:
                        segundosEspera = 20;
                        bpm = 2;
                    break;
            }
        }
        else
        {
            switch(numeroCambios)
            {
                case 0:
                        segundosEspera = 57.8f;
                        bpm = 1.775f;
                    break;
                case 1:
                        segundosEspera = 28.44f;
                        bpm = 0.885f;
                    break;
                case 2:
                        segundosEspera = 14.22f;
                        bpm = 1.775f;
                    break;
                case 3:
                        segundosEspera = 56.88f;
                        bpm = 0.885f;
                    break;
                case 4:
                        segundosEspera = 24.94f;
                        bpm = 1.775f;
                        break;
                default:
                        segundosEspera = 20;
                        bpm = 2;
                    break;
            }
        }
        numeroCambios++;
        SendChangeBMP(bpm);
        yield return new WaitForSeconds(segundosEspera);
        StartCoroutine(ChangeBMP(numeroCancion, numeroCambios));
    }

    public void SendChangeBMP(float bpm)
    {
        gameManager.ChangeAnimationSpeedOnAllPlayers(bpm);
        photonView.RPC("SendChangeBMPRPC", RpcTarget.AllViaServer, bpm);
    }
    
    [PunRPC]
    private void SendChangeBMPRPC(float bpm)
    {
        if(Ritmo.instance) Ritmo.instance.Delay = bpm;
    }
}