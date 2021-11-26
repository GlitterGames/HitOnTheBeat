using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    #region Enumerables
    public enum Ultimate
    {
        MEGA_PUNCH = 0,
        BOMBA_COLOR = 1,
        INVISIBILITY = 2
    }
    public enum Estado
    {
        NORMAL = 0,
        COLOR_RANGE = 1,
        ULTIMATE = 2,
        EXECUTING = 3
    }
    public enum Power_Up
    {
        NORMAL = 0,
        ESCUDO = 1,
        RITMODUPLICADO = 2
    }
    #endregion

    #region Atributes
    //Constantes
    public static int MAX_STRENGHT = 10;
    public int ULTIMATE_MAX_BEAT_DURATION = 15;

    //Editor
    public Ultimate tipoUltimate = Ultimate.MEGA_PUNCH;
    public int tipoSkin;
    [HideInInspector]
    public Estado estadoActual = Estado.NORMAL;
    public Floor actualFloor;
    public Floor previousFloor;
    public FloorDetectorType floorDir;
    public GameObject escudo;
    public GameObject hit;
    public GameObject X2;
    //Último personaje que me ha golpeado
    public int golpeador;
    public EfectosSonido efectosSonido;
    private int puesto = 0;

    private int m_fuerza;
    public int Fuerza
    {
        get
        {
            if (estadoActual == Estado.EXECUTING && tipoUltimate == Ultimate.MEGA_PUNCH) return MAX_STRENGHT * 2;
            else return m_fuerza;
        }
        set
        {
            if (!(estadoActual == Estado.EXECUTING && tipoUltimate == Ultimate.MEGA_PUNCH))
            {
                m_fuerza = value;
                if (m_fuerza > MAX_STRENGHT) m_fuerza = MAX_STRENGHT;
                else if (m_fuerza < 0) m_fuerza = 0;
                //Se actualiza la propia interfaz
                if (photonView.IsMine)
                {
                    HUDManager.instance.SetFuerza(m_fuerza);
                }
            }
            else
            {
                //Se actualiza la propia interfaz
                if (photonView.IsMine)
                {
                    HUDManager.instance.SetFuerza(MAX_STRENGHT);
                }
            }
        }
    }
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
    private Power_Up m_power = Power_Up.NORMAL;
    public Power_Up Power
    {
        get
        {
            return m_power;
        }
        set
        {
            m_power = value;
            //Se actualiza la propia interfaz
            if (photonView.IsMine)
            {
                HUDManager.instance.SetPowerUp((int) m_power);
            }
        }
    }
    public float durationPowerUp = 15f;
    public Coroutine powerCoroutine;

    //stats
    public int hitsStats = 0;  //Jugadores que ha golpeado
    public int jumpStats = 0;  //Saltos que ha dado
    public int pushStats = 0;  //Veces que ha sido golpeado
    public int killsStats = 0; //Jugadores que ha sacado del ring
    public float AnimVelocity = 1;
    public float AnimVelocityCollision = 0.7f;
    #endregion

    // Start is called before the first frame update

    void Awake()
    {
        powerCoroutine = null;
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();

        floorDir = FloorDetectorType.West;

        //Los viewId de Cada jugador se caracterizan por el número 1000 así sabemos de quien es este objeto.
        actualFloor = FindObjectOfType<PhotonInstanciate>().f[GetIdPlayer()];
        previousFloor = actualFloor;
        newPos = transform.position;
        oldPos = transform.position;
        Fuerza = 0;
        fuerzaCinetica = 0;
        colision = false;
        golpeador = -1;

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
        efectosSonido = GetComponent<EfectosSonido>();
        if (photonView.IsMine) StartCoroutine(PrimerPintado());
        SetEscudo(false);
    }

    IEnumerator PrimerPintado()
    {
        yield return new WaitForSeconds(0.7f);
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
        //Si está activando su ultimate no puede moverse.
        if (estadoActual == Estado.ULTIMATE) return;

        Vector3 screenPos = my_input.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Click realizado sobre casilla.
            Floor targetFloor = hit.transform.GetComponent<Floor>();
            if (targetFloor)
            {
                //Si se está permitiendo el movimiento.
                if (estadoActual != Estado.COLOR_RANGE) {
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
                    //Si ha sido encontrado un suelo valido o 
                    if (nextFloor != null)
                    {
                        movimientoMarcado = true;
                        photonView.RPC("RegisterClickRPC", RpcTarget.MasterClient, GetIdPlayer(),
                                nextFloor.row, nextFloor.index, floorDir);
                    }
                }
                //Si es XXColor y está marcando la posición de la bomba.
                else
                {
                    if (GetFloorAreaRange(actualFloor).Contains(targetFloor))
                    {
                        photonView.RPC("ChangeStateRPC", RpcTarget.All, Estado.ULTIMATE);
                        SetBombaColorUltimate(targetFloor);
                    }
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
        Fuerza--;
    }

    public void SetFuerza(int fuerza)
    {
        photonView.RPC("SetFuerzaRPC", RpcTarget.All, fuerza);
    }

    [PunRPC]
    public void SetFuerzaRPC(int fuerza)
    {
        Fuerza = fuerza;
    }

    [PunRPC]
    public void NoHaPulsadoRPC()
    {
        this.Fuerza -= fuerzaSinPulsar;
        if (Fuerza < 0) Fuerza = 0;
        fuerzaSinPulsar++;
    }

    public void Golpear()
    {
        photonView.RPC("GolpearRPC", RpcTarget.AllViaServer);
        photonView.RPC("HitRPC", photonView.Owner);
        fuerzaCinetica = 0;
    }

    public void SetPuesto(int puesto)
    {
        photonView.RPC("SetPuestoRPC", photonView.Owner, puesto);
    }

    [PunRPC]
    public void SetPuestoRPC(int puesto)
    {
        this.puesto = puesto;
    }

   [PunRPC]
    private void GolpearRPC()
    {
        //Si la boxeadora realiza un golpe, gasta la ultimate.
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsAttacking", true);
        if (estadoActual == Estado.EXECUTING && tipoUltimate == Ultimate.MEGA_PUNCH)
        {
            ChangeStateRPC(Estado.NORMAL);
            HUDManager.instance.durationUltimate = 1;
        }
        if (estadoActual == Estado.EXECUTING && tipoUltimate == Ultimate.INVISIBILITY)
        {
            ChangeStateRPC(Estado.NORMAL);
            HUDManager.instance.durationUltimate = 1;
        }
        StartCoroutine(HitCoroutine(1f));
    }
    public IEnumerator HitCoroutine(float t)
    {
        SetHit(true);
        yield return new WaitForSeconds(t);
        SetHit(false);
    }
    public void Mover(Floor nextFloor, FloorDetectorType dir)
    {
        photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
        photonView.RPC("JumpRPC", photonView.Owner);
        photonView.RPC("MoverRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir);
        photonView.RPC("MoverServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
    }

    [PunRPC]
    private void MoverRPC(int row, int index, FloorDetectorType dir)
    {
        Fuerza++;
        if (Power_Up.RITMODUPLICADO == Power) {
            Fuerza++;
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
    private void NoColorearRPC()
    {
        SetNormalColor(actualFloor);
    }

    [PunRPC]
    private void UseGravityRPC()
    {
        rb.useGravity = true;
    }
    public void StopCoroutinePowers()
    {
        if (powerCoroutine != null) StopCoroutine(powerCoroutine);
    }
    public bool EcharOne(FloorDetectorType dir, int max, bool moreThanTwo, bool notCinematic, bool sameFloor, int golpeador)
    {
        bool echado = false;
        Floor nextFloor = null;
        if(notCinematic == true)
        {
            //Se vuelve a ver al fantasma cuando recibe un golpe.
            if (estadoActual == Estado.EXECUTING && tipoUltimate == Ultimate.INVISIBILITY)
            {
                ChangeStateRPC(Estado.NORMAL);
                HUDManager.instance.durationUltimate = 1;
            }
            photonView.RPC("PushRPC", photonView.Owner);
            if(estadoActual==Estado.ULTIMATE && tipoUltimate == Ultimate.BOMBA_COLOR) CancelUltimate();
            this.colision = true; //Se acaba de realizar colision por lo que no realiza la cinematica hasta la siguiente ejecucion
            fuerzaCinetica = 0;
            this.golpeador = golpeador;
            Debug.LogWarning("GOLPEADOR "+this.golpeador);
        }
        if (!notCinematic) {
            nextFloor = actualFloor.GetFloor(dir);
        } 
        else if (sameFloor)
        {
            //Si hay dos jugadores se coge el floor de la dirección de mi oponente
            //Si hay más de dos se coge la inversa de mi dirección
            if (moreThanTwo) 
            {
                //En caso de que tengamos más de un jugador mi nueva dirección sera
                //la inversa de la que tenía para las fuerza cinética
                dir = actualFloor.GetInverseDireccion(dir);
                nextFloor = actualFloor.GetFloor(dir);
            }
            else
            {
                nextFloor = actualFloor.GetFloor(dir); 
            }
        }
        //Si estan en el aire, es decir no estan en la misma casilla
        //Se cogera la dirección del jugador opuesto
        else
        {
            nextFloor = previousFloor.GetFloor(dir); 
        }
        //En caso de que haya más iteracciones por hacer, estas se pondrán como fuerzaCinética
        //y se le restará una cada iteración hasta que sea mi fuerza cinética de 0
        if (max > 0)
        {
            fuerzaCinetica = max - 1; 
        }
        if (nextFloor == null)
        {
            Floor inverse = actualFloor.GetInverseFloor(dir);
            Vector3 diferencia = new Vector3(actualFloor.GetFloorPosition().x - inverse.GetFloorPosition().x, 0f, actualFloor.GetFloorPosition().z - inverse.GetFloorPosition().z);
            pos = new Vector3(actualFloor.GetFloorPosition().x + diferencia.x, transform.position.y, actualFloor.GetFloorPosition().z + diferencia.z);
            echado = true;
        }
        if (!echado)
        {
            photonView.RPC("ColorearRPC", photonView.Owner, nextFloor.row, nextFloor.index);
            //if (sameFloor) {photonView.RPC("EcharRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir); }
            //En el caso en el que la colision no fuera en la misma casilla la anterior y la posterior casillas serán la misma
            //else {
            photonView.RPC("EcharRPC", RpcTarget.All, nextFloor.row, nextFloor.index, dir); 
            //}
            photonView.RPC("EcharServerRPC", RpcTarget.AllViaServer, nextFloor.row, nextFloor.index);
        }
        else
        {
            if (this.golpeador != -1)
            {
                gameManager.jugadores[this.golpeador].Kill();
                Debug.LogWarning("Si " + gameManager.jugadores[this.golpeador].killsStats + " Tipo:" + gameManager.jugadores[this.golpeador].tipoUltimate.ToString());
            }
            else { Debug.LogWarning("No se ha podido hacer"); }
            photonView.RPC("NoColorearRPC", photonView.Owner);
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
        Debug.LogWarning("DIR: " +dir.ToString());
        floorDir = dir;
    }
    [PunRPC]
    private void EcharNotSameFloorRPC(int row, int index, FloorDetectorType dir)
    {
        Floor nextFloor = gameManager.casillas[row][index];
        previousFloor = actualFloor;
        actualFloor = nextFloor;
        floorDir = dir;
    }

    [PunRPC]
    private void EcharServerRPC(int row, int index)
    {
        if (animator.GetBool("IsJumping"))
        {
            StartCoroutine(AnimationsUpdate(row, index));
        }
        else
        {
            animator.SetBool("IsFalling", true);
            Floor nextFloor = gameManager.casillas[row][index];
            oldPos = newPos;
            newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        }
    }

    IEnumerator AnimationsUpdate(int row, int index)
    {
        yield return new WaitForSeconds(AnimVelocityCollision);
        if(actualFloor) SetAreaColor(actualFloor);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", true);
        Floor nextFloor = gameManager.casillas[row][index];
        oldPos = newPos;
        newPos = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
    }

    [PunRPC]
    private void EcharMapaRPC()
    {
        SetNormalColor(actualFloor);
        previousFloor = actualFloor;
        actualFloor = null;
    }

    [PunRPC]
    private void EcharMapaServerRPC(float x, float z)
    {
        if (animator.GetBool("IsJumping"))
        {
            StartCoroutine(AnimationsUpdate(x, z));
        }
        else
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
            oldPos = newPos;
            newPos = new Vector3(x, transform.position.y, z);
        }
    }
    IEnumerator AnimationsUpdate(float x, float z)
    {
        yield return new WaitForSeconds(AnimVelocityCollision);
        animator.SetBool("IsFalling", true);
        animator.SetBool("IsJumping", false);
        oldPos = newPos;
        newPos = new Vector3(x, transform.position.y, z);
    }

    public void Caer() {
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        pos.z = transform.position.z;
        photonView.RPC("EcharMapaRPC", RpcTarget.All);
        photonView.RPC("EcharMapaServerRPC", RpcTarget.AllViaServer, transform.position.x, transform.position.z);
    }
    #endregion

    #region Power Ups
    public void GetPowerUp()
    {
        //Si ya tienes un power up no puedes tener otro, pero el que
        //estaba se pone a 0
        efectosSonido.PlayEffect(0);
        if (Power_Up.NORMAL != this.Power) {
            SetPowerUpFloor(actualFloor, Floor.Type.Vacio);
            return;
        }
        Floor.Type t = actualFloor.GetPower();
        SetPowerUpFloor(actualFloor, Floor.Type.Vacio);
        powerCoroutine = gameManager.StartCoroutine(this);
        photonView.RPC("GetPowerUpRPC", RpcTarget.All, t);
    }
    
    
    [PunRPC]
    private void GetPowerUpRPC(Floor.Type t)
    {
        switch (t)
        {
            case Floor.Type.RitmoDuplicado:
                Debug.Log("Poniendo mi power a X2");
                this.Power = Power_Up.RITMODUPLICADO;
                SetX2(true);
                break;
            case Floor.Type.Escudo:
                Debug.Log("Poniendo mi power a ESCUDO");
                this.Power = Power_Up.ESCUDO;
                SetEscudo(true);
                break;
        }
    }
    //Escudo y Hit
    public void SetEscudo(bool activated)
    {
        photonView.RPC("EscudoRPC", RpcTarget.AllViaServer, activated);
    }
    public void SetX2(bool activated)
    {
        photonView.RPC("X2RPC", RpcTarget.AllViaServer, activated);
    }
    public void SetHit(bool activated)
    {
        photonView.RPC("HitRPC", RpcTarget.AllViaServer, activated);
        HUDManager.instance.UltimateCharge++;
    }
    [PunRPC]
    public void EscudoRPC(bool activated)
    {
        escudo.SetActive(activated);
    }
    [PunRPC]
    public void HitRPC(bool activated)
    {
        hit.SetActive(activated);
    }
    [PunRPC]
    public void X2RPC(bool activated)
    {
        X2.SetActive(activated);
    }
    //End of the use of the PowerUp
    public void EndPowerUp()
    {
        photonView.RPC("EndPowerUpRPC", RpcTarget.All);
    }
    [PunRPC]
    private void EndPowerUpRPC()
    {
        if (this.Power == Power_Up.RITMODUPLICADO) SetX2(false);
        if (this.Power == Power_Up.ESCUDO) SetEscudo(false);
        this.Power = Power_Up.NORMAL;
    }
    public IEnumerator PowerUp()
    {
        yield return new WaitForSeconds(durationPowerUp);
        photonView.RPC("EndPowerUpRPC", RpcTarget.All);
    }
    //FLOOR
    public void SetPowerUpFloor(Floor f, Floor.Type type)
    {
        bool cogido = false;
        if (f.Equals(actualFloor)) cogido = true;
        photonView.RPC("SetPowerUpFloorRPC", RpcTarget.AllViaServer, f.row, f.index, type, cogido);
        if(type==Floor.Type.Vacio && cogido) photonView.RPC("SetPowerUpColorRPC", photonView.Owner, f.row, f.index);
    }
    [PunRPC]
    private void SetPowerUpFloorRPC(int row, int index, Floor.Type type, bool cogido)
    {
        bool soyYo = photonView.IsMine;
        gameManager.casillas[row][index].SetPower(type, cogido, soyYo);    
    }
    [PunRPC]
    private void SetPowerUpColorRPC(int row, int index)
    {
        gameManager.casillas[row][index].GetComponent<Renderer>().material = gameManager.materiales.normal;
        gameManager.casillas[row][index].SetColor(gameManager.coloresEspeciales.actual);
    }

    #endregion

    #region DestroyRow
    public void SetColor(Floor f, Color c)
    {
        photonView.RPC("SetColorRPC", RpcTarget.AllViaServer, f.row, f.index, c.b, c.r, c.g);
    }
    public void SetColorBackground(int pos, Color c)
    {
        photonView.RPC("SetColorBackgroundRPC", RpcTarget.AllViaServer, pos, c.b, c.r, c.g);
    }
    public void SetColorN(Floor f, Color c)
    {
        photonView.RPC("SetColorNRPC", RpcTarget.AllViaServer, f.row, f.index, c.b, c.r, c.g);
    }
    public void SetPower(Floor f, Floor.Type type, bool b1, bool b2)
    {
        photonView.RPC("SetPowerRPC", RpcTarget.AllViaServer, f.row, f.index, type, b1, b2);
    }
    public void Cinematic(Floor f)
    {
        photonView.RPC("CinematicRPC", RpcTarget.AllViaServer, f.row, f.index);
    }
    public void CinematicBackground(int pos)
    {
        photonView.RPC("CinematicBackgroundRPC", RpcTarget.AllViaServer, pos);
    }
    public void Fall(Floor f)
    {
        photonView.RPC("FallRPC", RpcTarget.AllViaServer, f.row, f.index);
    }
    [PunRPC]
    public void SetColorRPC(int row, int index, float b, float r, float g)
    {
        Floor f = gameManager.casillas[row][index];
        Color c = new Color(r, g, b);
        f.SetColor(c);
    }
    [PunRPC]
    public void SetColorBackgroundRPC(int pos, float b, float r, float g)
    {
        Color c = new Color(r, g, b);
        gameManager.background[pos].GetComponent<Renderer>().material.color = c;
    }
    [PunRPC]
    public void SetColorNRPC(int row, int index, float b, float r, float g)
    {
        Floor f = gameManager.casillas[row][index];
        Color c = new Color(r, g, b);
        f.SetColorN(c);
    }
    [PunRPC]
    public void SetPowerRPC(int row, int index, Floor.Type type, bool b1, bool b2)
    {
        Floor f = gameManager.casillas[row][index];
        f.SetPower(type, b1, b2);
    }
    [PunRPC]
    public void CinematicRPC(int row, int index)
    {
        Floor f = gameManager.casillas[row][index];
        f.GetComponentInChildren<Rigidbody>().isKinematic = false;
        f.GetComponentInChildren<Rigidbody>().useGravity = true;
    }
    [PunRPC]
    public void CinematicBackgroundRPC(int pos)
    {
        gameManager.background[pos].SetActive(false);
    }
    [PunRPC]
    public void FallRPC(int row, int index)
    {
        Floor f = gameManager.casillas[row][index];
        f.gameObject.SetActive(false);
    }
    #endregion

    #region Colores
    private void SetNormalColor(Floor f) {
        f.SetColor(f.GetColorN());
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null)
            {
                if(floor.GetPower()==Floor.Type.Vacio) floor.SetColor(floor.GetColorN());
            }
        }
    }
    private void SetAreaColor(Floor f)
    {
        f.SetColor(gameManager.coloresEspeciales.actual);
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++) {
            Floor floor = casillasAdy[i];
            if (floor != null)
            {
                if(floor.GetPower()==Floor.Type.Vacio) floor.SetColor(gameManager.coloresEspeciales.adyacente);
            }
        }
    }

    public void SetAreaBombaColor(Floor f)
    {
        f.SetColor(gameManager.coloresBombaColor.selectedFloor0);
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null)
            {
                if (floor.GetPower() == Floor.Type.Vacio) floor.SetColor(gameManager.coloresBombaColor.selectedFloor1);
            }
        }
    }

    public void SetAreaBombaColorNormal(Floor f)
    {
        f.SetColor(f.GetColorN());
        Floor[] casillasAdy = f.GetAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null)
            {
                if (floor.GetPower() == Floor.Type.Vacio) floor.SetColor(floor.GetColorN());
            }
        }
    }
    #endregion

    #region  Ultimates
    //Se establecen las llamadas al master para registrar las ultimates,
    //cuando el master decida ejecutarlas se ejecutarán a la vez.
    public void StartUltimate()
    {
        
        switch (tipoUltimate)
        {
            case Ultimate.MEGA_PUNCH:
                efectosSonido.PlayEffect(1);
                photonView.RPC("ChangeStateRPC", RpcTarget.All, Estado.ULTIMATE);
                photonView.RPC("RegisterUltimateRPC", RpcTarget.MasterClient, GetIdPlayer(), tipoUltimate);
                break;
            case Ultimate.BOMBA_COLOR:
                photonView.RPC("ChangeStateRPC", RpcTarget.All, Estado.COLOR_RANGE);
                SetRangeColor(actualFloor);
                break;
            case Ultimate.INVISIBILITY:
                efectosSonido.PlayEffect(1);
                photonView.RPC("ChangeStateRPC", RpcTarget.All, Estado.ULTIMATE);
                photonView.RPC("RegisterUltimateRPC", RpcTarget.MasterClient, GetIdPlayer(), tipoUltimate);
                break;
        }
    }
    public void SetBombaColorUltimate(Floor f)
    {
        efectosSonido.PlayEffect(1);
        photonView.RPC("RegisterUltimateRPC", RpcTarget.MasterClient, GetIdPlayer(), tipoUltimate, f.row, f.index);
        SetRangeColorNormal(GetFloorAreaRange(actualFloor));
        SetAreaBombaColor(f);
    }

    [PunRPC]
    public void ChangeStateRPC(Estado state)
    {
        if (state == Estado.ULTIMATE) efectosSonido.PlayEffect(4);
        estadoActual = state;
    }

    public void CancelUltimate()
    {
        photonView.RPC("CancelUltimateRPC", photonView.Owner);
    }

    [PunRPC]
    public void CancelUltimateRPC()
    {
        if (estadoActual == Estado.COLOR_RANGE)
        {
            photonView.RPC("ChangeStateRPC", RpcTarget.All, Estado.NORMAL);
            SetRangeColorNormal(GetFloorAreaRange(actualFloor));
            SetAreaColor(actualFloor);
            HUDManager.instance.UltimateCharge = HUDManager.ULTIMATE_MAX_CHARGE;
        }
    }

    [PunRPC]
    public void RegisterUltimateRPC(int id, Ultimate type)
    {
        gameManager.RegisterUltimate(id, type);
    }

    [PunRPC]
    public void RegisterUltimateRPC(int id, Ultimate type, int row, int index)
    {
        gameManager.RegisterUltimate(id, type, row, index);
    }

    #region Mega Puño
    //Gestiona la ultimate Mega Punch.
    public void PerformMegaPunch(bool start)
    {
        photonView.RPC("PerformMegaPunchRPC", RpcTarget.All, start);
        photonView.RPC("PerformMegaPunchServerRPC", RpcTarget.AllViaServer, start);
    }

    //Lógica master del MegaPuño al iniciarla y al apagarla.
    [PunRPC]
    public void PerformMegaPunchRPC(bool start)
    {
        if (start)
        {
            ChangeStateRPC(Estado.EXECUTING);
        }
        else
        {
            ChangeStateRPC(Estado.NORMAL);
        }
    }

    //Animaciones del MegaPuño al iniciarla y al apagarla.
    [PunRPC]
    public void PerformMegaPunchServerRPC(bool start)
    {
        if (start)
        {
            animator.SetBool("IsSpecial", true);
            if (photonView.IsMine)
            {
                //Añadir efectos de la boxeadora.
                HUDManager.instance.durationUltimate = ULTIMATE_MAX_BEAT_DURATION;
            }
            GetComponent<MegaPunchManager>().SetSmoke(true);
        }
        else
        {
            //Parar efectos de la boxeadora.
            animator.SetBool("IsSpecial", false);
            GetComponent<MegaPunchManager>().SetSmoke(false);
        }
    }
    #endregion

    #region Bomba Color
    //Gestiona la ultimate Bomba Color cuando se selecciona dónde lanzará la bomba.
    public void PerformBombaColor(bool start, Floor f)
    {
        photonView.RPC("PerformBombaColorRPC", RpcTarget.All, start, f.row, f.index);
        photonView.RPC("PerformBombaColorServerRPC", RpcTarget.AllViaServer, start, f.row, f.index);
    }
    //Animación y ejecución de la Bomba Color.
    [PunRPC]
    public void PerformBombaColorRPC(bool start, int row, int index)
    {
        //Se lanza la bomba
        if (start)
        {
            ChangeStateRPC(Estado.EXECUTING);
            BombaColorManager bcm = GetComponent<BombaColorManager>();
            if (bcm)
            {
                bcm.StartAnimation(gameManager.casillas[row][index], ULTIMATE_MAX_BEAT_DURATION * Ritmo.instance.delay);
            }
            else
            {
                Debug.LogWarning("Esta Ultimate requiere del componente BombaColorManager");
            }
        }
        //Explota la bomba
        else
        {
            ChangeStateRPC(Estado.NORMAL);
            SetAreaBombaColorNormal(GetComponent<BombaColorManager>().target);
            photonView.RPC("BombaColorExplosion", RpcTarget.MasterClient, row, index);
        }
    }

    [PunRPC]
    public void BombaColorExplosion(int row, int index)
    {
        List<Floor> suelos = new List<Floor>();
        Floor target = gameManager.casillas[row][index];
        Floor[] adyacentes = target.adyacentes;
        if (target)
        {
            suelos.Add(target);
            foreach(Floor f in adyacentes)
            {
                if (f)
                {
                    suelos.Add(f);
                }
            }
        }
        foreach(PlayerController pc in gameManager.jugadores)
        {
            int fuerzaUsada = GetComponent<BombaColorManager>().fuerzaEmpleada;
            if (pc.actualFloor.Equals(target))
            {
                pc.EcharOne(pc.actualFloor.GetInverseDireccion(pc.floorDir), fuerzaUsada, true, false, false, -1);
            }
            else if (suelos.Contains(pc.actualFloor))
            {
                FloorDetectorType fdt;
                suelos.Remove(pc.actualFloor);
                if (pc.actualFloor.Equals(target.GetNorth_west()))
                {
                    fdt = FloorDetectorType.North_west;
                }
                else if (pc.actualFloor.Equals(target.GetNorth_east()))
                {
                    fdt = FloorDetectorType.North_east;
                }
                else if (pc.actualFloor.Equals(target.GetWest()))
                {
                    fdt = FloorDetectorType.West;
                }
                else if (pc.actualFloor.Equals(target.GetEast()))
                {
                    fdt = FloorDetectorType.East;
                }
                else if (pc.actualFloor.Equals(target.GetSouth_west()))
                {
                    fdt = FloorDetectorType.South_west;
                }
                else
                {
                    fdt = FloorDetectorType.South_east;
                }
                pc.EcharOne(fdt, fuerzaUsada, true, false, false, -1);
            }
        }
    }

    //Animación y ejecución de la Bomba Color.
    [PunRPC]
    public void PerformBombaColorServerRPC(bool start, int row, int index)
    {
        if (start)
        {
            animator.SetTrigger("Special");
            if (photonView.IsMine)
            {
                HUDManager.instance.durationUltimate = ULTIMATE_MAX_BEAT_DURATION;
                SetAreaColor(actualFloor);
            }
            Debug.Log("Bomba Color ejecutado");
        }
    }
    #endregion

    #region Invisibility
    //Gestiona la ultimate de Invisibilidad.
    public void PerformInvisibility(bool start)
    {
        photonView.RPC("PerformInvisibilityRPC", RpcTarget.All, start);
        photonView.RPC("PerformInvisibilityServerRPC", RpcTarget.AllViaServer, start);
    }

    //Animación y ejecución de la Invisibilidad.
    [PunRPC]
    public void PerformInvisibilityRPC(bool start)
    {
        if (start)
        {
            ChangeStateRPC(Estado.EXECUTING);
        }
        else
        {
            ChangeStateRPC(Estado.NORMAL);
        }
    }

    [PunRPC]
    public void PerformInvisibilityServerRPC(bool start)
    {
        if (start)
        {
            animator.SetBool("IsSpecial", true);
            //Si Frank pertecene al que ejecutó la ultimate.
            //Lo ve transparente.
            VisibilityManager vm = GetComponent<VisibilityManager>();
            if (vm)
            {
                if (photonView.IsMine)
                {
                    vm.Alpha = 0.5f;
                    HUDManager.instance.durationUltimate = ULTIMATE_MAX_BEAT_DURATION;
                }
                //Si Frank pertenece a otro jugador.
                //Deja de verlo.
                else
                {
                    vm.Visible = false;
                }
            }
            else
            {
                Debug.LogWarning("Esta Ultimate requiere del componente VisibilityManager");
            }
        }
        else
        { 
            animator.SetBool("IsSpecial", false);
            VisibilityManager vm = GetComponent<VisibilityManager>();
            if (vm)
            {
                if (photonView.IsMine)
                {
                    vm.Alpha = 1f;
                    HUDManager.instance.durationUltimate = ULTIMATE_MAX_BEAT_DURATION;
                }
                else
                {
                    vm.Visible = true;
                }
            }
            else
            {
                Debug.LogWarning("Esta Ultimate requiere del componente VisibilityManager");
            }
        }
    }
    #endregion

    public void UpdateUltimateTime()
    {
        photonView.RPC("UpdateUltimateTimeRPC", photonView.Owner);
    }

    [PunRPC]
    public void UpdateUltimateTimeRPC()
    {
        //Si está ejecutando una ultimate.
        if (HUDManager.instance.durationUltimate >= 0)
        {
            if (HUDManager.instance.durationUltimate > 0)
            {
                HUDManager.instance.durationUltimate -= 1;
            }
            //Se termina la ejecución de la ultimate.
            if(HUDManager.instance.durationUltimate == 0)
            {
                switch (tipoUltimate)
                {
                    case Ultimate.MEGA_PUNCH:
                        PerformMegaPunch(false);
                        break;
                    case Ultimate.BOMBA_COLOR:
                        PerformBombaColor(false, GetComponent<BombaColorManager>().target);
                        break;
                    case Ultimate.INVISIBILITY:
                        PerformInvisibility(false);
                        break;
                }
                HUDManager.instance.durationUltimate = -1;
            }
        }
    }

    private HashSet<Floor> GetFloorAreaRange(Floor target)
    {
        HashSet<Floor> casillas = new HashSet<Floor>();
        Floor[] casillasAdy = target.GetAdyacentes();
        foreach (Floor floorAdy in casillasAdy)
        {
            if (floorAdy)
            {
                Floor[] casillasArea = floorAdy.GetAdyacentes();
                foreach (Floor floorArea in casillasArea)
                {
                    if (floorArea) casillas.Add(floorArea);
                }
            }
        }
        return casillas;
    }

    private void SetRangeColor(Floor target)
    {
        HashSet<Floor> casillas = new HashSet<Floor>();
        casillas.Add(target);
        target.SetColor(gameManager.coloresBombaColor.anillo0);
        Floor[] casillasAdy = target.GetAdyacentes();
        foreach (Floor floorAdy in casillasAdy)
        {
            if (floorAdy)
            {
                Floor[] casillasArea = floorAdy.GetAdyacentes();
                foreach (Floor floorArea in casillasArea)
                {
                    if (floorArea)
                    {
                        if (casillas.Add(floorArea))
                        {
                            floorArea.SetColor(gameManager.coloresBombaColor.anillo2);
                        }
                    }
                }
                casillas.Add(floorAdy);
                floorAdy.SetColor(gameManager.coloresBombaColor.anillo1);
            }
        }
    }
    private void SetRangeColorNormal(HashSet<Floor> casillas)
    {
        foreach (Floor floor in casillas)
        {
            floor.SetColor(gameManager.color[floor.row]);
        }
    }
    #endregion

    #region Utiles
    public int GetIdPlayer()
    {
        return (photonView.ViewID / 1000) - 1;
    }

    #endregion

    #region Stats
    public void Kill()
    {
        photonView.RPC("KillRPC", photonView.Owner);
    }

    [PunRPC]
    public void KillRPC()
    {
        HUDManager.instance.UltimateCharge = HUDManager.ULTIMATE_MAX_CHARGE;
        killsStats++;
    }

    [PunRPC]
    public void HitRPC()
    {
        efectosSonido.PlayEffect(2);
        hitsStats++;
    }

    [PunRPC]
    public void PushRPC()
    {
        efectosSonido.PlayEffect(3);
        pushStats++;
    }

    [PunRPC]
    public void JumpRPC()
    {
        jumpStats++;
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
    public void DoEndGameRPC(string name, int num, int skin, int numBeats)
    {
        PlayerController mpc = FindObjectOfType<PhotonInstanciate>().my_player.GetComponent<PlayerController>();
        PlayerSelector ps = FindObjectOfType<PlayerSelector>();
        ps.puesto = mpc.puesto;
        ps.playerWinnerName = name;
        ps.playerWinner = num;
        ps.playerWinnerSkin = skin;
        ps.hitsStats = mpc.hitsStats;
        ps.killsStats = mpc.killsStats;
        ps.pushStats = mpc.pushStats;
        ps.jumpStats = mpc.jumpStats;
        if(numBeats<=0) ps.averageRhythmStats = 0;
        else ps.averageRhythmStats = mpc.jumpStats * 100 / numBeats;

        FindObjectOfType<RemovePlayers>().endGame = true;
        if (PhotonNetwork.IsMasterClient) gameManager.StopAllCoroutines();
        PhotonNetwork.LeaveRoom(true);
    }
    #endregion

    public void ChangeAnimationSpeed(float bpm)
    {
        AnimVelocity = 1.8f - (0.4f * (bpm - 1));
        secondsToCount = (1.4f / (AnimVelocity/1.4f)) * 0.3f / 1.4f;
        AnimVelocityCollision = (1.4f / (AnimVelocity/1.4f)) * 0.7f / 1.4f;
        /*switch(bpm)
        {
            case 1:
                AnimVelocity = 1.8f;
                secondsToCount = 0.25f;
                AnimVelocityCollision = 0.42f;
                break;

            case 2:
                AnimVelocity = 1.4f;
                secondsToCount = 0.3f;
                AnimVelocityCollision = 0.7f;
                break;

            case 3:
                AnimVelocity = 1;
                secondsToCount = 0.4f;
                AnimVelocityCollision = 0.98f;
                break;
        }*/
        animator.SetFloat("AnimMultiplier", AnimVelocity);
    }
}
