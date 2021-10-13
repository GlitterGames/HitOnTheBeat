using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Atributes
    public Floor f;
    public GameObject playerAvatar;
    public float speed;
    public float rotationSpeed;
    public bool moving;
    public bool rotating;
    public Vector3 movingDirection;
    public Vector3 targetPosition;
    public float distanceToTarget;

    private InputController my_input;
    #endregion

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
            Floor targetFloor = hit.transform.GetComponent<Floor>();
            if (targetFloor)
            {
                Floor nextFloor = null;

                if (targetFloor.Equals(f.getNorth_west()))
                {
                    nextFloor = f.getNorth_west();
                }
                else if (targetFloor.Equals(f.getNorth_east()))
                {
                    nextFloor = f.getNorth_east();
                }
                else if (targetFloor.Equals(f.getWest()))
                {
                    nextFloor = f.getWest();
                }
                else if (targetFloor.Equals(f.getEast()))
                {
                    nextFloor = f.getEast();
                }
                else if (targetFloor.Equals(f.getSouth_west()))
                {
                    nextFloor = f.getSouth_west();
                }
                else if (targetFloor.Equals(f.getSouth_east()))
                {
                    nextFloor = f.getSouth_east();
                }

                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    transform.position = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
                    f = nextFloor;
                }
            }
        }
    }

    //UNA MEJOR MANERA DE HACERLO PERO QUE TIENE EL MISMO RESILTADO, ASÍ QUE SI ES NECESARIO LO INTENTARÉ IMPLEMENTAR EN OTRO MOMENTO 
    //private void OnTriggerEnter(Collider other)
    //{
    //    Floor floor = other.GetComponent<Floor>();
    //
    //    if (floor != null)
    //    {
    //        Debug.LogWarning("A OTRA BALDOSA");
    //        f = floor;
    //    }
    //}
}
