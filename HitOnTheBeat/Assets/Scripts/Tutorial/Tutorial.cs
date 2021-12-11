using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    #region Atributes
    //Players
    PlayerController myPlayer;
    List<PlayerController> playerControllers = new List<PlayerController>();
    //Casillas
    List<Floor[]> casillas = new List<Floor[]>();
    public Queue<Floor> fallenFloors = new Queue<Floor>();
    //Colores
    public ColoresAnillos coloresAnillos;
    public ColoresParpadeo coloresParpadeo;
    [HideInInspector]
    public List<Color> color = new List<Color>();
    [HideInInspector]
    public List<Color> colorBlink = new List<Color>();
    //Otros
    int numRows;
    int duracion;
    bool spawn; 
    #endregion
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
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        numRows = 5;
        spawn = false;
        duracion = 30;
        InitColor();
        InitArrayCasillas();
        InitCasillas();
    }
    private void InitArrayCasillas()
    {
        casillas.Add(new Floor[1]);
        casillas.Add(new Floor[6]);
        casillas.Add(new Floor[12]);
        casillas.Add(new Floor[18]);
        casillas.Add(new Floor[24]);
        casillas.Add(new Floor[30]);
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
    private void InitColor()
    {
        color.Add(coloresAnillos.anillo0);
        color.Add(coloresAnillos.anillo1);
        color.Add(coloresAnillos.anillo2);
        color.Add(coloresAnillos.anillo3);
        color.Add(coloresAnillos.anillo4);
        color.Add(coloresAnillos.anillo5);
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
    // Update is called once per frame
    void Update()
    {
        
    }
    #region Tutorials
    public void TutorialFallingFloors()
    {

    }
    public void TutorialPowerUpsEscudo()
    {

    }
    public void TutorialPowerUpsX2()
    {

    }
    public void TutorialPowerUps()
    {

    }
    public void TutorialMovement()
    {

    }
    #endregion
    #region PowerUps and Rows
    public void DestroyRow(int row, float seg, int repeticiones)
    {
        foreach (Floor f in casillas[row])
        {
            StartCoroutine(Blink(f, seg, repeticiones));
        }
        StartCoroutine(BlinkBackground(row, seg, repeticiones));
    }
    private IEnumerator Blink(Floor f, float seg, int repeticiones)
    {
        int j;
        myPlayer.SetPower(f, Floor.Type.Parpadeando, false, false);
        //LAS CASILLAS PARPADEAN
        for (j = 1; j < colorBlink.Count; j++)
        {
            Color c1 = colorBlink[j - 1];
            Color c2 = colorBlink[j];
            myPlayer.SetColor(f, c1);
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                myPlayer.SetColor(f, c1);
                myPlayer.SetColorN(f, c1);
                yield return new WaitForSeconds(seg / repeticiones);
                myPlayer.SetColor(f, c2);
                myPlayer.SetColorN(f, c2);
                yield return new WaitForSeconds(seg / repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        yield return new WaitForSeconds(0.02f);
        fallenFloors.Enqueue(f);
    }
    private IEnumerator FallFloor(Floor f)
    {
        //LAS CASILLAS SE CAEN POR ACCIÓN DE LA GRAVEDAD
        myPlayer.Cinematic(f);
        yield return new WaitForSeconds(0.5f);
        myPlayer.Fall(f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //LOS myPlayer QUE SE ENCUENTREN EN ESAS CASILLAS SE CAERAN
        if (myPlayer.actualFloor.Equals(f)) myPlayer.Caer();
    }
    private IEnumerator BlinkBackground(int row, float seg, int repeticiones)
    {
        int j;
        //LAS CASILLAS PARPADEAN
        for (j = 1; j < colorBlink.Count; j++)
        {
            Color c1 = colorBlink[j - 1];
            Color c2 = colorBlink[j];
            myPlayer.SetColorBackground(row, c1);
            yield return new WaitForSeconds(seg);
            for (int i = 0; i < repeticiones; i++)
            {
                myPlayer.SetColorBackground(row, c1);
                yield return new WaitForSeconds(seg / repeticiones);
                myPlayer.SetColorBackground(row, c2);
                yield return new WaitForSeconds(seg / repeticiones);
            }
        }
        yield return new WaitForSeconds(seg);
        myPlayer.CinematicBackground(row);
    }
    private IEnumerator DestroyRows()
    {
        yield return new WaitForEndOfFrame();
        int numrep = numRows;
        for (int i = numrep; i >= 0; i--)
        {
            yield return new WaitForSeconds(duracion / 5);
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
        StartCoroutine(DestroyRows());
        StartCoroutine(SpawnPowerUps());
    }
    public void SpawnPowerUp(float time)
    {
        bool seguir = true;
        Floor f = null;
        if (numRows < 3) { return; }
        while (seguir)
        {
            seguir = false;
            int i = Random.Range(0, numRows);
            int j = Random.Range(0, casillas[i].Length);
            f = casillas[i][j];
            for (int a = 0; a < playerControllers.Count && !seguir; a++)
            {
                if (playerControllers[a].actualFloor.Equals(f)) seguir = true;
            }
            if (myPlayer.actualFloor.Equals(f)) seguir = true;
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
        if (f != null) FindObjectOfType<PhotonInstanciate>().my_player.
               GetComponent<PlayerController>().SetPowerUpFloor(f, Floor.Type.Vacio);
    }
    #endregion
}
