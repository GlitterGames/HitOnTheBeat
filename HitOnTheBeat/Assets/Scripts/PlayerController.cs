using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Enumerables
    public enum Tipo
    {
        BOXEADORA = 0,
        FANTASMA = 1,
        TERREMOTO = 2,
        BOMBA = 3
    }
    public enum Estado
    {
        NORMAL = 0,
        INVENCIBLE = 1,
        ULTIMATE = 2
    }
    #endregion

    #region Atributes
    public Tipo tipoPersonaje = Tipo.BOXEADORA;
    private Estado estadoActual = Estado.NORMAL;
    public Floor f;
    public Floor antf;
    public FloorDetectorType typeAnt;
    public int fuerza;
    public GameObject playerAvatar;
    private InputController my_input;
    public int id;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        typeAnt = FloorDetectorType.West;
        my_input = new InputController();

        //Se definen las callback del Input.
        my_input.Player.Click.performed += ctx => OnClick();
    }

    //Para que funcione el Input System en la versión actual.
    private void OnEnable()
    {
        my_input.Enable();
    }

    private void OnDisable()
    {
        my_input.Disable();
    }

    //Se ejecuta cuando se realiza click en la pantalla.
    private void OnClick()
    {
        if (id == 0) return; //Provisional prueba multiplayer.
        if (estadoActual == Estado.ULTIMATE && tipoPersonaje == Tipo.BOMBA) return;
        Vector3 screenPos = my_input.Player.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Click realizado sobre casilla.
            Floor targetFloor = hit.transform.GetComponent<Floor>();
            if (targetFloor)
            {
                Floor nextFloor = null;

                if (targetFloor.Equals(f.getNorth_west()))
                {
                    nextFloor = f.getNorth_west();
                    typeAnt = FloorDetectorType.North_west;
                }
                else if (targetFloor.Equals(f.getNorth_east()))
                {
                    nextFloor = f.getNorth_east();
                    typeAnt = FloorDetectorType.North_east;
                }
                else if (targetFloor.Equals(f.getWest()))
                {
                    nextFloor = f.getWest();
                    typeAnt = FloorDetectorType.West;
                }
                else if (targetFloor.Equals(f.getEast()))
                {
                    nextFloor = f.getEast();
                    typeAnt = FloorDetectorType.East;
                }
                else if (targetFloor.Equals(f.getSouth_west()))
                {
                    nextFloor = f.getSouth_west();
                    typeAnt = FloorDetectorType.South_west;
                }
                else if (targetFloor.Equals(f.getSouth_east()))
                {
                    nextFloor = f.getSouth_east();
                    typeAnt = FloorDetectorType.South_east;
                }

                
                //PERFORM MOVEMENT
                if (nextFloor != null)
                {
                    mover(nextFloor);
                }
            }
        }
    }

    #region Moviento
    public void mover(Floor nextFloor)
    {
        setNormalColor();
        transform.position = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
        antf = f;
        f = nextFloor;
        setAreaColor();
    }
    public bool echar(FloorDetectorType type) {
        Floor nextFloor = f.GetFloor(type);
        if (nextFloor != null)
        {
            Debug.Log("Me tenía que mover");
            transform.position = new Vector3(nextFloor.GetFloorPosition().x, transform.position.y, nextFloor.GetFloorPosition().z);
            antf = f;
            f = nextFloor;
            return false;
        }
        else {
            Debug.Log("Me tengo que eliminar");
            //ENVIARLE A LA POSICION DE LA CASILLA "NULL" 
            Floor inverse = f.GetInverseFloor(type);
            Vector3 diferencia = new Vector3(f.GetFloorPosition().x- inverse.GetFloorPosition().x, 0f, f.GetFloorPosition().z-inverse.GetFloorPosition().z);
            transform.position = new Vector3(f.GetFloorPosition().x+diferencia.x, transform.position.y, f.GetFloorPosition().z+diferencia.z);
            //ANIMACION DE ELIMINAR AL JUEGADOR
            eliminarJugador(f);
            f = null;
            return true;
        }
    }
    public void eliminarJugador(Floor nextFloor)
    {
        Debug.Log("Me sali de la pista");
    }
    #endregion

    #region Colores
    private void setNormalColor() {
        f.setColor(f.getColorN());
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++)
        {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(floor.getColorN());
        }
    }
    private void setAreaColor()
    {
        f.setColor(GameManager.casillaAct);
        Floor[] casillasAdy = f.getAdyacentes();
        for (int i = 0; i < casillasAdy.Length; i++) {
            Floor floor = casillasAdy[i];
            if (floor != null) floor.setColor(GameManager.casillaAdy);
        }
    }

    private void SetRangeColor(HashSet<Floor> casillas)
    {
        f.setColor(GameManager.casillaAct);

        foreach (Floor floor in casillas)
        {
            if (floor != null) floor.setColor(GameManager.casillaAttack);
        }
    }
    #endregion

    #region  Ultimates

    public void StartUltimate()
    {

    }

    public void PerformUltimate()
    {
        switch ()
        {

        }
    }

    private HashSet<Floor> GetFloorAreaRange()
    {
        HashSet<Floor> casillas = new HashSet<Floor>();
        Floor[] casillasAdy = f.getAdyacentes();
        foreach (Floor floorAdy in casillasAdy)
        {
            Floor[] casillasArea = floorAdy.getAdyacentes();
            foreach (Floor floorArea in casillasArea)
            {
                casillas.Add(floorArea);
            }
        }
        return casillas;
    }
    #endregion
}
