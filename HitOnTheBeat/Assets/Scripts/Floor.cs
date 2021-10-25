using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Floor : MonoBehaviourPun
{
    private Renderer r;
    private Color normal;
    #region Atributes 
    private static int CURRENT_ID = 0;
    public int id;
    Floor[] adyacentes = new Floor[6];
    public int row;

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
    public Floor getEast() {
        return adyacentes[0];
    }
    public void setEast(Floor _east)
    {
        adyacentes[0] = _east;
    }
    public Floor getWest()
    {
        return adyacentes[1];
    }
    public void setWest(Floor _west)
    {
       adyacentes[1] = _west;
    }
    public Floor getNorth_east()
    {
        return adyacentes[2];
    }
    public void setNorth_east(Floor _north_east)
    {
        adyacentes[2] = _north_east;
    }
    public Floor getNorth_west()
    {
        return adyacentes[3];
    }
    public void setNorth_west(Floor _north_west)
    {
        adyacentes[3] = _north_west;
    }
    public Floor getSouth_east()
    {
        return adyacentes[4];
    }
    public void setSouth_east(Floor _south_east)
    {
        adyacentes[4] = _south_east;
    }
    public Floor getSouth_west()
    {
        return adyacentes[5];
    }
    public void setSouth_west(Floor _south_west)
    {
        adyacentes[5] = _south_west;
    }
    public Floor[] getAdyacentes() {
        return adyacentes;
    }
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        r = this.GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetFloorPosition()
    {
        return (Vector3.right * transform.position.x + Vector3.forward * transform.position.z);
    }
    public void setColor(Color c) {
        r.material.color = c;
    }
    public void setColorN(Color c)
    {
        normal = c;
    }
    public Color getColor() {
        return r.material.color;
    }
    public Color getColorN()
    {
        return normal;
    }

    public override bool Equals(object other)
    {
        if (other == null) return false;
        if (other == this) return true;
        if (other.GetType() != this.GetType()) return false;

        Floor otro = (Floor)other;

        if (id == otro.id) return true;
        else return false;
    }
}
