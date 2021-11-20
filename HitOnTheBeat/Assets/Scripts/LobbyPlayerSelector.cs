using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayerSelector : MonoBehaviour
{
    private bool selected = false;
    public Button boton;
    public Text nombreText;
    public Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
    public GameObject[] playerDemo;
    public GameObject[] marcador;

    public void OnChangeCharacter(int type)
    {
        if (!selected) boton.interactable = true;
        int previous = FindObjectOfType<PlayerSelector>().selectedPlayer;
        playerDemo[previous].SetActive(false);
        playerDemo[type].SetActive(true);
        marcador[previous].SetActive(false);
        marcador[type].SetActive(true);
        nombreText.text = nombre[type];
        infoText.text = info[type];
        FindObjectOfType<PlayerSelector>().selectedPlayer = type;
    }
}
