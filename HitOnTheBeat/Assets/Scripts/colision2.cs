using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colision2 : MonoBehaviour
{
    public bool puedePulsarse;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
            if (puedePulsarse)
            {       
                Juego.instance.Acierto();
            }
        
        
    }
    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {

        if (otroObjeto.tag == "Activante")
        {
            puedePulsarse = true;

        }

    }

    private void OnTriggerExit2D(Collider2D otroObjeto)
    {

        if (otroObjeto.tag == "Activante")
        {
            puedePulsarse = false;
            
        }

    }
}
