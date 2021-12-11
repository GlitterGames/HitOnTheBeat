using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneTutorialPhoto : MonoBehaviour
{
    public int posicion;
    public Image imagen;
    public List<Sprite> sprites;
    void Start()
    {
        posicion = 1;
    }
    #region Eventos
    public void GoToNext()
    {
        posicion++;
        imagen.sprite = sprites[posicion + 1];
    }
    public void GoToBack()
    {
        posicion--;
        imagen.sprite = sprites[posicion + 1];
    }
    public void GoBack() {
        
    }
    #endregion
}
