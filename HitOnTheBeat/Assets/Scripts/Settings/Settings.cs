using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    public void goBack()
    {
        SceneManager.LoadScene(1);
    }
}
