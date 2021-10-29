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
        InitCasillas();
    }

    //Se ejecuta cada vez que comienza un nuevo Beat.
    public void DoBeatActions()
    {
        PerformMovements();
        PerformColision();
    }

    private void InitColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
        color.Add(Color.yellow);
        color.Add(Color.yellow);
        color.Add(Color.red);
        color.Add(Color.red);
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
    public void EliminarCasillas(int i)
    {
        casillas.RemoveAt(i);
    }

    public Vector3 spawnPowerUp()
    {
        int i = Random.Range(0, casillas.Count);
        int j = Random.Range(0, casillas[i].Length);
        
        return casillas[i][j].GetFloorPosition() + new Vector3(0,1.0f,0);
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
        move.player = jugadores.Find((player) => (player.photonView.ViewID/1000-1) == id);
        move.nextFloor = casillas[row][index];
        move.dir = dir;
        movimientos.Enqueue(move);
    }

    private void PerformMovements()
    {
        while (movimientos.Count>0)
        {
            Movement m = movimientos.Dequeue();
            m.player.Mover(m.nextFloor, m.dir);
        }
    }
}
