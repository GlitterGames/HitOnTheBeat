using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorColor : MonoBehaviour
{
    public Material [] material;
    public GameObject obj;
    Renderer rend;

    public int x;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.material = material[x];
    }

    public void NextMat(){
        if(x == 0)
        {
            x = 1;
        }
        else
        {
            x = 0;
        }
        rend.material = material[x];
    }
}
