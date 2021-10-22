using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Button ultimate;
    public PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        ultimate.onClick.AddListener(() => DoUltimate());
        if (!pc) pc = FindObjectOfType<PlayerController>(); //cambiar para que detecte la del jugador propio.
    }

    private void DoUltimate()
    {
        Debug.Log("Atacar");
    }
}
