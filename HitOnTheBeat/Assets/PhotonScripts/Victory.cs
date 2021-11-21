using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] playerDemo;
    public GameObject[] puesto;
    private PlayerSelector inGameData;
    public TMP_Text hitsText;
    public TMP_Text jumpText;
    public TMP_Text pushText;
    public TMP_Text killsText;
    public TMP_Text averageRhythmText;
    public EfectosSonido efectosSonido;

    void Start()
    {
        efectosSonido = GetComponent<EfectosSonido>();
        inGameData = FindObjectOfType<PlayerSelector>();
        puesto[inGameData.puesto].SetActive(true);
        playerDemo[inGameData.playerWinner].SetActive(true);
        hitsText.SetText("Golpes dados: " + inGameData.hitsStats);
        jumpText.SetText("Saltos: " + inGameData.jumpStats);
        pushText.SetText("Golpes recibidos: " + inGameData.pushStats);
        killsText.SetText("Bajas: " + inGameData.killsStats);
        averageRhythmText.SetText("Ritmo ratio: " + inGameData.averageRhythmStats + "%");
        switch (inGameData.selectedPlayer)
        {
            case 0:
                efectosSonido.PlayEffect(0);    //Punch
                break;
            case 1:
                efectosSonido.PlayEffect(1);    //XX
                break;
            case 2:
                efectosSonido.PlayEffect(2);    //FANTASMA
                break;
        }
        
    }
}
