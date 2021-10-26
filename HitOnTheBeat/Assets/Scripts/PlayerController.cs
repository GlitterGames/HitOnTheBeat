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
    public Vector3 oldPos;
    public float secondsCounter = 0f;
    public float secondsToCount = 0.4f;
    private bool movimientoMarcado = false;
    #endregion

    // Start is called before the first frame update

    void Awake()
    {
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();

        typeAnt = FloorDetectorType.West;

        //Los viewId de Cada jugador se caracterizan por el número 1000 así sabemos de quien es este objeto.
        actualFloor = FindObjectOfType<PhotonInstanciate>().f[(photonView.ViewID/1000)-1];
        previousFloor = actualFloor;
        newPos = transform.position;
        oldPos = transform.position;
        fuerza = 0;

        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsSpecial", false);
        animator.SetBool("IsFalling", false);
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
            secondsCounter += Time.deltaTime;
            float step = speed * Time.deltaTime;
            if (secondsCounter >= secondsToCount)
                transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            if (animator.GetBool("IsJumping") && !animator.GetBool("IsFalling"))
            {
                Quaternion rotTarget = Quaternion.LookRotation(newPos - this.transform.position);
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, 250f * Time.deltaTime);
            }
            else if (animator.GetBool("IsFalling"))
            {
                Quaternion rotTarget = Quaternion.LookRotation(oldPos - this.transform.position);
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotTarget, 50f * Time.deltaTime);
            }

        }
        else
        {
            secondsCounter = 0;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsAttacking", false);
        }
    }

    //Se ejecuta cuando se realiza click en la pantalla.
    private void OnClick()
    {
        if (!photonView.IsMine) return;
        if (movimientoMarcado) return;
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

                if (targetFloor.Equals(actualFloor.GetNorth_west()))
                {
                    nextFloor = actualFloor.GetNorth_west();
                    typeAnt = FloorDetectorType.North_west;
                }
                else if (targetFloor.Equals(actualFloor.GetNorth_east()))
                {
                    nextFloor = actualFloor.GetNorth_east();
                    typeAnt = FloorDetectorType.North_east;
                }
                else if (targetFloor.Equals(actualFloor.GetWest()))
                {
                    nextFloor = actualFloor.GetWest();
                    typeAnt = FloorDetectorType.West;
                }
                else if (targetFloor.Equals(actualFloor.GetEast()))
                {
                    nextFloor = actualFloor.GetEast();
                    typeAnt = FloorDetectorType.East;
                }
                else if (targetFloor.Equals(actualFloor.GetSouth_west()))
                {
                    nextFloor = actualFloor.GetSouth_west();
                    typeAnt = FloorDetectorType.South_west;
                }
                else if (targetFloor.Equals(actualFloor.GetSouth_east()))
                {
                    nextFloor = actualFloor.GetSouth_east();
                    typeAnt = FloorDetectorType.South_east;
                }

                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    if (Ritmo.instance.TryMovePlayer())
                    {
                        movimientoMarcado = true;
                        photonView.RPC("RegisterMoveRPC", RpcTarget.MasterClient, (photonView.ViewID / 1000) - 1, nextFloor.row, nextFloor.index);
                    }
                }
            }
        }
    }

    #region Moviento
    [PunRPC]
    public void RegisterMoveRPC(int id, int row, int index)
    {
        gameManager.RegisterMovement(id, row, index);
    }

    public void Mover(Floor nextFloor)
    {
        fuerza++;
        photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
        photonView.RPC("MoverRPC", RpcTarget.All, nextFloor.row, nextFloor.index);
        photonView.RPC("MoverServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
    }
    public void Golpear()
    {
        photonView.RPC("GolpearRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void MoverRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        movimientoMarcado = false;
        previousFloor = actualFloor;
        actualFloor = nextFloor;
    }

    [PunRPC]
    private void MoverServerRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        animator.SetBool("IsJumping", true);
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
    }

    [PunRPC]
    private void GolpearRPC()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsAttacking", true);
    }

    [PunRPC]
    private void ColorearRPC(int row, int index)
    {
        SetNormalColor(actualFloor);
        SetAreaColor(gameManager.casillas[row][index]);
    }

    public bool Echar(FloorDetectorType type, int max)
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
            photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
            photonView.RPC("EcharRPC", RpcTarget.All, nextFloor.row, nextFloor.index);
            photonView.RPC("EcharServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
        }
        else
        {
            photonView.RPC("EcharMapaRPC", RpcTarget.All);
            photonView.RPC("EcharMapaServerRPC", RpcTarget.AllViaServer, pos.x, pos.z);
        }

        return echado;
    }

    [PunRPC]
    private void EcharRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        previousFloor = actualFloor;
        actualFloor = nextFloor;
    }

    [PunRPC]
    private void EcharServerRPC(int row, int index)
    {
        animator.SetBool("IsFalling", true);
        animator.SetBool("IsJumping", false);
        Floor nextFloor = gameManager.casillas[row][index];
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        oldPos = new Vector3(previousFloor.GetFloorPosition().x, transform.position.y, previousFloor.GetFloorPosition().z);
    }

    [PunRPC]
    private void EcharMapaRPC()
    {
        previousFloor = null;
        actualFloor = null;
    }

    [PunRPC]
    private void EcharMapaServerRPC(float x, float z)
    {
        animator.SetBool("IsFalling", true);
        animator.SetBool("IsJumping", false);
        newPos = new Vector3(x, transform.position.y, z);
    }
    #endregion

    #region Colores
    private void SetNormalColor(Floor f) {
        f.SetColor(f.GetColorN());
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.SetColor(floor.GetColorN());
        }
    }
    private void SetAreaColor(Floor f)
    {
        f.SetColor(GameManager.casillaAct);
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++) {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.SetColor(GameManager.casillaAdy);
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
        Floor[] casillasAdy = actualFloor.GetAdyacentes();
        foreach (Floor floorAdy in casillasAdy)
        {
            Floor[] casillasArea = floorAdy.GetAdyacentes();
            foreach (Floor floorArea in casillasArea)
            {
                casillas.Add(floorArea);
            }
        }
        return casillas;
    }

    private void SetRangeColor(HashSet<Floor> casillas)
    {
        actualFloor.SetColor(GameManager.casillaAct);

        foreach (Floor floor in casillas)
        {
            if (floor != null) floor.SetColor(GameManager.casillaAttack);
        }
    }
    #endregion

    #region RPC Calls
    [PunRPC]
    private void UpdatePlayersRPC()
    {
        gameManager.UpdatePlayers();
    }
    #endregion
}
