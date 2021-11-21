using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectosSonido : MonoBehaviour
{
    public AudioClip []audioClips;
    public AudioSource efecto;
    // Start is called before the first frame update
    void Start()
    {
       efecto = GetComponent<AudioSource>();
    }

    public void PlayEffect(int numeroEfecto)
    {
        if (!efecto) efecto = FindObjectOfType<AudioSource>();
        efecto.clip = audioClips[numeroEfecto];
        efecto.Play();
    }
}
