using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelector : MonoBehaviour
{
    public int selectedPlayer;
    private bool selected = false;
    public Button boton;
    public Text nombreText;
    public Text infoText;
    public string[] nombre = new string[3];
    [TextArea(1, 10)]
    public string[] info;
    public GameObject[] playerDemo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnChangeCharacter(int type)
    {
        if (!selected) boton.interactable = true;
        playerDemo[selectedPlayer].SetActive(false);
        playerDemo[type].SetActive(true);
        nombreText.text = nombre[type];
        infoText.text = info[type];
        selectedPlayer = type;
    }
}
