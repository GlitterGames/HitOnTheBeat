using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Variables
    const int NUM_ANILLOS = 6;
    const int DESTROY_TIME = 5;
    public static Color casillaAct = Color.blue;
    public static Color casillaAdy = Color.cyan;
    public static Color casillaAttack = Color.yellow;
    public List<Floor[]> casillas = new List<Floor[]>();
    public List<Color> color = new List<Color>();
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

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient) PerformColision();
    }

    private void InitColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
        color.Add(Color.yellow);
        color.Add(Color.yellow);
        color.Add(Color.red);
        color.Add(Color.red);
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
                suelo.setColorN(color[i]);
                suelo.setColor(color[i]);
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
        HashSet<GameObject> players = PhotonNetwork.FindGameObjectsWithComponent(typeof(PlayerController));
        List<PlayerController> jugadores = new List<PlayerController>();
        foreach (GameObject go in players)
        {
            jugadores.Add(go.GetComponent<PlayerController>());
        }
        for (int k = 0; k < jugadores.Count; k++)
        {
            for (int i = k + 1; i < jugadores.Count; i++)
            {
                if (jugadores[k].actualFloor.Equals(jugadores[i].actualFloor))
                {
                    jugadores[i].fuerza = 0;
                    jugadores[k].fuerza = 30;

                    if (jugadores[k].fuerza == jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[k].Mover(jugadores[k].previousFloor);
                        jugadores[i].Mover(jugadores[i].previousFloor);

                        //PIERDEN FUERZA
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                    }
                    else if (jugadores[k].fuerza > jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[k].Mover(jugadores[k].actualFloor); //SE MUEVE EL JUGADOR AL NEXT FLOOR

                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[k].fuerza - jugadores[i].fuerza;
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = jugadores[i].echar(jugadores[k].typeAnt, max);
                        if (echado)
                        {
                            jugadores.Remove(jugadores[i]);
                            i--;
                        }
                    }
                    else
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[i].Mover(jugadores[i].actualFloor); //SE MUEVE EL JUGADOR AL NEXT FLOOR

                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[i].fuerza - jugadores[k].fuerza;
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = jugadores[k].echar(jugadores[i].typeAnt, max);
                        if (echado)
                        {
                            jugadores.Remove(jugadores[k]);
                            i = k + 1;
                        }
                    }
                }
            }
        }
    }
}
