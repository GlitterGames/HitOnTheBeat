using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkinSelector : MonoBehaviour
{
    public List<GameObject> skins;
    public List<string> nombre;
    [TextArea(1, 10)]
    public List<string> descripcion;
    public List<bool> purchased;
    public List<int> precio;
    public int selectedSkin;
    public TMP_Text nombreSkinText;
    public TMP_Text descripcionSkinText;
    public TMP_Text precioText;

    private void Start()
    {
        
    }

    // Update is called once per frame
    public void SelectSkin(int index)
    {
        
    }
}
