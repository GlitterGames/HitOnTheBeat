using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prueba : MonoBehaviour
{

    public Vector3 posicionInicial;


    // Start is called before the first frame update
    void Start()
    {
        posicionInicial = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += new Vector3(100 * Time.deltaTime, 0f, 0f);
        if (transform.localPosition.x > 550)
        {
            gameObject.SetActive(false);
        }
        
    }
}
