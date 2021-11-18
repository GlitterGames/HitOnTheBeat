using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Estructuras
    [System.Serializable]
    public struct ColoresAnillos
    {
        public Color anillo0;
        public Color anillo1;
        public Color anillo2;
        public Color anillo3;
        public Color anillo4;
        public Color anillo5;
    }
    [System.Serializable]
    public struct ColoresParpadeo
    {
        public Color ready;
        public Color steady;  
        public Color fall;     
    }
    [System.Serializable]
    public struct ColoresMovimiento
    {
        public Color actual;
        public Color adyacente;
    }
    [System.Serializable]
    public struct ColoresBombaColor
    {
        public Color anillo0;
        public Color anillo1;
        public Color anillo2;
        public Color selectedFloor0;
        public Color selectedFloor1;
    }
    public struct Movement
    {
        public PlayerController player;
        public Floor nextFloor;
        public FloorDetectorType dir;
    }
    public struct Ultimate
    {
        public PlayerController player;
        public PlayerController.Ultimate type;
        public Floor targetFloor;
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

    #region Constantes
    public static int MAX_STRENGHT = 10;
    #endregion

    #region Variables
    public ColoresMovimiento coloresEspeciales;
    public ColoresBombaColor coloresBombaColor;
    public ColoresAnillos coloresAnillos;
    public ColoresParpadeo coloresParpadeo;
    [HideInInspector]
    public List<Color> color = new List<Color>();
    [HideInInspector]
    public List<Color> colorBlink = new List<Color>();
    public Color backgroundColor;
    public GameObject[] background = new GameObject[6];
    public List<Floor[]> casillas = new List<Floor[]>();
    [HideInInspector]
    public List<PlayerController> jugadores = new List<PlayerController>();
    public Queue<Movement> movimientos = new Queue<Movement>();
    public Queue<Ultimate> ultimates = new Queue<Ultimate>();
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
        PerformUltimates();
        PerformMovements();
        PerformColision();
        ApplyEfectsFromFloor();
    }

    private void InitColor() {
        color.Add(coloresAnillos.anillo0);
        color.Add(coloresAnillos.anillo1);
        color.Add(coloresAnillos.anillo2);
        color.Add(coloresAnillos.anillo3);
        color.Add(coloresAnillos.anillo4);
        color.Add(coloresAnillos.anillo5);
    }
    private void InitColorBlink()
    {
        colorBlink.Add(coloresParpadeo.ready);
        colorBlink.Add(coloresParpadeo.steady);
        colorBlink.Add(coloresParpadeo.fall);
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
    //Si ya existe esa colision te devuelve la posición en la que te encuentra
    // -1 si no la encuentra
    private int Exists(List<Colisions> colisiones, Floor f) {
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
                Debug.Log("fuerza de jugadores " +jugadores[k].Fuerza);
                for (int i = k + 1; i < jugadores.Count; i++)
                {
                    if (jugadores[k].actualFloor.Equals(jugadores[i].actualFloor))
                    {
                        Debug.LogWarning("Hemos encontrado una colision");
                        int pos = Exists(colisiones, jugadores[k].actualFloor);
                        if (pos == -1)
                        {
                            Colisions colision = new Colisions(jugadores[k].actualFloor);
                            colision.positions.Add(k);
                            colision.positions.Add(i);
                            colisiones.Add(colision);
                        }
                        else
                        {
                            //AÑADE LOS JUGAODRES SI ESTOS NO SE ENCUENTRAN EN EL ARRAY
                            if(!colisiones[pos].positions.Contains(i)) colisiones[pos].positions.Add(i);
                            if (!colisiones[pos].positions.Contains(k)) colisiones[pos].positions.Add(k);
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
            //Cinemáticas
            for (int i = 0; i < jugadores.Count; i++)
            {
                //En caso de que se acabe de dar una colision no se realizará una cinemática
                if (jugadores[i].colision == true) 
                {
                    jugadores[i].colision = false;
                    //Se realizará en la siguiente iteración si me he quedado con fuerza cinética
                    if (jugadores[i].fuerzaCinetica > 0)
                    {
                        bcolision = true;
                    }
                }
                //En caso de que haya que realizar un movimiento por cinemática
                else if(jugadores[i].fuerzaCinetica>0)
                {
                    bool sameFloor = true;
                    bool notCinematic = false; //No es cinemática
                    bool moreThanTwo = false; //Se moverá en mi dirección
                    //Se le echará a la misma dirreción a la que iba
                    //con una fuerza de la fuerza cinética que tiene
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
    public bool Powers(bool sameFloor, Colisions c, out List<int> delete)
    {
        //Aray con las posiciones de los que se hayan caido
        List<int> eliminados = new List<int>();
        bool moreThanTwo = true; //Solo se cambiará en caso de que los jugadores sean 2
        bool notCinematic = true; //Si llama a este método no es cinemática
        bool powers = false; //Si ningún jugador tiene poderes se realizará la colision normal
        if (jugadores.Count == 2)
        {
            moreThanTwo = false;
            //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
            if (jugadores[c.positions[0]].fuerzaCinetica > 0) jugadores[c.positions[0]].Fuerza = jugadores[c.positions[0]].fuerzaCinetica;
            if (jugadores[c.positions[1]].fuerzaCinetica > 0) jugadores[c.positions[1]].Fuerza = jugadores[c.positions[1]].fuerzaCinetica;

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
                //Solo el que pierde acaba con fuerza 0
                jugadores[c.positions[0]].Fuerza = 0;
                powers = true;
            }
            else if (jugadores[c.positions[0]].power == PlayerController.Power_Up.ESCUDO)
            {
                //Para la corutina para que esta no le quite otro poder en el futuro
                StopCoroutine(jugadores[c.positions[1]].powerCoroutine);
                //Y llama a la RPC que utiliza su poder
                jugadores[c.positions[0]].UsePowerUp();
                //Solo el que pierde acaba con fuerza 0
                jugadores[c.positions[1]].Fuerza = 0;
                powers = true;
            }
            if (powers)
            {
                //Los dos jugadores dan la dirección del otro jugador
                //como dirección a la que tienen que ir con uno de fuerza
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
                echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
            }
        }
        else
        {
            //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
            for (int i = 0; i < c.positions.Count; i++)
            {
                if (jugadores[c.positions[i]].fuerzaCinetica > 0) jugadores[c.positions[i]].Fuerza = jugadores[c.positions[i]].fuerzaCinetica;
            }
            List<int> fuerzas = new List<int>();
            bool equal = false;
            int maxFuerza = 0;
            //Se recorren todos los jugadores viendo que fuerza es la mayor
            for (int i = 0; i < c.positions.Count; i++)
            {
                //Tu fuerza es equivalente a la mayor posible divida entre los que colisionas
                //redondeada hacia arriba
                int fuerza = (int)Mathf.Ceil(jugadores[c.positions[i]].Fuerza / (c.positions.Count - 1));
                //Si hay uno con mayor fuerza la sobreescribe y dice que no hay nadie con su misma fuerza
                if (fuerza > maxFuerza)
                {
                    equal = false;
                    maxFuerza = fuerza;
                }
                //Si hay uno con igual fuerza dice que hay nadie con su misma fuerza
                else if (fuerza == maxFuerza)
                {
                    equal = false;
                }
                //finalmente añado la fuerza con la que pego al array de fuerzas
                fuerzas.Add(fuerza);
            }
            //Se realizan las colisiones
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
                        bool echado = jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, fuerzas[i] - jugadores[c.positions[i]].Fuerza, moreThanTwo, notCinematic, sameFloor);
                        if (echado) eliminados.Add(i);
                    }
                    jugadores[c.positions[i]].Fuerza = 0;
                }
            }
        }
        delete = eliminados;
        return powers;
    }
    public List<int> PerformColision(bool sameFloor, Colisions c)
    {
        //Aray con las posiciones de los que se hayan caido
        List<int> eliminados = new List<int>(); 
        bool moreThanTwo = true; //Solo se cambiará en caso de que los jugadores sean 2
        bool notCinematic = true; //Si llama a este método no es cinemática
        //Si alguno de los dos tiene el poder de escudo las colisiones se realizan en este método
        if (Powers(sameFloor, c, out eliminados)) return eliminados;
        //Colision entre dos jugadores
        if (jugadores.Count == 2)
        {
            moreThanTwo = false;
            //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
            if (jugadores[c.positions[0]].fuerzaCinetica > 0) jugadores[c.positions[0]].Fuerza = jugadores[c.positions[0]].fuerzaCinetica;
            if (jugadores[c.positions[1]].fuerzaCinetica > 0) jugadores[c.positions[1]].Fuerza = jugadores[c.positions[1]].fuerzaCinetica;
            //Los dos jugadores se dan la dirección del otro jugador como dirección a la que tienen que ir
            //Si los dos jugadores tienen la misma fuerza se echan uno para atrás
            if (jugadores[c.positions[0]].Fuerza == jugadores[c.positions[1]].Fuerza)
            {
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
                echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
            }
            //Si los dos jugadores tienen distinta fuerza se echan la diferencia para atrás
            else if (jugadores[c.positions[0]].Fuerza > jugadores[c.positions[1]].Fuerza)
            {
                jugadores[c.positions[0]].Golpear();
                int max = jugadores[c.positions[0]].Fuerza - jugadores[c.positions[1]].Fuerza;
                bool echado = jugadores[c.positions[1]].EcharOne(jugadores[c.positions[0]].floorDir, max, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[1]);
            }
            //Si los dos jugadores tienen distinta fuerza se echan la diferencia para atrás
            else
            {
                jugadores[c.positions[1]].Golpear();
                int max = jugadores[c.positions[1]].Fuerza - jugadores[c.positions[0]].Fuerza;
                bool echado = jugadores[c.positions[0]].EcharOne(jugadores[c.positions[1]].floorDir, max, moreThanTwo, notCinematic, sameFloor);
                if (echado) eliminados.Add(c.positions[0]);
            }
            //Todos los participantes reinician su fuerza
            jugadores[c.positions[0]].Fuerza = 0;
            jugadores[c.positions[1]].Fuerza = 0;
        }
        //Colision entre más de dos jugadores
        else
        {
            //Por todos los jugadores pone la fuerza cinética como su fuerza para colisionar con los demás jugadores
            for (int i = 0; i < c.positions.Count; i++)
            {
                if (jugadores[c.positions[i]].fuerzaCinetica > 0) jugadores[c.positions[i]].Fuerza = jugadores[c.positions[i]].fuerzaCinetica;
            }
            List<int> fuerzas = new List<int>();
            bool equal = false;
            int maxFuerza = 0;
            //Se recorren todos los jugadores viendo que fuerza es la mayor
            for (int i = 0; i < c.positions.Count; i++)
            {
                //Tu fuerza es equivalente a la mayor posible divida entre los que colisionas
                //redondeada hacia arriba
                int fuerza = (int)Mathf.Ceil(jugadores[c.positions[i]].Fuerza / (c.positions.Count-1));
                //Si hay uno con mayor fuerza la sobreescribe y dice que no hay nadie con su misma fuerza
                if (fuerza > maxFuerza)
                {
                    equal = false;
                    maxFuerza = fuerza;
                }
                //Si hay uno con igual fuerza dice que hay nadie con su misma fuerza
                else if (fuerza == maxFuerza) 
                {
                    equal = false;
                }
                //finalmente añado la fuerza con la que pego al array de fuerzas
                fuerzas.Add(fuerza);
            }
            //Se realizan las colisiones
            for (int i = 0; i < c.positions.Count; i++)
            {
                //Si eres la mayor fuerza y nadie te iguala golpeas
                if (fuerzas[i] == maxFuerza && !equal)
                {
                    jugadores[c.positions[i]].Golpear();
                }
                //Si eres la mayor fuerza pero alguien te iguala te caes una para atrás en la dirección opuesta que llevabas
                else if (fuerzas[i] == maxFuerza && equal)
                {
                    jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, 1, moreThanTwo, notCinematic, sameFloor);
                }
                //Si no eras la mayor fuerza te caes para atras a dirección opuesta de la que llevabas con la diferencia entre
                //la máxima fuerza y la tuya
                else
                {
                    bool echado = jugadores[c.positions[i]].EcharOne(jugadores[i].floorDir, maxFuerza - fuerzas[i], moreThanTwo, notCinematic, sameFloor);
                    if (echado) eliminados.Add(i);
                }
                //Todos los participantes reinician su fuerza
                jugadores[c.positions[i]].Fuerza = 0;
            }
        }
        return eliminados;

    }
    //Esta region podría mejorarse mediante la implementación de eventos.
    #region PlayerControllerManagement
    public void RegisterMovement(int id, int row, int index, FloorDetectorType dir)
    {
        Movement move = new Movement();
        move.player = jugadores.Find((player) => player.GetIdPlayer() == id);
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

    public void RegisterUltimate(int id, PlayerController.Ultimate type)
    {
        Ultimate ultimate = new Ultimate();
        ultimate.player = jugadores.Find((player) => player.GetIdPlayer() == id);
        ultimate.type = type;
        ultimates.Enqueue(ultimate);
    }

    public void RegisterUltimate(int id, PlayerController.Ultimate type, int row, int index)
    {
        Ultimate ultimate = new Ultimate();
        ultimate.player = jugadores.Find((player) => player.GetIdPlayer() == id);
        ultimate.type = type;
        ultimate.targetFloor = casillas[row][index];
        ultimates.Enqueue(ultimate);
    }

    private void PerformUltimates()
    {
        while (ultimates.Count > 0)
        {
            Ultimate ul = ultimates.Dequeue();
            switch (ul.type)
            {
                case PlayerController.Ultimate.MEGA_PUNCH:
                    ul.player.PerformMegaPunch(true);
                    break;
                case PlayerController.Ultimate.BOMBA_COLOR:
                    ul.player.PerformBombaColor(true, ul.targetFloor);
                    break;
                case PlayerController.Ultimate.INVISIBILITY:
                    ul.player.PerformInvisibility(true);
                    break;
            }
        }
        //Se actualizan los valores de las Ultimates en todos los jugadores.
        foreach (PlayerController pc in jugadores)
        {
            pc.CancelUltimate();
            pc.UpdateUltimateTime();
        }
    }

    #endregion
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
        f.gameObject.SetActive(false);
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
        
        background[row].SetActive(false);
        yield return new WaitForEndOfFrame();
    }
}
