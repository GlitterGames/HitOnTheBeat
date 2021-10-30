﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    //Prefab del canvas.
    public Animator anim;
    int level = 0;

    public void StartTransition(int level, float delay)
    {
        anim.SetTrigger("Activate");
        this.level = level;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }
    public void StartTransition(int level)
    {
        anim.SetTrigger("Activate");
        this.level = level;
    }

    public void EndTransition()
    {
        SceneManager.LoadScene(level);
    }

    IEnumerator InstantEndTransition(float delay)
    {
        yield return new WaitForSeconds(0.55f + delay);
        SceneManager.LoadScene(level);
    }    
}