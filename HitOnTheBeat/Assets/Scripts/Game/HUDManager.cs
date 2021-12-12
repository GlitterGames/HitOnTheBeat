using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    //Static values
    public const int ULTIMATE_MAX_CHARGE = 3;
    public static HUDManager instance;

    //Editor
    public int durationUltimate;

    public Image icono;
    public List<Sprite> personajesIcono;
    public Image medidorFuerza;
    public List<Sprite> estadosFuerza;
    public Image medidorUltimate;
    public List<Sprite> estadosUltimate;
    public Image powerUp;
    public List<Sprite> estadosPowerUp;

    //References
    private PlayerController myPC;

    //Ultimates
    public Button ultimate;
    public List<Sprite> tiposUltimate;

    private int m_ultimateCharge;
    public int UltimateCharge {
        get
        {
            return m_ultimateCharge;
        }
        set
        {
            m_ultimateCharge = value;
            if (m_ultimateCharge > ULTIMATE_MAX_CHARGE) m_ultimateCharge = ULTIMATE_MAX_CHARGE;
            if (UltimateCharge == ULTIMATE_MAX_CHARGE) UltimateReady();
            SetUltimate(m_ultimateCharge);
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Obtener referencias
        myPC = FindObjectOfType<PhotonInstanciate>().my_player.GetComponent<PlayerController>();
        UltimateCharge = ULTIMATE_MAX_CHARGE;

        //Inicializamos la interfaz a valores por defecto u obtenidos de Lobby.
        int ps = FindObjectOfType<PlayerSelector>().selectedPlayer;
        icono.sprite = personajesIcono[ps];
        ultimate.GetComponent<Image>().sprite = tiposUltimate[ps];
        SetFuerza(myPC.Fuerza);
        SetPowerUp((int) myPC.Power);
        SetUltimate(m_ultimateCharge);

        ultimate.onClick.AddListener(() => DoUltimate());
        durationUltimate = -1;
    }

    private void DoUltimate()
    {
        PhotonInstanciate.instance.my_player.GetComponent<PlayerController>().StartUltimate();
        UltimateCharge = 0;
        ultimate.interactable = false;
    }

    private void UltimateReady()
    {
        ultimate.interactable = true;
    }

    //HUD
    public void SetFuerza(int state)
    {
        medidorFuerza.sprite = estadosFuerza[state];
    }

    public void SetUltimate(int state)
    {
        medidorUltimate.sprite = estadosUltimate[state];
    }

    public void SetPowerUp(int state)
    {
        powerUp.sprite = estadosPowerUp[state];
    }
}
