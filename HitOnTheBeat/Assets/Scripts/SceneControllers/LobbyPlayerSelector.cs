using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LobbyPlayerSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Skins
    {
        public List<GameObject> skins;
    }
    public List<Skins> playerDemo;
    public List<Button> botones;
    public TMP_Text nombreText;
    public TMP_Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
	public GameObject[] marcador;
    public bool personajeSeleccionado = false;

    public EfectosSonido efectosSonido;
    private PersistenceData persistence;

    private void Awake()
    { 
        MenuMusicPlayer.Instance.PlayMusic(0);
    }

    void Start()
    {
        efectosSonido = GetComponent<EfectosSonido>();
        persistence = FindObjectOfType<PersistenceData>();
    }
    public void OnChangeCharacter(int type)
    {
        efectosSonido.PlayEffect(type);
        if (!personajeSeleccionado)
        {
            personajeSeleccionado = true;
            foreach(var b in botones)
                b.interactable = true;
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
        playerDemo[persistence.selectedPlayer]
            .skins[persistence.selectedSkin].SetActive(false);
        playerDemo[type].skins[selectedSkin].SetActive(true);
        marcador[persistence.selectedPlayer].SetActive(false);
        marcador[type].SetActive(true);
        nombreText.SetText(nombre[type]);
        infoText.SetText(info[type]);
        persistence.selectedPlayer = type;
        persistence.selectedSkin = selectedSkin;

        //Permitir el cambio de skin.
        bool[] compradas = GetPurchasedSkins();
        for (int i = 1; i < compradas.Length; i++)
        {
            if (compradas[i]) goto Encontrada;
        }

        for (int i = botones.Count - 2; i < botones.Count; i++)
            botones[i].interactable = false;
        return;

        //Si se ha encontrado alguna comprada.
        Encontrada:
            for (int i = botones.Count - 2; i < botones.Count; i++)
                botones[i].interactable = true;
    }


    #region GestionSkins
    public void NextSkin()
    {
        bool[] compradas = GetPurchasedSkins();
        int actualSkin = persistence.selectedSkin;
        do
        {
            actualSkin++;
            if (actualSkin >= compradas.Length) actualSkin = 0;
        } while (!compradas[actualSkin]);
        UpdateSelectedSkin(actualSkin);
    }

    public void PreviousSkin()
    {
        bool[] compradas = GetPurchasedSkins();
        int actualSkin = persistence.selectedSkin;
        do
        {
            actualSkin--;
            if (actualSkin < 0) actualSkin = compradas.Length - 1;
        } while (!compradas[actualSkin]);
        UpdateSelectedSkin(actualSkin);
    }
    private bool[] GetPurchasedSkins()
    {
        int purchasedSkins;
        switch (persistence.selectedPlayer)
        {
            case 0:
                purchasedSkins = PlayerPrefs.GetInt("purchasedPunchPrincess", 1);
                break;
            case 1:
                purchasedSkins = PlayerPrefs.GetInt("purchasedXXColor", 1);
                break;
            case 2:
                purchasedSkins = PlayerPrefs.GetInt("purchasedFrank", 1);
                break;
            default:
                purchasedSkins = 1;
                break;
        }

        int numSkins = playerDemo[persistence.selectedPlayer].skins.Count;
        bool[] compradas = new bool[numSkins];
        for (int i = 0; i < numSkins; i++)
        {
            compradas[i] = Convert.ToBoolean(purchasedSkins & 1);
            purchasedSkins >>= 1;
        }
        return compradas;
    }

    public void UpdateSelectedSkin(int selectedSkin)
    {
        playerDemo[persistence.selectedPlayer].skins[persistence.selectedSkin].SetActive(false);
        playerDemo[persistence.selectedPlayer].skins[selectedSkin].SetActive(true);
        persistence.selectedSkin = selectedSkin;
        switch (persistence.selectedPlayer)
        {
            case 0:
                PlayerPrefs.SetInt("skinPunchPrincess", selectedSkin);
                break;
            case 1:
                PlayerPrefs.SetInt("skinXXColor", selectedSkin);
                break;
            case 2:
                PlayerPrefs.SetInt("skinFrank", selectedSkin);
                break;
        }
    }
    #endregion
}
