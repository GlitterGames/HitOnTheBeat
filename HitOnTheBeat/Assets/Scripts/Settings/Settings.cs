using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    /*private void Awake()
    {
        var noDestruir = FindObjectsOfType<Settings>();

        if (noDestruir.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }*/
    public void goBack()
    {
        FindObjectOfType<SceneTransitioner>().StartTransition(1, 0.5f);;
    }
}
