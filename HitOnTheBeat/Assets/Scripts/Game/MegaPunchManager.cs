using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaPunchManager : MonoBehaviour
{
    public GameObject ultimateSmoke;

    // Start is called before the first frame update
    public void SetSmoke(bool active)
    {
        ultimateSmoke.SetActive(active);
    }
}
