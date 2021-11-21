using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayerSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Skins
    {
        public List<GameObject> skins;
    }
    public List<Skins> playerDemo;
    private bool selected = false;
    public Button boton;
    public Button boton2;
    public Button boton3;
    public Text nombreText;
    public Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
	public GameObject[] marcador;
    public bool personajeSeleccionado = false;

    public EfectosSonido efectosSonido;

    void Start()
    {
        efectosSonido = GetComponent<EfectosSonido>();
    }
    public void OnChangeCharacter(int type)
    {
        efectosSonido.PlayEffect(type);
        if (!personajeSeleccionado)
        {
            personajeSeleccionado = true;
            if (!selected) boton.interactable = true;
            if (!selected) boton2.interactable = true;
            if (!selected) boton3.interactable = true;
        }
        int selectedSkin = 0;
        switch (type)
        {
            case 0:
                selectedSkin = PlayerPrefs.GetInt("skinPunchPrincess", 0);
                break;
            case 1:
                selectedSkin = PlayerPrefs.GetInt("skinXXColor", 0);
                break;
            case 2:
                selectedSkin = PlayerPrefs.GetInt("skinFrank", 0);
                break;
        }
        playerDemo[FindObjectOfType<PlayerSelector>().selectedPlayer]
            .skins[FindObjectOfType<PlayerSelector>().selectedSkin].SetActive(false);
        playerDemo[type].skins[selectedSkin].SetActive(true);
        marcador[FindObjectOfType<PlayerSelector>().selectedPlayer].SetActive(false);
        marcador[type].SetActive(true);
        nombreText.text = nombre[type];
        infoText.text = info[type];
        FindObjectOfType<PlayerSelector>().selectedPlayer = type;
        FindObjectOfType<PlayerSelector>().selectedSkin = selectedSkin;
    }
}
