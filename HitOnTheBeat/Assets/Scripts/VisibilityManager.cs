using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    [SerializeField]
    private bool m_visible = true;
    [SerializeField]
    [Range(0,1)]
    private float m_alpha = 1;
    public bool Visible
    {
        get
        {
            return m_visible;
        }
        set
        {
            if (m_visible != value) OnVisibiltyChange(value);
            m_visible = value;
        }
    }
    public float Alpha
    {
        get
        {
            return m_alpha;
        }
        set
        {
            if (m_alpha != value) OnAlphaChange(value);
            m_alpha = value;
        }
    }

    private delegate void BoolDelegate (bool active);
    private event BoolDelegate OnVisibiltyChange;
    private delegate void FloatDelegate(float alpha);
    private event FloatDelegate OnAlphaChange;
    public List<Renderer> objects;
    public List<CanvasRenderer> uiObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        OnVisibiltyChange += OnVisibilityNewValue;
        OnAlphaChange += OnSetAlpha;
        OnVisibiltyChange(m_visible);
        OnAlphaChange(m_alpha);
    }

    //Únicamente cuando cambia el valor de la visibilidad.
    private void OnVisibilityNewValue(bool valor)
    {
        foreach(Renderer r in objects) r.enabled = valor;
        foreach(CanvasRenderer r in uiObjects) r.GetComponent<CanvasRenderer>().SetAlpha(Convert.ToSingle(valor));
    }
    private void OnSetAlpha(float valor)
    {
        foreach (Renderer r in objects) 
        {
            Color color = r.material.color;
            color.a = valor;
            r.material.color = color;
        }
        foreach (CanvasRenderer r in uiObjects) r.GetComponent<CanvasRenderer>().SetAlpha(valor);
    }
}
