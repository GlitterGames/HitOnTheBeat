using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputController my_input;
    // Start is called before the first frame update
    void Awake()
    {
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();
    }

    //Para que funcione el Input System en la versión actual.
    private void OnEnable()
    {
        my_input.Enable();
    }

    private void OnDisable()
    {
        my_input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Se ejecuta cuando se realiza click en la pantalla.
    private void OnClick()
    {
        Vector3 screenPos = my_input.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Click realizado sobre casilla
            //Cambiar esta condición por reglas de juego.
            if (hit.transform.name == "Casilla")
            {
                Debug.Log("Destruccion");
            }
        }
    }
}
