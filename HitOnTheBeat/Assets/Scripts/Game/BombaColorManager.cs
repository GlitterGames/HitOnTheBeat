using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombaColorManager : MonoBehaviourPun
{
    public GameObject bombaNoUsable;
    public GameObject bombaMoverUsable;
    public GameObject bombaVisibleUsable;
    public GameObject explosion;
    [Range(0,5)]
    public float durationEffect = 1f;
    [HideInInspector]
    public Floor target;
    [HideInInspector]
    public int fuerzaEmpleada;
    private Vector3 initialPos;
    private float currentTime;
    // Start is called before the first frame update

    public void StartAnimation(Floor target, float delay)
    {
        this.target = target;
        explosion.SetActive(false);
        bombaNoUsable.SetActive(false);
        bombaVisibleUsable.SetActive(true);
        bombaMoverUsable.GetComponent<Animator>().enabled = true;
        bombaMoverUsable.GetComponent<Animator>().speed = 1f / (delay-1f);
        GetComponent<PlayerController>().SetAreaBombaColor(target);
        initialPos = bombaMoverUsable.transform.position;
        PlayerController personaje = GetComponent<PlayerController>();
        fuerzaEmpleada = personaje.Fuerza;
        personaje.Fuerza = 0;

        StartCoroutine(LanzarBomba(delay));
    }

    IEnumerator LanzarBomba(float delay)
    {
        while(currentTime < delay)
        {
            bombaMoverUsable.transform.position = Vector3.Lerp(initialPos, target.transform.position, currentTime / delay);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        bombaMoverUsable.GetComponent<Animator>().enabled = false;
        currentTime = 0;
        bombaNoUsable.SetActive(true);
        bombaVisibleUsable.SetActive(false);
        explosion.SetActive(true);
        yield return new WaitForSeconds(durationEffect);
        explosion.SetActive(false);
    }
}
