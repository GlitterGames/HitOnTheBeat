using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinCharacterSelector : MonoBehaviour
{
    public List<GameObject> botones;
    public List<GameObject> personajes;
    public List<Transform> positionMarcador;
    public GameObject marcador;
    public int seleccion;

    private void Start()
    {
        SelectCharacter(0);   
    }

    public void SelectCharacter(int index)
    {
        botones[seleccion].SetActive(false);
        personajes[seleccion].SetActive(false);
        seleccion = index;
        botones[seleccion].SetActive(true);
        personajes[seleccion].SetActive(true);
        marcador.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionMarcador[index].position.x,
            positionMarcador[index].position.y);
    }
}