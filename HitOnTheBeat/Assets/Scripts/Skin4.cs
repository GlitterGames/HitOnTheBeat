using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skin4 : MonoBehaviour
{

    public Image expositor;
    public Sprite newSprite;
    public GameObject marcador;
    public EfectosSonido efectosSonido;

    public void NewImage()
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(0);
        expositor.sprite = newSprite;
        marcador.GetComponent<RectTransform>().anchoredPosition = new Vector2(-936, 63);
    }
}

