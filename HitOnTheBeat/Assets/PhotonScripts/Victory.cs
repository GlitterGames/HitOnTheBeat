using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] playerDemo;
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
        playerDemo[inGameData.playerWinner].SetActive(true);
        hitsText.SetText("Golpes dados: " + inGameData.hitsStats);
        jumpText.SetText("Saltos: " + inGameData.jumpStats);
        pushText.SetText("Golpes recibidos: " + inGameData.pushStats);
        killsText.SetText("Bajas: " + inGameData.killsStats);
        averageRhythmText.SetText("Ritmo ratio: " + inGameData.averageRhythmStats + "%");
        switch (inGameData.selectedPlayer)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                efectosSonido.PlayEffect(0);    //Punch
                break;
            case 4:
            case 5:
            case 6:
            case 7:
                efectosSonido.PlayEffect(1);    //XX
                break;
            case 8:
            case 9:
                efectosSonido.PlayEffect(2);    //FANTASMA
                break;
        }
        
    }
}
