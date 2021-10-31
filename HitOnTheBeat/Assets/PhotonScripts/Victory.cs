using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] playerDemo;
    void Start()
    {
        playerDemo[FindObjectOfType<PlayerSelector>().playerWinner].SetActive(true);
    }
}
