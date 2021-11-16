using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Estructuras
    public struct Movement
    {
        public PlayerController player;
        public Floor nextFloor;
        public FloorDetectorType dir;
    }
    #endregion

    #region Variables
    public static Color casillaAct = Color.blue;
    public static Color casillaAdy = Color.cyan;
    public static Color casillaAttack = Color.yellow;
    public List<Color> color = new List<Color>();
    public List<Color> colorBlink = new List<Color>();
    public Color backgroundColor;
    public GameObject[] background = new GameObject[6];
    public List<Floor[]> casillas = new List<Floor[]>();
    public List<PlayerController> jugadores = new List<PlayerController>();
    public Queue<Movement> movimientos = new Queue<Movement>();
    #endregion

    void Awake()
    {
        //Inicializaci�n de la estructura de datos que vamos a utilizar para alamcenar las casillas
        casillas.Add(new Floor[1]);
        casillas.Add(new Floor[6]);
        casillas.Add(new Floor[12]);
        casillas.Add(new Floor[18]);
        casillas.Add(new Floor[24]);
        casillas.Add(new Floor[30]);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitColor();
        InitColorBlink();
        InitCasillas();
        InitBackground();
    }

    //Se ejecuta cada vez que comienza un nuevo Beat.
    public void DoBeatActions()
    {
        PerformMovements();
        PerformColision();
        ApplyEfectsFromFloor();
    }

    private void InitColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
        color.Add(Color.yellow);
        color.Add(Color.yellow);
        color.Add(Color.red);
        color.Add(Color.red);
    }
    private void InitColorBlink()
    {
        colorBlink.Add(Color.green);
        colorBlink.Add(Color.yellow);
        colorBlink.Add(Color.red);
    }
    private void InitBackground()
    {
        for(int i = 0; i<6; i++)
        {
            background[i].GetComponent<Renderer>().material.color = backgroundColor;
        }
    }
    public void UpdatePlayers()
    {
        jugadores.Clear();
        HashSet<GameObject> players = PhotonNetwork.FindGameObjectsWithComponent(typeof(PlayerController));
        foreach (GameObject go in players)
        {
            jugadores.Add(go.GetComponent<PlayerController>());
        }
    }

    private void InitCasillas()
    {
        //Adquiero todas las casillas de las escena
        Floor[] f = FindObjectsOfType<Floor>();

        //Le paso dichas casillas a todos los jugadores.
        foreach (Floor floor in f)
        {
            casillas[floor.row][floor.index] = floor;
        }

        CasillasSetColor();
    }

    private void CasillasSetColor()
    {
        for (int i = 0; i < casillas.Count; i++)
        {
            foreach (Floor suelo in casillas[i])
            {
                suelo.SetColorN(color[i]);
                suelo.SetColor(color[i]);
            }
        }
    }

    public void SpawnPowerUp(float time)
    {
        int i = Random.Range(0, casillas.Count);
        int j = Random.Range(0, casillas[i].Length);

        Floor f = casillas[0][0];
        Debug.Log("La casilla donde se dbería pintar es: " +f.row +" " +f.index);
       
        f.powertime = StartCoroutine(SetType(f, Floor.Type.RitmoDuplicado, time));
    }
    private IEnumerator SetType(Floor f, Floor.Type t, float time) {
        FindObjectOfType<PhotonInstanciate>().my_player.
            GetComponent<PlayerController>().SetPowerUp(f, t);
        yield return new WaitForSeconds(time);
        FindObjectOfType<PhotonInstanciate>().my_player.
            GetComponent<PlayerController>().SetPowerUp(f, Floor.Type.Vacio);
    }
    public void ApplyEfectsFromFloor() {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (PlayerController jugador in jugadores)
            {
                if (jugador.actualFloor.GetPower() != Floor.Type.Vacio)
                {
                    jugador.GetPowerUp();
                    StopCoroutine(jugador.actualFloor.powertime);
                }
            }
        }
    }
    private void PerformColision()
    {
        for (int k = 0; k < jugadores.Count; k++)
        {
            for (int i = k + 1; i < jugadores.Count; i++)
            {
                if (jugadores[k].actualFloor.Equals(jugadores[i].actualFloor))
                {
                    if (jugadores[k].fuerza == jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        FloorDetectorType aux = jugadores[k].floorDir;
                        jugadores[k].Echar(jugadores[i].floorDir, 1);
                        jugadores[i].Echar(aux, 1);

                        //PIERDEN FUERZA
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                    }
                    else if (jugadores[k].fuerza > jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[k].Golpear(); //SE MUEVE EL JUGADOR AL NEXT FLOOR
                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[k].fuerza - jugadores[i].fuerza;
                        jugadores[k].fuerza = max;
                        jugadores[i].fuerza = 0;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = jugadores[i].Echar(jugadores[k].floorDir, max);
                        if (echado)
                        {
                            jugadores[k].Kill();
                            jugadores.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[i].Golpear(); //SE MUEVE EL JUGADOR AL NEXT FLOOR
                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[i].fuerza - jugadores[k].fuerza;
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = max;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = jugadores[k].Echar(jugadores[i].floorDir, max);
                        if (echado)
                        {
                            jugadores[i].Kill();
                            jugadores.RemoveAt(k);
                            i = k;
                        }
                    }
                }
            }
        }
    }
    public void RegisterMovement(int id, int row, int index, FloorDetectorType dir)
    {
        Movement move = new Movement();
        move.player = jugadores.Find((player) => (player.photonView.ViewID / 1000 - 1) == id);
        move.nextFloor = casillas[row][index];
        move.dir = dir;
        movimientos.Enqueue(move);
    }

    private void PerformMovements()
    {
        while (movimientos.Count > 0)
        {
            Movement m = movimientos.Dequeue();
            m.player.Mover(m.nextFloor, m.dir);
        }
    }
    public void DestroyRow(int row, float seg, int repeticiones) {
        foreach (Floor f in casillas[row])
        {
            StartCoroutine(Blink(f, seg, repeticiones));
        }
        StartCoroutine(BlinkBackground(row, seg, repeticiones));
    }
    private IEnumerator Blink(Floor f, float seg, int repeticiones) {
        int j;
        //LAS CASILLAS PARPADEAN
        for (j = 1; j < colorBlink.Count; j++) {
            Color c1 = colorBlink[j-1];
            Color c2 = colorBlink[j];
            f.SetColor(c1);
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                f.SetColor(c1);
                yield return new WaitForSeconds(seg/repeticiones);
                f.SetColor(c2);
                yield return new WaitForSeconds(seg/repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        yield return new WaitForSeconds(0.02f);
        //LAS CASILLAS SE CAEN POR ACCIÓN DE LA GRAVEDAD
        f.GetComponentInChildren<Rigidbody>().isKinematic = false;
        f.GetComponentInChildren<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(0.5f);
        f.gameObject.active = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //LOS JUGADORES QUE SE ENCUENTREN EN ESAS CASILLAS SE CAERAN
        if (PhotonNetwork.IsMasterClient) {
            foreach (PlayerController jugador in jugadores) {
                if (jugador.actualFloor.Equals(f))
                {
                    jugador.Caer(); 
                    jugadores.Remove(jugador);
                }
            }
            casillas[f.row][f.index] = null;
        }
    }
    private IEnumerator BlinkBackground(int row, float seg, int repeticiones)
    {
        int j;
        //LAS CASILLAS PARPADEAN
        for (j = 1; j < colorBlink.Count; j++)
        {
            Color c1 = colorBlink[j - 1];
            Color c2 = colorBlink[j];
            background[row].GetComponent<Renderer>().material.color = c1;
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                background[row].GetComponent<Renderer>().material.color = c1;
                yield return new WaitForSeconds(seg / repeticiones);
                background[row].GetComponent<Renderer>().material.color = c2;
                yield return new WaitForSeconds(seg / repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        
        background[row].active = false;
        yield return new WaitForEndOfFrame();
    }
    bool usado = false;
    public void Update()
    {
        //PRUEBA DE USO CASILLAS PARPADEO Y CAIDA.
        //if (!usado) {
        //    DestroyRow(4, 0.5f, 3);
        //    usado = true;
        //}
        //if (!usado&&PhotonNetwork.IsMasterClient) {
        //    SpawnPowerUp(100f);
        //    SpawnPowerUp(100f);
        //    SpawnPowerUp(100f);
        //    SpawnPowerUp(100f);
        //    usado = true;
        //}
    }
}
