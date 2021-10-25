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
    private photonInstanciate photon;
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
    }

    void Start()
    {
        animator = GetComponent<Animator>();
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
        if (!photonView.IsMine) return;

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
                Debug.Log("Click realizado");
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
                    mover(nextFloor);
                }
            }
        }
    }

    #region Moviento
    public void mover(Floor nextFloor)
    {
        setNormalColor(actualFloor);
        this.photonView.RPC("MoverRPC", RpcTarget.AllViaServer, nextFloor.photonView.ViewID);
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        setAreaColor(nextFloor);
    }

    [PunRPC]
    private void MoverRPC(int id)
    {
        Floor nextFloor = PhotonView.Find(id).GetComponent<Floor>();
        previousFloor = actualFloor;
        actualFloor = nextFloor;
    }

    public bool echar(FloorDetectorType type) {
        Floor nextFloor = actualFloor.GetFloor(type);
        if (nextFloor != null)
        {
            Debug.Log("Me tenía que mover");
            transform.position = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
            previousFloor = actualFloor;
            actualFloor = nextFloor;
            return false;
        }
        else {
            Debug.Log("Me tengo que eliminar");
            //ENVIARLE A LA POSICION DE LA CASILLA "NULL" 
            Floor inverse = actualFloor.GetInverseFloor(type);
            Vector3 diferencia = new Vector3(actualFloor.GetFloorPosition().x- inverse.GetFloorPosition().x, 0f, actualFloor.GetFloorPosition().z-inverse.GetFloorPosition().z);
            transform.position = new Vector3(actualFloor.GetFloorPosition().x+diferencia.x, transform.position.y, actualFloor.GetFloorPosition().z+diferencia.z);
            //ANIMACION DE ELIMINAR AL JUEGADOR
            eliminarJugador(actualFloor);
            actualFloor = null;
            return true;
        }
    }
    public void eliminarJugador(Floor nextFloor)
    {
        Debug.Log("Me sali de la pista");
    }
    #endregion

    #region Colores
    private void setNormalColor(Floor f) {
        f.setColor(f.getColorN());
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(floor.getColorN());
        }
    }
    private void setAreaColor(Floor f)
    {
        f.setColor(GameManager.casillaAct);
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++) {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(GameManager.casillaAdy);
        }
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

    #region  Ultimates

    /*public void StartUltimate()
    {

    }

    public void PerformUltimate()
    {
        switch ()
        {

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
    #endregion
}
