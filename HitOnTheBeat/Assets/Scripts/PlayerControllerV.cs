using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
public class PlayerControllerV : MonoBehaviourPun
{
    #region Atributes
    private string _name = "personaje";
    public Floor f;
    public GameObject playerAvatar;
    public float speed;
    public float rotationSpeed;
    public bool moving;
    public bool rotating;
    public bool Player=true;
    public Vector3 movingDirection;
    public Vector3 targetPosition;
    public float distanceToTarget;

    public Button ConnectBtn;
    public Button JoinRandomBtn;
    public Text Log;
    public byte maxPlayersInRoom = 4;
    public byte minPlayersInRoom = 2;
    private int playerCount = 0;
    public Text PlayerCounter;
    private bool IsLoading = false;

    private InputController my_input;
    #endregion

   /* public void JoinRandom()
    {
        if (!PhotonNetwork.JoinRandomRoom())
        {
            Debug.Log( "\nFallo al unirse a la sala");
        }
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log ("\nNo existen salas a las que unirse, creando una nueva...");
        if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions()
        { MaxPlayers = maxPlayersInRoom }))
        {
            Debug.Log( "\nSala creada con éxito");
        }
        else
        {
          Debug.Log("\nFallo al crear la sala");
        }
    }

    public void OnJoinedRoom()
    {
       Debug.Log( "\nUnido a la sala");
        JoinRandomBtn.interactable = false;
    }
    
    */
    // Start is called before the first frame update
    void Awake()
    {
        //JoinRandom();
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();
        //Photon.Pun.PhotonNetwork.Instantiate(_name, transform.position, Quaternion.identity);
     
    }

    //Para que funcione el Input System en la versi�n actual.
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
    public void FixedUpdate()
    {
       /*if (PhotonNetwork.CurrentRoom != null)
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerCounter.text = playerCount + "/" + maxPlayersInRoom;
           
        }*/

        if (Player)
        {
            Player = photonView.IsMine;
        }
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

    //UNA MEJOR MANERA DE HACERLO PERO QUE TIENE EL MISMO RESILTADO, AS� QUE SI ES NECESARIO LO INTENTAR� IMPLEMENTAR EN OTRO MOMENTO 
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
