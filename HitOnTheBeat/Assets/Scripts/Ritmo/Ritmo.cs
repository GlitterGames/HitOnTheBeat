using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Ritmo : MonoBehaviourPun
{
    public static Ritmo instance;
    [System.Serializable]
    public struct Colores
    {
        public Color noClickeable; //0
        public Color clickeable;   //1
        public Color acierto;      //2
        public Color fallo;        //3
    }
    public Colores colores;
    public GameObject marcador;
    public GameObject indicador;
    private GameManager gameManager;
    public float delay;
    [Range(0,1)]
    public float successPercentaje;
    private float currentTime;
    
    //Variables master
    public bool puedeClickear = false;
    public bool beatDone = false;

    //variables de uso no compartido
    public bool haFallado = false;
    public bool haPulsado = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetColor(colores.noClickeable);
        if (!PhotonNetwork.IsMasterClient) return;
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        ManageTime();

        if (!puedeClickear && !beatDone)
        {
            SetColorAllBeatEnd();
            gameManager.DoBeatActions();
            beatDone = true;
        }
        else if(puedeClickear && beatDone)
        {
            SetColorAllBeatStart();
            beatDone = false;
        }
        else if(!puedeClickear) // && beatDone
        {
            if (gameManager.movimientos.Count != 0) gameManager.movimientos.Clear();
        }
    }

    public void ManageTime()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime / delay >= (1 - successPercentaje)) puedeClickear = true;
        if (currentTime > delay)
        {
            currentTime = 0;
            puedeClickear = false;
        }
        IndicatorSetSize(currentTime / delay);
    }

    public bool TryMovePlayer()
    {
        if (puedeClickear) return true;
        else return false;
    }

    public void IndicatorSetSize(float size)
    {
        photonView.RPC("IndicatorSetSizeRPC", RpcTarget.AllViaServer, size);
    }

    public void SetColor(Color color)
    {
        marcador.GetComponent<Image>().color = color;
    }
    public void SetColorAllBeatStart()
    {
        photonView.RPC("SetColorBeatStartRPC", RpcTarget.AllViaServer);
    }
    public void SetColorAllBeatEnd()
    {
        photonView.RPC("SetColorBeatEndRPC", RpcTarget.AllViaServer);
    }

    public Color IntToColor(int index)
    {
        switch(index)
        {
            default:
                return colores.noClickeable;
            case 1:
                return colores.clickeable;
            case 2:
                return colores.acierto;
            case 3:
                return colores.fallo;
        }
    }

    [PunRPC]
    public void SetColorRPC(int color)
    {
        marcador.GetComponent<Image>().color = IntToColor(color);
    }

    [PunRPC]
    public void SetColorBeatStartRPC()
    {
        if (!haFallado) marcador.GetComponent<Image>().color = colores.clickeable;
    }

    [PunRPC]
    public void SetColorBeatEndRPC()
    {
        if (!haFallado) {
            marcador.GetComponent<Image>().color = colores.noClickeable;
        }
        if (!haPulsado)
        {
            marcador.GetComponent<Image>().color = colores.fallo;
            FindObjectOfType<PhotonInstanciate>().my_player.GetPhotonView().RPC("NoHaPulsadoRPC", RpcTarget.All);
        }
        haFallado = false;
        haPulsado = false;
    }

    [PunRPC]
    public void IndicatorSetSizeRPC(float size)
    {
        indicador.GetComponent<IndicadorController>().SetSize(size);
    }
}
