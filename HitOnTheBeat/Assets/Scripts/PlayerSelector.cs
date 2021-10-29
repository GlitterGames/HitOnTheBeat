using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public int i = 0;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public void OnPunchPrincess()
    {
        i = 0;
    }

    public void OnCani()
    {
        i = 1;
    }
}
