using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
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
    #region Estructuras
    public struct Movement
    {
        public PlayerController player;
        public Floor nextFloor;
        public FloorDetectorType dir;
    }
    public class Colisions 
    {
        public Floor floor;
        public List<int> positions;
        public Colisions(Floor f)
        {
            this.floor = f;
            positions = new List<int>();
        }
    }
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
        Floor f = casillas[i][j];
        int num = Floor.Type.GetNames(typeof(Floor.Type)).Length;
        int k = Random.Range(1, num);
        Debug.Log("La casilla donde se dbería pintar es: " + f.row + " " + f.index +" DE TIPO " + (Floor.Type)k);
        f.powertime = StartCoroutine(SetType(f, (Floor.Type)k, time));
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
    private int exists(List<Colisions> colisiones, Floor f) {
        for(int i=0; i<colisiones.Count; i++)
        {
            if (colisiones[i].Equals(f)) return i;
        }
        return -1;
    }
    private void PerformColision()
    {
        bool bcolision = true; //En caso de que no se tengan que comprobar colisiones
        while (bcolision)
        {
            List<Colisions> colisiones = new List<Colisions>();
            bcolision = false;
            //Colision en caso de dos jugadores se intercambien casillas
            for (int k = 0; k < jugadores.Count; k++)
            {
                for (int i = k + 1; i < jugadores.Count; i++)
                {
                    if (jugadores[k].actualFloor.Equals(jugadores[i].previousFloor)&&(jugadores[k].previousFloor.Equals(jugadores[i].actualFloor)))
                    {
                        Colisions colision = new Colisions(jugadores[i].actualFloor);
                        colision.positions.Add(k);
                        colision.positions.Add(i);
                        colisiones.Add(colision);
                    }
                }
            }
                //eliminar jugadores que se hayan caido
            for (int i = 0; i < colisiones.Count; i++)
            {
                List<int> players = PerformColision(false, colisiones[i]);
                for (int j = 0; j < players.Count; j++)
                {
                    jugadores.RemoveAt(players[j]);
                }
            }
            //Colision normal entre jugadores
            colisiones = new List<Colisions>();
            for (int k = 0; k < jugadores.Count; k++)
            {
                Debug.Log("fuerza de jugadores " +jugadores[k].fuerza);
                for (int i = k + 1; i < jugadores.Count; i++)
                {
                    if (jugadores[k].actualFloor.Equals(jugadores[i].actualFloor))
                    {
                        Debug.LogWarning("Hemos encontrado una colision");
                        int pos = exists(colisiones, jugadores[k].actualFloor);
                        if (pos == -1)
                        {
                            Colisions colision = new Colisions(jugadores[k].actualFloor);
                            colision.positions.Add(k);
                            colision.positions.Add(i);
                            colisiones.Add(colision);
                        }
                        else
                        {
                            colisiones[pos].positions.Add(i); //Añade un nuevo jugador a
                        }
                    }
                }
            }
                //eliminar jugadores que se hayan caido
            for (int i = 0; i < colisiones.Count; i++)
            {
                List<int> positions = PerformColision(true, colisiones[i]);
                for (int j = 0; j < positions.Count; j++)
                {
                    Debug.LogWarning("Se debe eliminar un jugador");
                    jugadores.RemoveAt(positions[j]);
                }
            }
            //REALIZAR LOS MOVIMIENTOS POR CINETICAS
            for (int i = 0; i < jugadores.Count; i++)
            {
                if (jugadores[i].colision == true) //En caso de que se acabe de dar una colision no se realizará una cinematica
                {
                    jugadores[i].colision = false;
                    if (jugadores[i].fuerzaCinetica > 0)
                    {
                        bcolision = true;
                    }
                }
                else if(jugadores[i].fuerzaCinetica>0)
                {
                    bool sameFloor = true;
                    bool notCinematic = false;
                    bool moreThanTwo = false;
                    Debug.LogWarning("Fuerza cinetica 1");
                    Debug.LogWarning("prev: Actual floor = " + jugadores[i].actualFloor.row + " " + jugadores[i].actualFloor.index);
                    Debug.LogWarning("prev: Previos floor = " + jugadores[i].previousFloor.row + " " + jugadores[i].previousFloor.index);
                    bool echado = jugadores[i].EcharOne(jugadores[i].floorDir, jugadores[i].fuerzaCinetica, moreThanTwo, notCinematic, sameFloor); 
                    if (echado) jugadores.RemoveAt(i);
                    if (jugadores[i].fuerzaCinetica > 0)
                    {
                        bcolision = true;
                    }
                }
            }
        }
    }
    public bool powers(bool sameFloor, Colisions c, out List<int> delete)
    {
        List<int> eliminados = new List<int>();
        bool powers = false;
        bool moreThanTwo = true;
        bool notCinematic = true;
        if (jugadores.Count == 2)
        {
            moreThanTwo = false;
            if (jugadores[c.positions[0]].fuerzaCinetica > 0) jugadores[c.positions[0]].fuerza = jugadores[c.positions[0]].fuerzaCinetica;
            if (jugadores[c.positions[1]].fuerzaCinetica > 0) jugadores[c.positions[1]].fuerza = jugadores[c.positions[1]].fuerzaCinetica;
            if (jugadores[c.positions[0]].power == PlayerController.Power_Up.ESCUDO && jugadores[c.positions[1]].power == PlayerController.Power_Up.ESCUDO)
            {
                StopCoroutine(jugadores[c.positions[0]].powerCoroutine);
                jugadores[c.positions[0]].UsePowerUp();
                StopCoroutine(jugadores[c.positions[1]].powerCoroutine);
                jugadores[c.positions[1]].UsePowerUp();
                powers = true;
            }
            else if (jugadores[c.positions[1]].power == PlayerController.Power_Up.ESCUDO)
            {
                StopCoroutine(jugadores[c.positions[1]].powerCoroutine);
                jugadores[c.positions[1]].UsePowerUp();
                jugadores[c.positions[0]].fuerza = 0;
                powers = true;
            }
            else if (jugadores[c.positions[0]].power == PlayerController.Power_Up.ESCUDO)
            {
                StopCoroutine(jugadores[c.positions[1]].powerCoroutine);
                jugadores[c.positions[0]].UsePowerUp();
                jugadores[c.positions[1]].fuerza = 0;
                powers = true;
            }
            if (powers)
            {
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
                echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
            }
        }
        else
        {
            for (int i = 0; i < c.positions.Count; i++)
            {
                if (jugadores[c.positions[i]].fuerzaCinetica > 0) jugadores[c.positions[i]].fuerza = jugadores[c.positions[i]].fuerzaCinetica;
            }
            List<int> fuerzas = new List<int>();
            bool equal = false;
            int maxFuerza = 0;
            for (int i = 0; i < c.positions.Count; i++)
            {
                int fuerza = 0;
                for (int j = 0; j < c.positions.Count; j++)
                {
                    if (i != j) fuerza += (int)Mathf.Ceil(jugadores[c.positions[j]].fuerza / c.positions.Count);
                }
                if (fuerza > maxFuerza)
                {
                    equal = false;
                    maxFuerza = fuerza;
                }
                else if (fuerza == maxFuerza)
                {
                    equal = false;
                }
                fuerzas.Add(fuerza);
            }
            for (int i = 0; i < c.positions.Count; i++)
            {
                if(jugadores[c.positions[i]].power == PlayerController.Power_Up.ESCUDO)
                {
                    StopCoroutine(jugadores[c.positions[i]].powerCoroutine);
                    jugadores[c.positions[i]].UsePowerUp();
                    bool echado = jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                    if (echado) eliminados.Add(c.positions[i]);
                    powers = true;
                }
                else
                {
                    if (fuerzas[i] == maxFuerza && !equal)
                    {
                        jugadores[c.positions[i]].Golpear();
                    }
                    else if (fuerzas[i] == maxFuerza && equal)
                    {
                        jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                    }
                    else
                    {
                        bool echado = jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, fuerzas[i] - jugadores[c.positions[i]].fuerza, moreThanTwo, notCinematic, sameFloor);
                        if (echado) eliminados.Add(i);
                    }
                    jugadores[c.positions[i]].fuerza = 0;
                }
            }
        }
        delete = eliminados;
        return powers;
    }
    public List<int> PerformColision(bool sameFloor, Colisions c)
    {
        List<int> eliminados = new List<int>();
        bool moreThanTwo = true;
        bool notCinematic = true;
        if (powers(sameFloor, c, out eliminados)) return eliminados;
        if (jugadores.Count == 2)
        {
            moreThanTwo = false;
            if (jugadores[c.positions[0]].fuerzaCinetica > 0) jugadores[c.positions[0]].fuerza = jugadores[c.positions[0]].fuerzaCinetica;
            if (jugadores[c.positions[1]].fuerzaCinetica > 0) jugadores[c.positions[1]].fuerza = jugadores[c.positions[1]].fuerzaCinetica;
            Debug.LogWarning("Realizar colision");
            if (jugadores[c.positions[0]].fuerza == jugadores[c.positions[1]].fuerza)
            {
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
                echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
                Debug.LogWarning("CASO1");
            }
            else if (jugadores[c.positions[0]].fuerza > jugadores[c.positions[1]].fuerza)
            {
                jugadores[c.positions[0]].Golpear();
                int max = jugadores[c.positions[0]].fuerza - jugadores[c.positions[1]].fuerza;
                bool echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, max, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
                Debug.LogWarning("CASO2");
            }
            else
            {
                jugadores[c.positions[1]].Golpear();
                int max = jugadores[c.positions[1]].fuerza - jugadores[c.positions[0]].fuerza;
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, max, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
                Debug.LogWarning("CASO3");
            }
            jugadores[c.positions[0]].fuerza = 0;
            jugadores[c.positions[1]].fuerza = 0;
        }
        else
        {
            for (int i = 0; i < c.positions.Count; i++)
            {
                if (jugadores[c.positions[i]].fuerzaCinetica > 0) jugadores[c.positions[i]].fuerza = jugadores[c.positions[i]].fuerzaCinetica;
            }
            List<int> fuerzas = new List<int>();
            bool equal = false;
            int maxFuerza = 0;
            for (int i = 0; i < c.positions.Count; i++)
            {
                int fuerza = 0;
                for (int j = 0; j < c.positions.Count; j++)
                {
                    if (i != j) fuerza += (int)Mathf.Ceil(jugadores[c.positions[j]].fuerza / c.positions.Count);
                }
                if (fuerza > maxFuerza)
                {
                    equal = false;
                    maxFuerza = fuerza;
                }
                else if (fuerza == maxFuerza)
                {
                    equal = false;
                }
                fuerzas.Add(fuerza);
            }
            for (int i = 0; i < c.positions.Count; i++)
            {
                if (fuerzas[i] == maxFuerza && !equal)
                {
                    jugadores[c.positions[i]].Golpear();
                }
                else if (fuerzas[i] == maxFuerza && equal)
                {
                    jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                }
                else
                {
                    bool echado = jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, fuerzas[i] - jugadores[c.positions[i]].fuerza, moreThanTwo, notCinematic, sameFloor);
                    if (echado) eliminados.Add(i);
                }
                jugadores[c.positions[i]].fuerza = 0;
            }
        }
        return eliminados;

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
