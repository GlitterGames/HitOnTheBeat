using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SkinSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Skin
    {
        public GameObject skinDemo;
        public Button boton;
        public string nombre;
        [TextArea(1, 10)]
        public string descripcion;
        public bool purchased;
        public int precio;
    }
    public int player;
    public GameObject marcador;
    public List<Skin> skins;
    public int selectedSkin;
    public TMP_Text nombreSkinText;
    public TMP_Text descripcionSkinText;
    public TMP_Text precioText;
    public Button comprar;

    private void OnEnable()
    {
        marcador.SetActive(true);
        CargarPreferencias();
    }

    //Cuando se cierra el panel de este personaje
    //se oculta el modelo mostrado en ese momento.
    private void OnDisable()
    {
        if(marcador) marcador.SetActive(false);
        if (skins[selectedSkin].skinDemo) skins[selectedSkin].skinDemo.SetActive(false);
    }

    public void SelectSkin(int index)
    {
        Skin skin = skins[index];
        if (skin.purchased)
        {
            comprar.interactable = false;
            precioText.SetText("");
            switch (player)
            {
                case 0:
                    PlayerPrefs.SetInt("skinPunchPrincess", index);
                    break;
                case 1:
                    PlayerPrefs.SetInt("skinXXColor", index);
                    break;
                case 2:
                    PlayerPrefs.SetInt("skinFrank", index);
                    break;
            }
        }
        else
        {
            comprar.interactable = true;
            precioText.SetText(skin.precio.ToString());
        }
        nombreSkinText.SetText(skin.nombre);
        descripcionSkinText.SetText(skin.descripcion);
        skins[selectedSkin].skinDemo.SetActive(false);
        selectedSkin = index;
        skin.skinDemo.SetActive(true);
    }

    public void PurchaseSkin()
    {
        Skin skin = skins[selectedSkin];
        int gemas = PlayerPrefs.GetInt("gemas", 0);
        if (gemas < skin.precio) return;
        else FindObjectOfType<GemasManager>().AddTickets(-skin.precio);
        skin.purchased = true;
        skins[selectedSkin] = skin;
        comprar.interactable = false;
        precioText.SetText("");
        switch (player)
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
        SavePurchasedSkins(selectedSkin, true);
    }

    private void CargarPreferencias()
    {
        switch (player)
        {
            case 0:
                selectedSkin = PlayerPrefs.GetInt("skinPunchPrincess", 0);
                UpdatePurchasedSkins(PlayerPrefs.GetInt("purchasedPunchPrincess", 1));
                break;
            case 1:
                selectedSkin = PlayerPrefs.GetInt("skinXXColor", 0);
                UpdatePurchasedSkins(PlayerPrefs.GetInt("purchasedXXColor", 1));
                break;
            case 2:
                selectedSkin = PlayerPrefs.GetInt("skinFrank", 0);
                UpdatePurchasedSkins(PlayerPrefs.GetInt("purchasedFrank", 1));
                break;
        }
        SelectSkin(selectedSkin);
        skins[selectedSkin].boton.Select();
    }

    #region PlayerPref compras
    private void UpdatePurchasedSkins(int purchasedSkins)
    {
        for (int i = 0; i < skins.Count; i++)
        {
            Skin s = skins[i];
            bool purchased = Convert.ToBoolean(purchasedSkins & 1);
            s.purchased = purchased;
            skins[i] = s;
            purchasedSkins >>= 1;
        }
    }

    private void SavePurchasedSkins(int index, bool value)
    {
        Skin s = skins[index];
        s.purchased = value;
        skins[index] = s;
        int pref;
        switch (player)
        {
            case 0:
                pref = PlayerPrefs.GetInt("purchasedPunchPrincess", 1);
                break;
            case 1:
                pref = PlayerPrefs.GetInt("purchasedXXColor", 1);
                break;
            case 2:
                pref = PlayerPrefs.GetInt("purchasedFrank", 1);
                break;
            default:
                pref = 1;
                break;
        }
        if (value)
        {
            pref |= 1 << index;
        }
        else
        {
            pref &= 0 << index;
        }
        switch (player)
        {
            case 0:
                PlayerPrefs.SetInt("purchasedPunchPrincess", pref);
                break;
            case 1:
                PlayerPrefs.SetInt("purchasedXXColor", pref);
                break;
            case 2:
                PlayerPrefs.SetInt("purchasedFrank", pref);
                break;
        }
    }
    #endregion
}
