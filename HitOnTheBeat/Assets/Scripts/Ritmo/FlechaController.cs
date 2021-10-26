using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaController : MonoBehaviour
{
    private void Start()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight/10 , 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(100 * Time.deltaTime, 0f, 0f);
        if (transform.position.x < Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth * 2, 0, 0)).x)
        {
            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.tag == "Activante")
        {
            Ritmo.instance.puedeClickear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otroObjeto)
    {

        if (otroObjeto.tag == "Activante")
        {
            Ritmo.instance.puedeClickear = false;
        }
    }
}
