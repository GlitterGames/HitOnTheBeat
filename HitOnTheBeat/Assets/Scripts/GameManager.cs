using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    const int NUM_CASILLAS = 4;
    const int DESTROY_TIME = 5;
    public static Color casillaAct = Color.blue;
    public static Color casillaAdy = Color.cyan;
    public static Color casillaAttack = Color.yellow;
    public List<List<Floor>> casillas = new List<List<Floor>>();
    public List<PlayerController> jugadores = new List<PlayerController>();
    public List<Color> color = new List<Color>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        initColor();
        initCasillas();
        initPlayer();
        casillasSetColor();
    }
    private void initColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
        color.Add(Color.yellow);
        color.Add(Color.red);
    }
    private void initCasillas()
    {
        //Inicialización de la estructura de datos que vamos a utilizar para alamcenar las casillas
        for (int i = 0; i < NUM_CASILLAS; i++)
        {
            casillas.Add(new List<Floor>());
        }
        //Adquiero todas las casillas de las escena
        Floor[] f = (Floor[])Object.FindObjectsOfType(typeof(Floor));
        //Las añado a la fila que les corresponde
        int j = 0;
        foreach (Floor floor in f)
        {
            floor.id = j;
            int posicion = floor.row;
            casillas[posicion].Add(floor);
            j++;
        }
    }
    private void initPlayer()
    {
        PlayerController[] p = (PlayerController[])Object.FindObjectsOfType(typeof(PlayerController));
        foreach (PlayerController player in p)
        {
            jugadores.Add(player);
        }
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
            r.useGravity = true; //Animación de caida
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
    // Update is called once per frame
    void Update()
    {
        colision();
    }
    private void colision(){
        for(int k = 0; k<jugadores.Count; k++) {
            for (int i = k+1; i < jugadores.Count; i++) {
                if (jugadores[k].f.id == jugadores[i].f.id)
                {
                    if (jugadores[k].fuerza == jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[k].mover(jugadores[k].antf);
                        jugadores[i].mover(jugadores[i].antf);
                        //PIERDEN FUERZA
                        jugadores[k].fuerza = 0;
                        jugadores[i].fuerza = 0;
                    }
                    else if (jugadores[k].fuerza > jugadores[i].fuerza)
                    {
                        //JUGADOR GANADOR SE QUEDA DONDE ESTABA ANTES
                        jugadores[k].mover(jugadores[k].antf);
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
                        jugadores[i].mover(jugadores[i].antf);
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
