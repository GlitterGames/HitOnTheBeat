﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTransitioner : MonoBehaviour
{
    //Prefab del canvas.
    public Animator anim;
    public TMP_Text info;
    public EfectosSonido efectosSonido;
    int level = 0;

    public void StartTransition(int level, float delay, string texto)
    {
        StartTransition(level, delay);
        info.SetText(texto);
    }

    public void StartTransition(int level, float delay)
    {
        anim.SetTrigger("Activate");
        this.level = level;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }
    public void StartTransition(int level, string texto)
    {
        StartTransition(level);
        info.SetText(texto);
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

    #region Eventos
    public void GoToMainScene(float delay)
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(1);
        anim.SetTrigger("Activate");
        this.level = 0;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }
    public void GoToLobbyScene(float delay)
    {
        efectosSonido = GetComponent<EfectosSonido>();
        if(efectosSonido) efectosSonido.PlayEffect(0);
        else
        {
            FindObjectOfType<EfectosSonido>().PlayEffect(0);
        }
        GameObject.Find("Animation").GetComponent<Animator>().SetTrigger("Activate");
        this.level = 1;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }

    public void GoToGameScene(float delay)
    {
        anim.SetTrigger("Activate");
        this.level = 2;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }

    public void GoToCreditsScene(float delay)
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(0);
        anim.SetTrigger("Activate");
        this.level = 3;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }
    
    public void GoToVictoryScene(float delay)
    {
        GameObject.Find("Animation").GetComponent<Animator>().SetTrigger("Activate");
        this.level = 4;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }
    public void GoToShopScene(float delay)
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(0);
        anim.SetTrigger("Activate");
        this.level = 5;
        if (delay >= 0) StartCoroutine(InstantEndTransition(delay));
    }

    public void GoSettings()
    {
        efectosSonido = GetComponent<EfectosSonido>();
        efectosSonido.PlayEffect(0);
        StartTransition(6, 0);
    }
    #endregion
}
