using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Floor : MonoBehaviour
{
    private Renderer r;
    private Color normal;
    #region Atributes 
    public int index;
    public int row;
    public Floor[] adyacentes = new Floor[6];
    public enum Type {
        Vacio,
        RitmoDuplicado,
        Escudo
    }
    private Type type = Type.Vacio;
    public Coroutine powertime;
    public Floor GetFloor(FloorDetectorType type) {
        switch (type)
        {
            case FloorDetectorType.East:
                return adyacentes[0];
            case FloorDetectorType.West:
                return adyacentes[1];
            case FloorDetectorType.North_east:
                return adyacentes[2];
            case FloorDetectorType.North_west:
                return adyacentes[3];
            case FloorDetectorType.South_east:
                return adyacentes[4];
            case FloorDetectorType.South_west:
                return adyacentes[5];
        }
        return null;
    }
    public Floor GetInverseFloor(FloorDetectorType type)
    {
        switch (type)
        {
            case FloorDetectorType.East:
                return adyacentes[1];
            case FloorDetectorType.West:
                return adyacentes[0];
            case FloorDetectorType.North_east:
                return adyacentes[5];
            case FloorDetectorType.North_west:
                return adyacentes[4];
            case FloorDetectorType.South_east:
                return adyacentes[3];
            case FloorDetectorType.South_west:
                return adyacentes[2];
        }
        return null;
    }
    public FloorDetectorType GetInverseDireccion(FloorDetectorType type)
    {
        switch (type)
        {
            case FloorDetectorType.East:
                return FloorDetectorType.West;
            case FloorDetectorType.West:
                return FloorDetectorType.East;
            case FloorDetectorType.North_east:
                return FloorDetectorType.South_west;
            case FloorDetectorType.North_west:
                return FloorDetectorType.South_east;
            case FloorDetectorType.South_east:
                return FloorDetectorType.North_west;
            case FloorDetectorType.South_west:
                return FloorDetectorType.North_east;
        }
        return FloorDetectorType.East;
    }
    public Floor GetEast() {
        return adyacentes[0];
    }
    public void SetEast(Floor _east)
    {
        adyacentes[0] = _east;
    }
    public Floor GetWest()
    {
        return adyacentes[1];
    }
    public void SetWest(Floor _west)
    {
       adyacentes[1] = _west;
    }
    public Floor GetNorth_east()
    {
        return adyacentes[2];
    }
    public void SetNorth_east(Floor _north_east)
    {
        adyacentes[2] = _north_east;
    }
    public Floor GetNorth_west()
    {
        return adyacentes[3];
    }
    public void SetNorth_west(Floor _north_west)
    {
        adyacentes[3] = _north_west;
    }
    public Floor GetSouth_east()
    {
        return adyacentes[4];
    }
    public void SetSouth_east(Floor _south_east)
    {
        adyacentes[4] = _south_east;
    }
    public Floor GetSouth_west()
    {
        return adyacentes[5];
    }
    public void SetSouth_west(Floor _south_west)
    {
        adyacentes[5] = _south_west;
    }
    public Floor[] GetAdyacentes() {
        return adyacentes;
    }
    public void SetPower(Type t, bool cogido, bool soyYo)
    {
        Material m = FindObjectOfType<GameManager>().materiales.normal;
        this.type = t;
        switch (t)
        {
            case Type.Vacio:
                m = FindObjectOfType<GameManager>().materiales.normal;
                break;
            case Type.RitmoDuplicado:
                m = FindObjectOfType<GameManager>().materiales.X2;
                break;
            case Type.Escudo:
                m = FindObjectOfType<GameManager>().materiales.escudo;
                break;
        }
        if (t != Type.Vacio) {
            this.GetComponent<Renderer>().material = m;
        } 
        else if (!(t == Type.Vacio && cogido && soyYo))
        {
            this.GetComponent<Renderer>().material = m;
            SetColor(GetColorN());
        }
    }
    public Type GetPower()
    {
        return type;
    }
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        r = this.GetComponentInChildren<Renderer>();
        powertime = null;
    }

    public Vector3 GetFloorPosition()
    {
        return (Vector3.right * transform.position.x + Vector3.forward * transform.position.z);
    }
    public void SetColor(Color c) {
        r.material.SetColor("_BaseColor", c);
    }
    public void SetColorN(Color c)
    {
        normal = c;
    }
    public Color GetColor() {
        return r.material.color;
    }
    public Color GetColorN()
    {
        return normal;
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (other == (object)this) return true;
        if (other.GetType() != this.GetType()) return false;

        Floor otro = (Floor)other;

        if (index == otro.index && row == otro.row) return true;
        else return false;
    }
    public override int GetHashCode()
    {
        if (row == 0) return 0;
        else return row * (row * 6) + index - 5;
    }

    public override string ToString()
    {
        return "CASILLA[Anillo: " + row + ". Indice: " + index + ".]"; 
    }
}
