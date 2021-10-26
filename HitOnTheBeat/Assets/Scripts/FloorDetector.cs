using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDetector : MonoBehaviour
{
    public FloorDetectorType detectorType;
    public Floor f;
    // Start is called before the first frame update
    void Start()
    {
        f = GetComponentInParent<Floor>();
        char[] trim = { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '(', ')'};
        string name = this.name;
        string type = name.TrimEnd(trim);
        
        switch (type) {
            case "East":
                detectorType = FloorDetectorType.East;
                break;
            case "West":
                detectorType = FloorDetectorType.West;
                break;
            case "NorthEast":
                detectorType = FloorDetectorType.North_east;
                break;
            case "NorthWest":
                detectorType = FloorDetectorType.North_west;
                break;
            case "SouthEast":
                detectorType = FloorDetectorType.South_east;
                break;
            case "SouthWest":
                detectorType = FloorDetectorType.South_west;
                break;
            default:
                Debug.LogError("ERROR: Floor colision does not match");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Floor floor = other.GetComponent<Floor>();
        if(!f) f = GetComponentInParent<Floor>();
        if (floor != null)
        {
            switch (detectorType)
            {
                case FloorDetectorType.East:
                    f.setEast(floor);
                    break;
                case FloorDetectorType.West:
                    f.setWest(floor);
                    break;
                case FloorDetectorType.North_east:
                    f.setNorth_east(floor);
                    break;
                case FloorDetectorType.North_west:
                    f.setNorth_west(floor);
                    break;
                case FloorDetectorType.South_east:
                    f.setSouth_east(floor);
                    break;
                case FloorDetectorType.South_west:
                    f.setSouth_west(floor);
                    break;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        Floor floor = other.GetComponent<Floor>();
        if (!f) f = GetComponentInParent<Floor>();
        if (floor != null)
        {
            switch (detectorType)
            {
                case FloorDetectorType.East:
                    f.setEast(null);
                    break;
                case FloorDetectorType.West:
                    f.setWest(null);
                    break;
                case FloorDetectorType.North_east:
                    f.setNorth_east(null);
                    break;
                case FloorDetectorType.North_west:
                    f.setNorth_west(null);
                    break;
                case FloorDetectorType.South_east:
                    f.setSouth_east(null);
                    break;
                case FloorDetectorType.South_west:
                    f.setSouth_west(null);
                    break;
            }
        }
    }
}
public enum FloorDetectorType
{
    East,
    West,
    North_east,
    North_west,
    South_east,
    South_west
}
