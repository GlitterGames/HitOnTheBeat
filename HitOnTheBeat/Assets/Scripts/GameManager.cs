using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Variables
    const int NUM_FILAS = 6;
    const int NUM_CASILLAS = 91;
    const int DESTROY_TIME = 5;
    public static Color casillaAct = Color.blue;
    public static Color casillaAdy = Color.cyan;
    public static Color casillaAttack = Color.yellow;
    public List<List<Floor>> casillas = new List<List<Floor>>();
    public List<Color> color = new List<Color>();
    private bool cargaLista = false;
    #endregion

    void Awake()
    {
        //Inicializaci�n de la estructura de datos que vamos a utilizar para alamcenar las casillas
        for (int i = 0; i < NUM_FILAS; i++)
        {
            casillas.Add(new List<Floor>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initColor();
    }

    // Update is called once per frame
    void Update()
    {
        if(!cargaLista) initCasillas();
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient) colision();
    }

    private void initColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
        color.Add(Color.yellow);
        color.Add(Color.yellow);
        color.Add(Color.red);
        color.Add(Color.red);
    }
    private void initCasillas()
    {
        //Adquiero todas las casillas de las escena
        HashSet<GameObject> f = PhotonNetwork.FindGameObjectsWithComponent(typeof(Floor));
        if(f.Count < NUM_CASILLAS)
        {
            return;
        }
        cargaLista = true;
        //Le paso dichas casillas a todos los jugadores.
        foreach (GameObject floor in f)
        {
            Floor casilla = floor.GetComponent<Floor>();
            int posicion = casilla.row;
            casillas[posicion].Add(casilla);
        }
        casillasSetColor();
    }

    private void casillasSetColor()
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
    public void eliminarCasillas(int i)
    {
        Rigidbody r;
        foreach (Floor f in casillas[i])
        {
            r = f.GetComponent<Rigidbody>();
            r.useGravity = true; //Animaci�n de caida
            foreach (Transform child in f.transform) { GameObject.Destroy(child.gameObject, DESTROY_TIME);}
            Destroy(f, DESTROY_TIME);
        }
        casillas.RemoveAt(i);
    }

    public Vector3 spawnPowerUp()
    {
        int i = Random.Range(0, casillas.Count);
        int j = Random.Range(0, casillas[i].Count);
        
        return casillas[i][j].GetFloorPosition() + new Vector3(0,1.0f,0);
    }
    
    private void colision(){
        HashSet<GameObject> players = PhotonNetwork.FindGameObjectsWithComponent(typeof(PlayerController));
        List<PlayerController> jugadores = new List<PlayerController>();
        foreach(GameObject go in players)
        {
            jugadores.Add(go.GetComponent<PlayerController>());
        }
        for (int k = 0; k<jugadores.Count; k++) {
            for (int i = k+1; i < jugadores.Count; i++) {
                if (jugadores[k].actualFloor.id == jugadores[i].actualFloor.id)
                {
                    if (jugadores[k].fuerza == jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        Debug.Log("Sin fuerza");
                        jugadores[k].mover(jugadores[k].previousFloor);
                        jugadores[i].mover(jugadores[i].previousFloor);

                        
                        //PIERDEN FUERZA
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                    }
                    else if (jugadores[k].fuerza > jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                         Debug.Log("Cikn fuerza");
                        jugadores[k].mover(jugadores[k].previousFloor);
                       
                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[k].fuerza - jugadores[i].fuerza;
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = false;
                        for (int j = 0; j < max && !echado; j++)
                        {
                            echado = jugadores[i].echar(jugadores[k].typeAnt);
                        }
                        if (echado) jugadores.Remove(jugadores[i]);
                    }
                    else
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        Debug.Log("YXCTFURVYGIHBNJ");
                        jugadores[i].mover(jugadores[i].previousFloor);
                        
                        //CUANTAS HAN DE PERDERSE, Y PIERDEN FUERZA
                        int max = jugadores[i].fuerza - jugadores[k].fuerza;
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                        //MOVER TANTAS CASILLAS COMO SEA NECESARIO AL PERDEDOR EN EL CHOQUE
                        bool echado = false;
                        for (int j = 0; j < max && !echado; j++)
                        {
                            echado = jugadores[k].echar(jugadores[i].typeAnt);
                        }
                        if (echado) {jugadores.Remove(jugadores[k]); i = k + 1;}
                    }
                }
            }
        }
    }
}
