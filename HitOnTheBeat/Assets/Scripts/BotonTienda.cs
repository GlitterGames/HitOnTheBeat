using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonTienda : MonoBehaviour
{
    private SceneTransitioner st;
    public void btn_tienda()
    {
        st.StartTransition(1);
        SceneManager.LoadScene("Tienda");
    }
   
} 
