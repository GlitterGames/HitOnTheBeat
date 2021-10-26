using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ritmo : MonoBehaviour
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
        instance = this;
        InvokeRepeating("InvocarFlecha", tiempoInicial, delay);
    }
    void Start()
    {
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
            //Animaci�n de acierto.

            return true;
        }
        else
        {
            //Animaci�n de fallo.

            return false;
        }
    }

    public void InvocarFlecha()
    {
       GameObject nuevaFlecha= Instantiate(flecha, flecha.transform.position, flecha.transform.rotation);
       nuevaFlecha.transform.SetParent(GetComponentInChildren<Canvas>().transform, false);

    }
}
