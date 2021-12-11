using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyPlayerSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Skins
    {
        public List<GameObject> skins;
    }
    public List<Skins> playerDemo;
    public Button boton;
    public Button boton2;
    public Button boton3;
    public TMP_Text nombreText;
    public TMP_Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
	public GameObject[] marcador;
    public bool personajeSeleccionado = false;

    public EfectosSonido efectosSonido;

    private void Awake()
    { 
        MenuMusicPlayer.Instance.PlayMusic(0);
    }

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
            boton.interactable = true;
            boton2.interactable = true;
            boton3.interactable = true;
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
        playerDemo[FindObjectOfType<PersistenceData>().selectedPlayer]
            .skins[FindObjectOfType<PersistenceData>().selectedSkin].SetActive(false);
        playerDemo[type].skins[selectedSkin].SetActive(true);
        marcador[FindObjectOfType<PersistenceData>().selectedPlayer].SetActive(false);
        marcador[type].SetActive(true);
        nombreText.SetText(nombre[type]);
        infoText.SetText(info[type]);
        FindObjectOfType<PersistenceData>().selectedPlayer = type;
        FindObjectOfType<PersistenceData>().selectedSkin = selectedSkin;
    }
}
