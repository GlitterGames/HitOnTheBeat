using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombaColorManager : MonoBehaviourPun
{
    public GameObject bombaNoUsable;
    public GameObject bombaUsable;
    public GameObject explosion;
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
        bombaUsable.SetActive(true);
        bombaUsable.GetComponent<Animator>().speed = 1f / (Ritmo.instance.delay * GetComponent<PlayerController>().ULTIMATE_MAX_BEAT_DURATION -1f);
        GetComponent<PlayerController>().SetAreaBombaColor(target);
        initialPos = bombaUsable.transform.position;
        PlayerController personaje = GetComponent<PlayerController>();
        fuerzaEmpleada = personaje.Fuerza;
        personaje.Fuerza = 0;

        StartCoroutine(LanzarBomba(delay));
    }

    IEnumerator LanzarBomba(float delay)
    {
        while(currentTime < delay)
        {
            bombaUsable.transform.position = Vector3.Lerp(initialPos, target.transform.position, currentTime / delay);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        currentTime = 0;
        bombaNoUsable.SetActive(true);
        bombaUsable.SetActive(false);
        explosion.SetActive(true);
    }
}
