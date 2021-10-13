using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comportamiento : MonoBehaviour
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
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }
    void ReadInput()
    {
        Floor nextFloor = null;
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.LogWarning("PRESIONASTE LA W");
            nextFloor = f.getNorth_west();
            if (nextFloor == null) Debug.LogWarning("ahí no hay nada");
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            nextFloor = f.getNorth_east();
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            nextFloor = f.getWest();
        }else if (Input.GetKeyDown(KeyCode.D))
        {
            nextFloor = f.getEast();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            nextFloor = f.getSouth_west();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            nextFloor = f.getSouth_east();
        }

        //PERFORM MOVEMENT
        if (nextFloor != null)
        {
            transform.position= new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
            f = nextFloor;
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
