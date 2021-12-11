using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class TutorialManager : MonoBehaviourPun
{
    /*

    #region Estructuras
    [System.Serializable]
    public struct Materiales
    {
        public Material normal;
        public Material X2;
        public Material escudo;
    }
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
    #endregion
	
    #region Variables
    public int numRows;
    public Materiales materiales;
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
    public float duracion;
    bool spawn;
    public Queue<Floor> fallenFloors = new Queue<Floor>();
    #endregion

    void Awake()
    {
        numRows = 5;
        spawn = false;
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
        AnimateFloors();
    }

    //Se ejecuta cada vez que comienza un nuevo Beat.
    public void DoBeatActions()
    {
        ApplyEfectsFromFloor();
        PerformUltimates();
        PerformMovements();
        PerformColision();
        spawn = true;
        while (fallenFloors.Count>0)
        {
            StartCoroutine(FallFloor(fallenFloors.Dequeue()));
        }
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

    public void ApplyEfectsFromFloor() {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (PlayerController jugador in jugadores)
            {
                if (jugador.actualFloor.GetPower() != Floor.Type.Vacio)
                {
                    Debug.Log("HE PILLADO UN POWE UP");
                    jugador.GetPowerUp();
                    if(jugador.actualFloor.powertime!=null) StopCoroutine(jugador.actualFloor.powertime);
                    else Debug.Log("No tiene corutina de power up");
                }
            }
        }
    }
    #region Colisiones
    //Si ya existe esa colision te devuelve la posición en la que te encuentra
    // -1 si no la encuentra
    private int Exists(List<Colisions> colisiones, Floor f) {
        for(int i=0; i<colisiones.Count; i++)
        {
            if (colisiones[i].floor.Equals(f)) return i;
        }
        return -1;
    }
    //Esta clase cambia los ID de los eliminados después de realizarse una eliminación
    //ya que su posición en el array variará
    private List<int> Reordenar(int pos, List<int> eliminados)
    {
        List<int> reordenados = new List<int>();
        for (int i = 0; i < eliminados.Count; i++) {
            //En el caso de que el eliminado este en una posición mayor de la que se ha
            //eliminado, la posicion de este eliminado será en una unidad menor
            if (eliminados[i] > pos)
            {
                reordenados.Add(eliminados[i] - 1);
            }
            else {
                reordenados.Add(eliminados[i]);
            }
        }
        return reordenados;
    }
    private void ReordenarGolpeados(int pos)
    {
        for (int i = 0; i < jugadores.Count; i++)
        {
            //Si el que se ha eliminado tiene la misma pos, ya no es tu golpeador
            if (jugadores[i].golpeador == pos) jugadores[i].golpeador = -1;
            //Si el que te ha golpeado estaba en una pos superior a la que se a eliminado
            //se baja una posicion
            if (jugadores[i].golpeador > pos) jugadores[i].golpeador--;
        }
    }
    private void PerformColision()
    {
        bool bcolision = true; //En caso de que no se tengan que comprobar colisiones
        List<Colisions> colisiones = new List<Colisions>();
        while (bcolision)
        {
            bcolision = false;
            //Colision en caso de dos jugadores se intercambien casillas
            colisiones = PerformNotSameFloorColisions(ref bcolision);
            //eliminar jugadores que se hayan caido
            DeletePlayersColision(colisiones, false);
            //Colision normal entre jugadores
            colisiones = PerformSameFloorColisions(ref bcolision);
            //eliminar jugadores que se hayan caido
            DeletePlayersColision(colisiones, true);
            //Colisiones cinemáticas
            CinematicColisions(ref bcolision);
        }
    }
    private void DeletePlayersColision(List<Colisions> colisiones, bool sameFloor)
    {
        
        for (int i = 0; i < colisiones.Count; i++)
        {
            colisiones[i].PerformColisions();
            List<int> positions = colisiones[i].eliminados;
            for (int j = 0; j < positions.Count; j++)
            {
                RemovePlayer(positions[j]);
                positions = Reordenar(positions[j], positions);
                ReordenarGolpeados(positions[j]);
            }
        }
    }
    private void CinematicColisions(ref bool bcolision)
    {
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
            else if (jugadores[i].fuerzaCinetica > 0)
            {
                bool sameFloor = true;
                bool notCinematic = false; //No es cinemática
                bool moreThanTwo = false; //Se moverá en mi dirección
                                          //Se le echará a la misma dirreción a la que iba
                                          //con una fuerza de la fuerza cinética que tiene
                bool echado = jugadores[i].EcharOne(jugadores[i].floorDir, jugadores[i].fuerzaCinetica, moreThanTwo, notCinematic, sameFloor, -1);
                if (echado)
                {
                    RemovePlayer(i);
                    ReordenarGolpeados(i);
                    i--;
                }
                bcolision = true;
            }
        }
    }
    private List<Colisions> PerformSameFloorColisions(ref bool bcolision)
    {
        List<Colisions> colisiones = new List<Colisions>();
        for (int k = 0; k < jugadores.Count; k++)
        {
            for (int i = k + 1; i < jugadores.Count; i++)
            {
                if (jugadores[k].actualFloor.Equals(jugadores[i].actualFloor))
                {
                    int pos = Exists(colisiones, jugadores[k].actualFloor);
                    if (pos == -1)
                    {
                        Colisions colision = new Colisions(jugadores[k].actualFloor, this, true);
                        colision.positions.Add(k);
                        colision.positions.Add(i);
                        colisiones.Add(colision);
                    }
                    else
                    {
                        //AÑADE LOS JUGAODRES SI ESTOS NO SE ENCUENTRAN EN EL ARRAY
                        if (!colisiones[pos].positions.Contains(i)) colisiones[pos].positions.Add(i);
                        if (!colisiones[pos].positions.Contains(k)) colisiones[pos].positions.Add(k);
                    }
                    bcolision = true;
                }
            }
        }
        return colisiones;
    }
    private List<Colisions> PerformNotSameFloorColisions(ref bool bcolision)
    {
        List<Colisions> colisiones = new List<Colisions>();
        for (int k = 0; k < jugadores.Count; k++)
        {
            for (int i = k + 1; i < jugadores.Count; i++)
            {
                if (jugadores[k].actualFloor.Equals(jugadores[i].previousFloor) && (jugadores[k].previousFloor.Equals(jugadores[i].actualFloor)))
                {
                    Colisions colision = new Colisions(jugadores[i].actualFloor, this, false);
                    colision.positions.Add(k);
                    colision.positions.Add(i);
                    colisiones.Add(colision);
                    bcolision = true;
                }
            }
        }
        return colisiones;
    }
    public Coroutine StartCoroutine(PlayerController p)
    {
        return StartCoroutine(p.PowerUp());
    }
    #endregion
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
        foreach (var player in jugadores) player.seHaMovido = false;
        while (movimientos.Count > 0)
        {
            Movement m = movimientos.Dequeue();
            m.player.Mover(m.nextFloor, m.dir);
            m.player.seHaMovido = true;
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
            pc.UpdateUltimateTime();
        }
    }

    #endregion

    #region ROWS and POWER UPS
    public void DestroyRow(int row, float seg, int repeticiones) {
        foreach (Floor f in casillas[row])
        {
            StartCoroutine(Blink(f, seg, repeticiones));
        }
        StartCoroutine(BlinkBackground(row, seg, repeticiones));
    }
    private IEnumerator Blink(Floor f, float seg, int repeticiones) {
        int j;
        jugadores[0].SetPower(f, Floor.Type.Parpadeando, false, false);
        //LAS CASILLAS PARPADEAN
        for (j = 1; j < colorBlink.Count; j++) {
            Color c1 = colorBlink[j-1];
            Color c2 = colorBlink[j];
            jugadores[0].SetColor(f, c1);
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                jugadores[0].SetColor(f, c1);
                jugadores[0].SetColorN(f, c1);
                yield return new WaitForSeconds(seg/repeticiones);
                jugadores[0].SetColor(f, c2);
                jugadores[0].SetColorN(f, c2);
                yield return new WaitForSeconds(seg/repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        yield return new WaitForSeconds(0.02f);
        fallenFloors.Enqueue(f);
    }
    private IEnumerator FallFloor(Floor f)
    {
        //LAS CASILLAS SE CAEN POR ACCIÓN DE LA GRAVEDAD
        jugadores[0].Cinematic(f);
        yield return new WaitForSeconds(0.5f);
        jugadores[0].Fall(f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //LOS JUGADORES QUE SE ENCUENTREN EN ESAS CASILLAS SE CAERAN
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < jugadores.Count; i++)
            {
                if (jugadores[i].actualFloor.Equals(f))
                {
                    jugadores[i].Caer();
                    RemovePlayer(i);
                    ReordenarGolpeados(i);
                    i--;
                }
            }
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
            jugadores[0].SetColorBackground(row, c1);
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                jugadores[0].SetColorBackground(row, c1);
                yield return new WaitForSeconds(seg / repeticiones);
                jugadores[0].SetColorBackground(row, c2);
                yield return new WaitForSeconds(seg / repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        jugadores[0].CinematicBackground(row);
    }
    private IEnumerator DestroyRows()
    {
        yield return new WaitForEndOfFrame();
        int numrep = numRows;
        for (int i = numrep; i>=0; i--)
        {
            yield return new WaitForSeconds(duracion/5);
            numRows--;
            DestroyRow(i, 1f, 3);
        }
    }
    private IEnumerator SpawnPowerUps()
    {
        //Tiempo de espera entre el spawn de otro power up
        float espera = 10f;
        bool seguir = true;
        while (seguir)
        {
            if (numRows < 3) { seguir = false; }
            yield return new WaitForSeconds(espera);
            spawn = false;
            yield return new WaitUntil(() => spawn);
            spawn = false;
            SpawnPowerUp(15f);
        }
    }
    //Cuando se cambie el master deberán ejecutarse estos métodos con valores actualizados.
    private void AnimateFloors()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //StartCoroutine(DestroyRows());
        StartCoroutine(SpawnPowerUps());
    }
    public void SpawnPowerUp(float time)
    {
        bool seguir = true;
        Floor f = null;
        if (numRows < 3) { return; }
        while (seguir) { 
            seguir = false;
            int i = Random.Range(0, numRows);
            int j = Random.Range(0, casillas[i].Length);
            f = casillas[i][j];
            for (int a = 0; a < jugadores.Count && !seguir; a++)
            {
                if (jugadores[a].actualFloor.Equals(f)) seguir = true;
            }
            if (f.GetPower() != Floor.Type.Vacio) seguir = true;
            if (numRows < 3) { return; }
        }
        int num = Floor.Type.GetNames(typeof(Floor.Type)).Length;
        int k = Random.Range(2, num);
        f.powertime = StartCoroutine(SetType(f, (Floor.Type)k, time));
    }
    private IEnumerator SetType(Floor f, Floor.Type t, float time)
    {
        FindObjectOfType<PhotonInstanciate>().my_player.
            GetComponent<PlayerController>().SetPowerUpFloor(f, t);
        yield return new WaitForSeconds(time);
        if(f!=null) FindObjectOfType<PhotonInstanciate>().my_player.
            GetComponent<PlayerController>().SetPowerUpFloor(f, Floor.Type.Vacio);
    }
    #endregion 
    public void ChangeAnimationSpeedOnAllPlayers(float bpm)
    {
        for (int i = 0; i < jugadores.Count; i++)
            {
                jugadores[i].ChangeAnimationSpeed(bpm);
            }
    }
    public void RemovePlayer(int index)
    {
        jugadores[index].SetPuesto(jugadores.Count - 1);
        if (jugadores.Count == 1)
        {
            RemovePlayers.instance.winnerName = jugadores[0].photonView.Owner.NickName;
            RemovePlayers.instance.winnerUltimate = jugadores[0].tipoUltimate;
            RemovePlayers.instance.winnerSkin = jugadores[0].tipoSkin;
        }
        jugadores.RemoveAt(index);
    }*/
}
