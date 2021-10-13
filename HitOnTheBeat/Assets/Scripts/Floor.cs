using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    #region Atributes 
    private static int CURRENT_ID = 0;
    public int id;
    public Floor east;
    public Floor west;
    public Floor north_east;
    public Floor north_west;
    public Floor south_east;
    public Floor south_west;

    public Floor getEast() {
        return east;
    }
    public void setEast(Floor _east)
    {
        east = _east;
    }
    public Floor getWest()
    {
        return west;
    }
    public void setWest(Floor _west)
    {
       west = _west;
    }
    public Floor getNorth_east()
    {
        return north_east;
    }
    public void setNorth_east(Floor _north_east)
    {
        north_east = _north_east;
    }
    public Floor getNorth_west()
    {
        return north_west;
    }
    public void setNorth_west(Floor _north_west)
    {
        north_west = _north_west;
    }
    public Floor getSouth_east()
    {
        return south_east;
    }
    public void setSouth_east(Floor _south_east)
    {
        south_east = _south_east;
    }
    public Floor getSouth_west()
    {
        return south_west;
    }
    public void setSouth_west(Floor _south_west)
    {
        south_west = _south_west;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        id = CURRENT_ID++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 GetFloorPosition()
    {
        return (Vector3.right * transform.position.x + Vector3.forward * transform.position.z);
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
