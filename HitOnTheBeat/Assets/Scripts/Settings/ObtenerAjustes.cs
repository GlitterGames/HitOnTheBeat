using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class ObtenerAjustes : MonoBehaviour
{
    public Image panelBrillo;
    public AudioMixer masterMixer;
    private float valorBrillo;
    private float valorMusica;
    private float valorEfectos;
    // Start is called before the first frame update
    void Start()
    {
        valorBrillo = PlayerPrefs.GetFloat("brillo", 0.5f);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, valorBrillo);

        valorMusica = PlayerPrefs.GetFloat("volumenSonido", -20);
        valorEfectos = PlayerPrefs.GetFloat("volumenEfectos", -20);
        masterMixer.SetFloat("Music", valorMusica);
        masterMixer.SetFloat("SoundEffects", valorEfectos);
    }
}
