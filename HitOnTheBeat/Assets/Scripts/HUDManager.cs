using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    //Ultimates
    public Button ultimate;
    private int m_ultimateCharge;
    public int UltimateCharge {
        get
        {
            return m_ultimateCharge;
        }
        set
        {
            m_ultimateCharge = value;
            if (m_ultimateCharge > 100) m_ultimateCharge = 100;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ultimate.onClick.AddListener(() => DoUltimate());
    }

    private void DoUltimate()
    {
        PhotonInstanciate.instance.my_player.GetComponent<PlayerController>().StartUltimate();
        UltimateCharge = 0;
        ultimate.interactable = false;
    }

    public void AddUltimateCharge(int value)
    {
        UltimateCharge += value;
        if (UltimateCharge == 100) UltimateReady();
    }

    private void UltimateReady()
    {
        ultimate.interactable = true;
    }
}
