 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicPlayer : MonoBehaviourPun
{

    public AudioClip[] canciones;
    public AudioSource cancionActual;

    public Ritmo ritmo;
    private int pos;
    private int numeroCambios = 0;
    private int segundosEspera;
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
        StartCoroutine(ChangeBMP(numeroCancion, numeroCambios));
    }

    private void duracionCancion(int numeroCancion)
    {
        gameManager.duracion = canciones[numeroCancion].length;
    }
    IEnumerator WaitForSong(int numeroCancion)
    {
        yield return new WaitForSeconds(3f);
        StartSong(numeroCancion);
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
                        segundosEspera = 10;
                        bpm = 1.34f;
                    break;
                case 1:
                        segundosEspera = 10;
                        bpm = 1.14f;
                    break;
                case 2:
                        segundosEspera = 10;
                        bpm = 1.09f;
                    break;
                case 3:
                        segundosEspera = 20;
                        bpm = 1.04f;
                    break;
                case 4:
                        segundosEspera = 20;
                        bpm = 1;
                    break;
                case 5:
                        segundosEspera = 20;
                        bpm = 0.96f;
                    break;
                case 6:
                        segundosEspera = 20;
                        bpm = 0.92f;
                    break;
                case 7:
                        segundosEspera = 20;
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
                        segundosEspera = 30;
                        bpm = 1;
                    break;
                case 1:
                        segundosEspera = 20;
                        bpm = 2;
                    break;
                case 2:
                        segundosEspera = 20;
                        bpm = 3;
                    break;
                case 3:
                        segundosEspera = 20;
                        bpm = 2;
                    break;
                case 4:
                        segundosEspera = 20;
                        bpm = 2;
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
        photonView.RPC("SendChangeBMPRPC", RpcTarget.AllViaServer, bpm);
    }
    
    [PunRPC]
    private void SendChangeBMPRPC(float bpm)
    {
        gameManager.ChangeAnimationSpeedOnAllPlayers(bpm);
        Ritmo.instance.delay = bpm;
    }
}