using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneTutorialPhoto : MonoBehaviour
{
    public int posicion;
    public Image imagen;
    public List<Sprite> sprites;
    public Button back;
    public Button next;
    public GameObject canvas;
    public Text position;
    void Start()
    {
        posicion = 0;
        imagen.sprite = sprites[posicion];
        SetPosition();
        DisableBackButton();
    }
    #region Eventos
    public void GoToNext()
    {
        posicion++;
        imagen.sprite = sprites[posicion];
        if (posicion == sprites.Count-1) DisableNextButton();
        if (posicion == 1) AbleBackButton();
        SetPosition();
    }
    public void GoToBack()
    {
        posicion--;
        imagen.sprite = sprites[posicion];
        if (posicion == 0) DisableBackButton();
        if (posicion == (sprites.Count-2)) AbleNextButton();
        SetPosition();
        
    }
    public void GoBack() {
        canvas.SetActive(false);
    }
    public void GoTutorial()
    {
        canvas.SetActive(true);
    }
    public void AbleBackButton() {
        back.interactable = true;
    }
    public void AbleNextButton() {
        next.interactable = true;
    }
    public void DisableNextButton() {
        next.interactable = false;
    }
    public void DisableBackButton()
    {
        back.interactable = false;
    }
    public void SetPosition() {
        position.text = (posicion+1) + "/" +sprites.Count;
    }
    #endregion
}
