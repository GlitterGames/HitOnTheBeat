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
    public Floor suelo;
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
    private photonInstanciate photon;
    public GameObject vacio;
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
        //photon = GameObject.Find("@photonInstanciate");
        //suelo = vacio.GetComponent<f>();
        //photon = GetComponent<photonInstanciate>();
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();
        //suelo = photon.f;
        //vacio = GameObject.Find("@photonInstanciate");
        photon = GameObject.Find("@photonInstanciate").GetComponent<photonInstanciate>();
        suelo = photon.f;
        transform.position = new Vector3(suelo.transform.position.x, 1.062631f, suelo.transform.position.z);
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

                if (targetFloor.Equals(suelo.getNorth_west()))
                {
                    nextFloor = suelo.getNorth_west();
                }
                else if (targetFloor.Equals(suelo.getNorth_east()))
                {
                    nextFloor = suelo.getNorth_east();
                }
                else if (targetFloor.Equals(suelo.getWest()))
                {
                    nextFloor = suelo.getWest();
                }
                else if (targetFloor.Equals(suelo.getEast()))
                {
                    nextFloor = suelo.getEast();
                }
                else if (targetFloor.Equals(suelo.getSouth_west()))
                {
                    nextFloor = suelo.getSouth_west();
                }
                else if (targetFloor.Equals(suelo.getSouth_east()))
                {
                    nextFloor = suelo.getSouth_east();
                }

                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    setNormalColor(suelo);
                    transform.position = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
                    suelo = nextFloor;
                    setColor(suelo);
                }
            }
        }
    }

    private void setNormalColor(Floor f)
    {
        f.setColor(f.getColorN());
        Floor[] casillasAdy = f.getAdyacentes();
        Floor floor;
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            f = casillasAdy[i];
            if (f != null) f.setColor(f.getColorN());
        }
    }
    private void setColor(Floor f)
    {
        f.setColor(GameManager.casillaAct);
        Floor[] casillasAdy = f.getAdyacentes();
        Floor floor;
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            f = casillasAdy[i];
            if (f != null) f.setColor(GameManager.casillaAdy);
        }
    }
}
