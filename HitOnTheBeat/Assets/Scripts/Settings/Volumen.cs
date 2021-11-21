using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Volumen : MonoBehaviour
{
    public Slider sliderSonido, sliderEfectos;
    public Image imagenMuteSonido, imagenMuteEfectos;
    public Sprite []sprites;
    public AudioMixer masterMixer;

    void Start()
    {
        sliderSonido.value = PlayerPrefs.GetFloat("volumenSonido", -20);
        sliderEfectos.value = PlayerPrefs.GetFloat("volumenEfectos", -20);
        masterMixer.SetFloat("Music", sliderSonido.value);
        masterMixer.SetFloat("SoundEffects", sliderEfectos.value);
        RevisarSiEstoyMuteSonido(sliderSonido.value);
        RevisarSiEstoyMuteEfectos(sliderEfectos.value);
    }

    public void ChangeSliderSonido(float valor)
    {
        PlayerPrefs.SetFloat("volumenSonido", valor);
        masterMixer.SetFloat("Music", valor);
        RevisarSiEstoyMuteSonido(valor);
    }
    
    public void ChangeSliderEfectos(float valor)
    {
        PlayerPrefs.SetFloat("volumenEfectos", valor);
        masterMixer.SetFloat("SoundEffects", valor);
        RevisarSiEstoyMuteEfectos(valor);
    }

    public void RevisarSiEstoyMuteSonido(float valor)
    {
        if (valor == -80)
            imagenMuteSonido.sprite = sprites[0];
        else
            imagenMuteSonido.sprite = sprites[1];
    }

        public void RevisarSiEstoyMuteEfectos(float valor)
    {
        if (valor == -80)
            imagenMuteEfectos.sprite = sprites[2];
        else
            imagenMuteEfectos.sprite = sprites[3];
    }
}
