using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
public class PlayerController : MonoBehaviourPun
{
    #region Enumerables
    public enum Tipo
    {
        BOXEADORA = 0,
        FANTASMA = 1,
        TERREMOTO = 2,
        BOMBA = 3
    }
    public enum Estado
    {
        NORMAL = 0,
        INVENCIBLE = 1,
        ULTIMATE = 2
    }
    #endregion

    #region Atributes
    public Tipo tipoPersonaje = Tipo.BOXEADORA;
    public Estado estadoActual = Estado.NORMAL;
    public Floor actualFloor;
    public Floor previousFloor;
    public FloorDetectorType typeAnt;
    public int fuerza;
    public GameObject playerAvatar;
    private InputController my_input;
    private PhotonInstanciate photon;
    private GameManager gameManager;
    private Animator animator;
    public float speed;
    public Vector3 newPos;
    #endregion

    // Start is called before the first frame update

    void Awake()
    {
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();

        typeAnt = FloorDetectorType.West;

        //Los viewId de Cada jugador se caracterizan por el número 1000 así sabemos de quien es este objeto.
        actualFloor = FindObjectOfType<PhotonInstanciate>().f[(photonView.ViewID-1000)/1000];
        previousFloor = actualFloor;
        newPos = transform.position;

        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsSpecial", false);
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

    void Update()
    {
        if (transform.position != newPos)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            transform.LookAt(newPos);
        }
    }

    //Se ejecuta cuando se realiza click en la pantalla.
    private void OnClick()
    {
        if (!photonView.IsMine) return;
        if (transform.position != newPos) return;
        if (estadoActual == Estado.ULTIMATE && tipoPersonaje == Tipo.BOMBA) return;

        Vector3 screenPos = my_input.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Click realizado sobre casilla.
            Floor targetFloor = hit.transform.GetComponent<Floor>();
            if (targetFloor)
            {
                Floor nextFloor = null;

                if (targetFloor.Equals(actualFloor.getNorth_west()))
                {
                    nextFloor = actualFloor.getNorth_west();
                    typeAnt = FloorDetectorType.North_west;
                }
                else if (targetFloor.Equals(actualFloor.getNorth_east()))
                {
                    nextFloor = actualFloor.getNorth_east();
                    typeAnt = FloorDetectorType.North_east;
                }
                else if (targetFloor.Equals(actualFloor.getWest()))
                {
                    nextFloor = actualFloor.getWest();
                    typeAnt = FloorDetectorType.West;
                }
                else if (targetFloor.Equals(actualFloor.getEast()))
                {
                    nextFloor = actualFloor.getEast();
                    typeAnt = FloorDetectorType.East;
                }
                else if (targetFloor.Equals(actualFloor.getSouth_west()))
                {
                    nextFloor = actualFloor.getSouth_west();
                    typeAnt = FloorDetectorType.South_west;
                }
                else if (targetFloor.Equals(actualFloor.getSouth_east()))
                {
                    nextFloor = actualFloor.getSouth_east();
                    typeAnt = FloorDetectorType.South_east;
                }

                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    Mover(nextFloor);
                }
            }
        }
    }

    #region Moviento
    public void Mover(Floor nextFloor)
    {
        photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
        photonView.RPC("MoverRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
    }

    [PunRPC]
    private void MoverRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        previousFloor = actualFloor;
        actualFloor = nextFloor;
    }

    [PunRPC]
    private void ColorearRPC(int row, int index)
    {
        SetNormalColor(actualFloor);
       SetAreaColor(gameManager.casillas[row][index]);
    }

    public bool echar(FloorDetectorType type, int max)
    {
        bool echado = false;
        Vector3 pos = Vector3.zero;
        Floor nextFloor = null;
        for (int i = 0; i < max && !echado; i++)
        {
            nextFloor = actualFloor.GetFloor(type);
            if (nextFloor != null)
            {
                previousFloor = actualFloor;
                actualFloor = nextFloor;
                echado = false;
            }
            else
            {
                //ENVIARLE A LA POSICION DE LA CASILLA "NULL" 
                Floor inverse = actualFloor.GetInverseFloor(type);
                Vector3 diferencia = new Vector3(actualFloor.GetFloorPosition().x - inverse.GetFloorPosition().x, 0f, actualFloor.GetFloorPosition().z - inverse.GetFloorPosition().z);
                pos = new Vector3(actualFloor.GetFloorPosition().x + diferencia.x, transform.position.y, actualFloor.GetFloorPosition().z + diferencia.z);
                echado = true;
            }
        }
        if (!echado)
        {
            photonView.RPC("EcharRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
        }
        else
        {
            photonView.RPC("EcharMapaRPC", RpcTarget.AllViaServer, pos.x, pos.z);
        }

        return echado;
    }

    [PunRPC]
    private void EcharRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        previousFloor = actualFloor;
        actualFloor = nextFloor;
    }

    [PunRPC]
    private void EcharMapaRPC(float x, float z)
    {
        newPos = new Vector3(x, transform.position.y, z);
        previousFloor = null;
        actualFloor = null;
    }

    public void eliminarJugador(Floor nextFloor)
    {
        Debug.Log("Me sali de la pista");
    }
    #endregion

    #region Colores
    private void SetNormalColor(Floor f) {
        f.setColor(f.getColorN());
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(floor.getColorN());
        }
    }
    private void SetAreaColor(Floor f)
    {
        f.setColor(GameManager.casillaAct);
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++) {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(GameManager.casillaAdy);
        }
    }
    #endregion

    #region  Ultimates

    /*public void StartUltimate()
    {

    }

    public void PerformUltimate()
    {
        switch (tipoPersonaje)
        {
            case Tipo.BOMBA:

                break;
        }
    }*/

    private HashSet<Floor> GetFloorAreaRange()
    {
        HashSet<Floor> casillas = new HashSet<Floor>();
        Floor[] casillasAdy = actualFloor.getAdyacentes();
        foreach (Floor floorAdy in casillasAdy)
        {
            Floor[] casillasArea = floorAdy.getAdyacentes();
            foreach (Floor floorArea in casillasArea)
            {
                casillas.Add(floorArea);
            }
        }
        return casillas;
    }

    private void SetRangeColor(HashSet<Floor> casillas)
    {
        actualFloor.setColor(GameManager.casillaAct);

        foreach (Floor floor in casillas)
        {
            if (floor != null) floor.setColor(GameManager.casillaAttack);
        }
    }

    #endregion
}
