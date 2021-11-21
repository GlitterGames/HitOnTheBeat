using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public List<Skin> skins;
    public int selectedSkin;
    public TMP_Text nombreSkinText;
    public TMP_Text descripcionSkinText;
    public TMP_Text precioText;
    public Button comprar;

    private void OnEnable()
    {
        switch (player) {
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
        SelectSkin(selectedSkin);
        skins[selectedSkin].boton.Select();
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
    }
}
