using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    public Renderer body;
    public Material skin;
    public Material alphaSkin;
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
        //Manejo de VFX.
        StopAllCoroutines();
        StartCoroutine(VisibilityVFX(valor));
    }
    private void OnSetAlpha(float valor)
    {
        if (valor >= 1) body.material = skin;
        else
        {
            Color colorBody = alphaSkin.color;
            colorBody.a = valor;
            alphaSkin.color = colorBody;
            body.material = alphaSkin;
        }
        foreach (Renderer r in objects)
        {
            if (valor >= 1)
            {
                r.enabled = true;
            }
            else
            {
                r.enabled = false;
            }
        }
        foreach (CanvasRenderer r in uiObjects) r.GetComponent<CanvasRenderer>().SetAlpha(valor);
    }

    private IEnumerator VisibilityVFX(bool activate)
    {
        float currentValue;
        if (activate)
        {
            currentValue = -1;
            while (currentValue < 1)
            {
                currentValue += Time.deltaTime;
                if (currentValue > 1) currentValue = 1;
                body.material.SetFloat("DisolveValue", currentValue);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            currentValue = 1;
            while (currentValue > -1)
            {
                currentValue -= Time.deltaTime;
                if (currentValue < -1) currentValue = -1;
                body.material.SetFloat("DisolveValue", currentValue);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
