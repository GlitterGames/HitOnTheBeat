using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brillo : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image panelBrillo;
    public Sprite []sprites;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0.5f - PlayerPrefs.GetFloat("brillo", 0.5f);
        Debug.Log(slider.value);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderValue);
        RevisarBrillo(0.5f - slider.value);
    }

    public void ChangeSlider (float valor)
    {
        Debug.Log(valor);
        sliderValue = 0.5f - valor;
        RevisarBrillo(sliderValue);
        PlayerPrefs.SetFloat("brillo", sliderValue);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderValue);
    }
    public void RevisarBrillo(float valor)
    {
        if (valor == 0.5f)
            image.sprite = sprites[0];
        else
            image.sprite = sprites[1];
    }
}
