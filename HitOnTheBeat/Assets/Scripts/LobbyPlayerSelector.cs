using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayerSelector : MonoBehaviour
{
    private bool selected = false;
    public Button boton;
    public Button boton2;
    public Button boton3;
    public Text nombreText;
    public Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
    public GameObject[] playerDemo;
	public GameObject[] marcador;
    public bool personajeSeleccionado = false;

    public void OnChangeCharacter(int type)
    {
        if (!personajeSeleccionado)
        {
            personajeSeleccionado = true;
            if (!selected) boton.interactable = true;
            if (!selected) boton2.interactable = true;
            if (!selected) boton3.interactable = true;
        }
        playerDemo[FindObjectOfType<PlayerSelector>().selectedPlayer].SetActive(false);
        playerDemo[type].SetActive(true);
        marcador[previous].SetActive(false);
        marcador[type].SetActive(true);
        nombreText.text = nombre[type];
        infoText.text = info[type];
        FindObjectOfType<PlayerSelector>().selectedPlayer = type;
    }
}
