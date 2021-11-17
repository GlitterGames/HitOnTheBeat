using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    #region Enumerables
    public enum Tipo
    {
        BOXEADORA = 0,
        BOMBA = 1,
        FANTASMA = 2,
        TERREMOTO = 3
    }
    public enum Estado
    {
        NORMAL = 0,
        INVENCIBLE = 1,
        ULTIMATE = 2
    }
    public enum Power_Up
    {
        NORMAL = 0,
        ESCUDO = 1,
        RITMODUPLICADO = 2
    }
    #endregion

    #region Atributes
    public Tipo tipoPersonaje = Tipo.BOXEADORA;
    public Estado estadoActual = Estado.NORMAL;
    public Floor actualFloor;
    public Floor previousFloor;
    public FloorDetectorType floorDir;
    public int fuerza;
    public int fuerzaCinetica;
    public bool colision;
    public int fuerzaSinPulsar = 1;
    private InputController my_input;
    private GameManager gameManager;
    private Animator animator;
    private Rigidbody rb;
    public float speed;
    public Vector3 newPos;
    public Vector3 oldPos;
    public float secondsCounter = 0f;
    public float secondsToCount = 0.4f;
    public bool movimientoMarcado = false;
    private Vector3 pos = Vector3.zero;
    public Power_Up power = Power_Up.NORMAL;
    public float durationPowerUp = 5f;
    public Coroutine powerCoroutine = null;
    #endregion

    // Start is called before the first frame update

    void Awake()
    {
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();

        floorDir = FloorDetectorType.West;

        //Los viewId de Cada jugador se caracterizan por el número 1000 así sabemos de quien es este objeto.
        actualFloor = FindObjectOfType<PhotonInstanciate>().f[(photonView.ViewID / 1000) - 1];
        previousFloor = actualFloor;
        newPos = transform.position;
        oldPos = transform.position;
        fuerza = 0;
        fuerzaCinetica = 0;
        colision = false;

        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();

        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsSpecial", false);
        animator.SetBool("IsFalling", false);
    }

    void Start()
    {
        if (photonView.IsMine) StartCoroutine(PrimerPintado());
    }

    IEnumerator PrimerPintado()
    {
        yield return new WaitForSeconds(1);
        SetAreaColor(actualFloor);
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
            if ((animator.GetBool("IsJumping") || (animator.GetBool("IsAttacking"))) && !animator.GetBool("IsFalling"))
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
            if (transform.position == pos)
                photonView.RPC("UseGravityRPC", RpcTarget.All);
        }
    }

    //Se ejecuta cuando se realiza click en la pantalla.
    private void OnClick()
    {
        if (!photonView.IsMine) return;
        if (movimientoMarcado) return;
        if (Ritmo.instance.haFallado) return;
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
                    floorDir = FloorDetectorType.North_west;
                }
                else if (targetFloor.Equals(actualFloor.GetNorth_east()))
                {
                    nextFloor = actualFloor.GetNorth_east();
                    floorDir = FloorDetectorType.North_east;
                }
                else if (targetFloor.Equals(actualFloor.GetWest()))
                {
                    nextFloor = actualFloor.GetWest();
                    floorDir = FloorDetectorType.West;
                }
                else if (targetFloor.Equals(actualFloor.GetEast()))
                {
                    nextFloor = actualFloor.GetEast();
                    floorDir = FloorDetectorType.East;
                }
                else if (targetFloor.Equals(actualFloor.GetSouth_west()))
                {
                    nextFloor = actualFloor.GetSouth_west();
                    floorDir = FloorDetectorType.South_west;
                }
                else if (targetFloor.Equals(actualFloor.GetSouth_east()))
                {
                    nextFloor = actualFloor.GetSouth_east();
                    floorDir = FloorDetectorType.South_east;
                }

                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    movimientoMarcado = true;
                    photonView.RPC("RegisterClickRPC", RpcTarget.MasterClient, (photonView.ViewID / 1000) - 1,
                        nextFloor.row, nextFloor.index, floorDir);
                }
            }
        }
    }

    #region Moviento
    [PunRPC]
    public void RegisterClickRPC(int id, int row, int index, FloorDetectorType dir)
    {
        bool acierto = Ritmo.instance.TryMovePlayer();
        if (acierto)
        {
            gameManager.RegisterMovement(id, row, index, dir);
            photonView.RPC("AciertoRPC", RpcTarget.All);
        }
        else
        {
            photonView.RPC("DismarkPlayerRPC", photonView.Owner);
            photonView.RPC("FalloRPC", RpcTarget.All);
        }
        photonView.RPC("MarcarRitmoRPC", photonView.Owner, acierto);
    }

    [PunRPC]
    public void DismarkPlayerRPC()
    {
        movimientoMarcado = false;
    }

    [PunRPC]
    public void MarcarRitmoRPC(bool ritmoAcertado)
    {
        Ritmo ritmo = Ritmo.instance;
        ritmo.haPulsado = true;
        if (ritmoAcertado)
        {
            ritmo.SetColor(ritmo.colores.acierto);
        }
        else
        {
            ritmo.SetColor(ritmo.colores.fallo);
            ritmo.haFallado = true;
        }
    }

    [PunRPC]
    public void AciertoRPC()
    {
        fuerzaSinPulsar = 1;
    }

    [PunRPC]
    public void FalloRPC()
    {
        fuerza--;
        if (fuerza < 0) fuerza = 0;
    }

    [PunRPC]
    public void NoHaPulsadoRPC()
    {
        this.fuerza -= fuerzaSinPulsar;
        if (fuerza < 0) fuerza = 0;
        fuerzaSinPulsar++;
    }

    public void Golpear()
    {
        photonView.RPC("GolpearRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void GolpearRPC()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsAttacking", true);
    }

    public void Mover(Floor nextFloor, FloorDetectorType dir)
    {
        photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
        photonView.RPC("MoverRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir);
        photonView.RPC("MoverServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
    }

    [PunRPC]
    private void MoverRPC(int row, int index, FloorDetectorType dir)
    {
        fuerza++;
        if (Power_Up.RITMODUPLICADO == power) {
            fuerza++;
        }
        Floor nextFloor = gameManager.casillas[row][index];
        movimientoMarcado = false;
        previousFloor = actualFloor;
        actualFloor = nextFloor;
        floorDir = dir;
    }

    [PunRPC]
    private void MoverServerRPC(int row, int index)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        animator.SetBool("IsJumping", true);
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
    }

    [PunRPC]
    private void ColorearRPC(int row, int index)
    {
        SetNormalColor(actualFloor);
        SetAreaColor(gameManager.casillas[row][index]);
    }

    [PunRPC]
    private void UseGravityRPC()
    {
        rb.useGravity = true;
    }

    public bool EcharOne(FloorDetectorType dir, int max, bool moreThanTwo, bool notCinematic, bool sameFloor)
    {
        bool echado = false;
        Floor nextFloor = null;
        FloorDetectorType floordir = dir;
        if(notCinematic == true)
        {
            this.colision = true; //Se acaba de realizar colision por lo que no realiza la cinematica hasta la siguiente ejecucion
            nextFloor = actualFloor.GetFloor(floordir);
        }
        else
        if (sameFloor)
        {
            if (moreThanTwo) //SOLO DOS JUGADORES VAS A LA DIRECCIÓN INVERSA DE TU SITUACIÓN
            {
                nextFloor = actualFloor.GetInverseFloor(floordir);
            }
            else
            {
                nextFloor = actualFloor.GetInverseFloor(floordir); //MÁS DE UN JUGADOR LA DIRECCIÓN ES LA INVERSA A LA TUYA
            }
        }
        else
        {
            Debug.LogWarning("EN EL MISMO SITIO"); 
            nextFloor = previousFloor.GetFloor(floordir); //MISMO FLOOR TIENES QUE PILLAR EL INVERSO DE TU PREVIOUS FLOOR
        }
        if (max > 0)
        {
            fuerzaCinetica = max - 1; 
        }
        if (nextFloor == null)
        {
            Floor inverse = actualFloor.GetInverseFloor(floordir);
            Vector3 diferencia = new Vector3(actualFloor.GetFloorPosition().x - inverse.GetFloorPosition().x, 0f, actualFloor.GetFloorPosition().z - inverse.GetFloorPosition().z);
            pos = new Vector3(actualFloor.GetFloorPosition().x + diferencia.x, transform.position.y, actualFloor.GetFloorPosition().z + diferencia.z);
            echado = true;
        }
        if (!echado)
        {
            Debug.LogWarning("EN EL MAPA");
            photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
            if (sameFloor) {photonView.RPC("EcharRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir); }
            //En el caso en el que no la colision no fuera en la misma casilla la anterior y la posterior casillas serán la misma
            else { photonView.RPC("EcharNotSameFloorRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir); }
            photonView.RPC("EcharServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
        }
        else
        {
            Debug.LogWarning("SIN EL MAPA");
            photonView.RPC("EcharMapaRPC", RpcTarget.All);
            photonView.RPC("EcharMapaServerRPC", RpcTarget.AllViaServer, pos.x, pos.z);
        }
        return echado;
    }
    public bool Echar(FloorDetectorType dir, int max)
    {
        bool echado = false;
        Floor nextFloor = null;
        for (int i = 0; i < max && !echado; i++)
        {
            nextFloor = actualFloor.GetFloor(dir);
            if (nextFloor != null)
            {
                //Se actualiza no es la última iteración.
                //Si es la última iteración se cambiará vía RPC.
                if (i < max - 1)
                {
                    previousFloor = actualFloor;
                    actualFloor = nextFloor;
                }
            }
            else
            {
                //ENVIARLE A LA POSICION DE LA CASILLA "NULL" 
                Floor inverse = actualFloor.GetInverseFloor(dir);
                Vector3 diferencia = new Vector3(actualFloor.GetFloorPosition().x - inverse.GetFloorPosition().x, 0f, actualFloor.GetFloorPosition().z - inverse.GetFloorPosition().z);
                pos = new Vector3(actualFloor.GetFloorPosition().x + diferencia.x, transform.position.y, actualFloor.GetFloorPosition().z + diferencia.z);
                echado = true;
            }
        }
        if (!echado)
        {
            photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
            photonView.RPC("EcharRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir);
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
    private void EcharRPC(int row, int index, FloorDetectorType dir)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        previousFloor = actualFloor;
        actualFloor = nextFloor;
        floorDir = dir;
    }
    [PunRPC]
    private void EcharNotSameFloorRPC(int row, int index, FloorDetectorType dir)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        actualFloor = nextFloor;
        previousFloor = actualFloor;
        floorDir = dir;
    }

    [PunRPC]
    private void EcharServerRPC(int row, int index)
    {
        animator.SetBool("IsFalling", true);
        animator.SetBool("IsJumping", false);
        Floor nextFloor = gameManager.casillas[row][index];
        oldPos = newPos;
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
    }

    [PunRPC]
    private void EcharMapaRPC()
    {
        previousFloor = actualFloor;
        actualFloor = null;
    }

    [PunRPC]
    private void EcharMapaServerRPC(float x, float z)
    {
        animator.SetBool("IsFalling", true);
        animator.SetBool("IsJumping", false);
        oldPos = newPos;
        newPos = new Vector3(x, transform.position.y, z);
    }
	
	public void Caer() {
        photonView.RPC("EcharMapaRPC", RpcTarget.All);
        photonView.RPC("EcharMapaServerRPC", RpcTarget.AllViaServer, pos.x, pos.z);
    }
    public void GetPowerUp()
    {
        if (Power_Up.NORMAL != this.power) return; //LO PILLAS PERO NO TE AFECTA YA QUE YA POSEES UN POWER 
        Floor.Type t = actualFloor.GetPower();
        SetPowerUp(actualFloor, Floor.Type.Vacio);
        powerCoroutine = StartCoroutine(PowerUp());
        photonView.RPC("GetPowerUpRPC", RpcTarget.All, t);
    }
    
    [PunRPC]
    private void GetPowerUpRPC(Floor.Type t)
    {
        switch (t)
        {
            case Floor.Type.RitmoDuplicado:
                this.power = Power_Up.RITMODUPLICADO;
                break;
            case Floor.Type.Escudo:
                this.power = Power_Up.ESCUDO;
                break;
        }
    }
    public void UsePowerUp()
    {
        photonView.RPC("UsePowerUpRPC", RpcTarget.All);
    }
    [PunRPC]
    private void UsePowerUpRPC()
    {
        this.power = Power_Up.NORMAL;
    }

    private IEnumerator PowerUp()
    {
        yield return new WaitForSeconds(durationPowerUp);
        photonView.RPC("UsePowerUpRPC", RpcTarget.All);
    }
    public void SetPowerUp(Floor f, Floor.Type type)
    {
        photonView.RPC("SetPowerUpRPC", RpcTarget.AllViaServer, f.row, f.index, type);
    }
    [PunRPC]
    private void SetPowerUpRPC(int row, int index, Floor.Type type)
    {
        gameManager.casillas[row][index].SetPower(type);    
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
    private void SetPlayerCamera(int id)
    {
        FindObjectOfType<VirtualCameraController>().SetTarget(PhotonView.Find(id).transform);
    }

    [PunRPC]
    private void UpdatePlayersRPC()
    {
        gameManager.UpdatePlayers();
    }

    [PunRPC]
    private void DoExitPlayer()
    {
        FindObjectOfType<RemovePlayers>().ExitPlayer();
    }

    [PunRPC]
    private void DoUpdateWinner(int num)
    {
        FindObjectOfType<PlayerSelector>().playerWinner = num;
    }
    #endregion

}
