using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ritmo : MonoBehaviourPun
{
    public static Ritmo instance;
    public bool puedeClickear = false;
    public GameObject flecha;
    public GameManager gameManager;
    public float tiempoInicial;
    public float delay;
    public bool beatDone = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        instance = this;
        InvokeRepeating("InvocarFlecha", tiempoInicial, delay);
    }
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if(!puedeClickear && !beatDone)
        {
            gameManager.DoBeatActions();
            beatDone = true;
        }
        else if(puedeClickear && beatDone)
        {
            beatDone = false;
        }
        else if(!puedeClickear)
        {
            if (gameManager.movimientos.Count != 0) gameManager.movimientos.Clear();
        }
    }

    public bool TryMovePlayer()
    {
        if (puedeClickear)
        {
            //Animación de acierto.

            return true;
        }
        else
        {
            //Animación de fallo.

            return false;
        }
    }

    public void InvocarFlecha()
    {
        photonView.RPC("InvocarFlechaRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void InvocarFlechaRPC()
    {
        GameObject nuevaFlecha = Instantiate(flecha, flecha.transform.position, flecha.transform.rotation);
        nuevaFlecha.transform.SetParent(GetComponentInChildren<Canvas>().transform, false);
    }
}
