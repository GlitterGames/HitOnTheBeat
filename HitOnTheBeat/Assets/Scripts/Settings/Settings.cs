using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    public EfectosSonido efectosSonido;
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
    void Start()
    {
        efectosSonido = GetComponent<EfectosSonido>();
    }
    public void goBack()
    {
        efectosSonido.PlayEffect(1);
        FindObjectOfType<SceneTransitioner>().StartTransition(1, 0.5f);;
    }
}
