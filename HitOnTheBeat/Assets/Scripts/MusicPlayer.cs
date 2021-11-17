 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public AudioClip[] canciones;
    public AudioSource cancionActual;
   
    public int bpm;
  private AudioClip GetRandom()
    {
        return canciones[Random.Range(0, canciones.Length)];
    }
    // Start is called before the first frame update
    void Start()
    {
       
        cancionActual = FindObjectOfType<AudioSource>();
        cancionActual.loop = false;
        cancionActual.clip = GetRandom();
       
    }
  
    // Update is called once per frame
    void Update()
    {
        if (!cancionActual.isPlaying)
        {
           
            cancionActual.Play();
        }
    }
}