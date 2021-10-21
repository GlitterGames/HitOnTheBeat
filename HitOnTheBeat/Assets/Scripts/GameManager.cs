using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    const int NUM_CASILLAS = 2;
    const int DESTROY_TIME = 5;
    public static Color casillaAct = Color.blue;
    public static Color casillaAdy = Color.cyan;
    private List<List<Floor>> casillas = new List<List<Floor>>();
    private List<Color> color = new List<Color>();
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        initColor();
        initCasillas();
        casillasSetColor();
    }
    private void initColor() {
        color.Add(Color.black);
        color.Add(Color.grey);
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
        foreach (Floor floor in f)
        {
            int posicion = floor.row;
            casillas[posicion].Add(floor);
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

    public Vector3 spawnPowertUp()
    {
        int i = Random.Range(0, casillas.Count);
        int j = Random.Range(0, casillas[i].Count);
        
        return casillas[i][j].GetFloorPosition() + new Vector3(0,1.0f,0);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
