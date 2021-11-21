using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinCharacterSelector : MonoBehaviour
{
    public List<GameObject> botones;
    public List<GameObject> personajes;
    public int seleccion = 0;

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
    }
}