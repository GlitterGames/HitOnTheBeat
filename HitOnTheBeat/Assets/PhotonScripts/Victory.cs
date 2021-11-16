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

    void Start()
    {
        inGameData = FindObjectOfType<PlayerSelector>();
        playerDemo[inGameData.playerWinner].SetActive(true);
        hitsText.SetText("Golpes dados: " + inGameData.hitsStats);
        jumpText.SetText("Saltos: " + inGameData.jumpStats);
        pushText.SetText("Golpes recibidos: " + inGameData.pushStats);
        killsText.SetText("Bajas: " + inGameData.killsStats);
        averageRhythmText.SetText("Ritmo medio: " + inGameData.averageRhythmStats);
    }
}
