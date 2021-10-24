using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juego : MonoBehaviour
{

    public prueba movimiento;
    public static Juego instance;
    public GameObject flecha;
    public float tiempo;
    public float retraso;
    public GameObject myPrefab;
    public GameObject canvasObject;




    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        InvokeRepeating("InvocarFlecha", tiempo, retraso);
      
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Acierto()
    {
        Debug.Log("Acierto");

    }
    public void Fallo()
    {
        Debug.Log("Fallo");
    }
    public void InvocarFlecha()
    {
       // Instantiate(flecha, movimiento.posicionInicial, flecha.transform.rotation,GameObject.FindGameObjectWithTag("Canvas").transform);
       GameObject nuevaFlecha= Instantiate(myPrefab, flecha.transform.position, flecha.transform.rotation);
        // myPrefab.transform.SetParent(canvasObject.transform, false);
        //myPrefab.transform.localScale = new Vector3(1, 1, 1);
        nuevaFlecha.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);



    }
}
