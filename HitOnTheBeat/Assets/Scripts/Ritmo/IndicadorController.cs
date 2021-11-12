using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class IndicadorController : MonoBehaviour
{
    public float newScale = 1;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetSize(float size)
    {
        transform.localScale = Vector3.one * size * newScale;
    }
}
