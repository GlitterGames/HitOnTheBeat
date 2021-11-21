using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PantallaCompleta : MonoBehaviour
{
    public Toggle toggle;
    Resolution[] resoluciones;
    private EfectosSonido efectosSonido;
    // Start is called before the first frame update
    void Start()
    {
        
            if(Screen.fullScreen)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
    }

    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(0);
        Screen.fullScreen = pantallaCompleta;
    }
}
