using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skin1 : MonoBehaviour
{
    public Image expositor;
    public Sprite newSprite;
    public GameObject marcador;
    public void NewImage()
    {
        expositor.sprite = newSprite;
        marcador.GetComponent<RectTransform>().anchoredPosition = new Vector2(-936, 363);
    }
}