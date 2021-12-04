using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using System;

public class CameraController : MonoBehaviour
{
    //Atributos
    [ContextMenuItem("Actualizar Posicion", "UpdatePos")]
    [SerializeField]
    private float m_mainDistance = 1.5f;
    public float MainDistance
    {
        get { return m_mainDistance; }
        set
        {
            m_mainDistance = value;
            if (m_mainDistance < minZoom) m_mainDistance = minZoom;
            else if (m_mainDistance > maxZoom) m_mainDistance = maxZoom;
            SetPos(m_mainDistance, m_mainAngle, timeToGet, timeToZoom);
        }
    }
    [ContextMenuItem("Actualizar Posicion", "UpdatePos")]
    [SerializeField]
    private float m_mainAngle = 45f;
    public float MainAngle
    {
        get { return m_mainAngle; }
        set
        {
            m_mainAngle = value;
            SetPos(m_mainDistance, m_mainAngle, timeToGet, timeToZoom);
        }
    }
    //Editor
    public bool invertVertical = false;
    public bool invertHorizontal = false;
    [Range(0, 0.3f)]
    public float deltaDistance = 0.05f;
    [Range(0, 5)]
    public float timeToGet = 2f;
    [Range(0, 2)]
    public float timeToZoom = 0.5f;
    [Range(0, 1.5f)]
    public float minZoom = 1.5f;
    [Range(1.5f, 40)]
    public float maxZoom = 30;

    //Referencias
    public CinemachineTargetGroup Target { get; private set; }

    // Se llama antes del primer renderizado.
    void Awake()
    {
        Target = FindObjectOfType<CinemachineTargetGroup>();
    }

    private void Start()
    {
        UpdatePos();
    }

    //Establece la nueva posición de la cámara.
    private void UpdatePos()
    {
        SetPos(MainDistance, MainAngle, timeToGet, timeToZoom);
    }
    public void SetPos(float d, float r, float timeToGet, float timeToZoom)
    {
        StopAllCoroutines();
        if (timeToGet <= 0)
        {
            Vector3 newRot = Target.transform.rotation.eulerAngles;
            newRot.x = MainAngle;
            Target.transform.rotation = Quaternion.Euler(newRot);
        }
        else
        {
            StartCoroutine(DoSetRot(MainAngle, timeToGet));
        }
        if(timeToZoom <= 0)
        {
            Target.m_Targets[0].radius = MainDistance;
        }
        else
        {
            StartCoroutine(DoSetZoom(MainDistance, timeToZoom));
        }
    }

    IEnumerator DoSetZoom(float endPos, float timeToZoom)
    {
        float startTime = Time.realtimeSinceStartup;
        float initialDistance = Target.m_Targets[0].radius;
        while (Mathf.Abs(Target.m_Targets[0].radius - endPos) > deltaDistance)
        {
            Target.m_Targets[0].radius =  Mathf.Lerp(initialDistance, endPos, (Time.realtimeSinceStartup - startTime) / timeToZoom);
            yield return new WaitForEndOfFrame();
        }
        Target.m_Targets[0].radius = endPos;
    }

    IEnumerator DoSetRot(float endRot, float timeToGet)
    {
        float startTime = Time.realtimeSinceStartup;
        float initialDistance = Target.m_Targets[0].radius;
        Vector3 currentRot = Target.transform.rotation.eulerAngles;
        float initialRot = currentRot.x;
        while (Mathf.Abs(initialRot - endRot) > deltaDistance)
        {
            float auxDistance = Mathf.Lerp(initialRot, endRot, (Time.realtimeSinceStartup - startTime) / timeToGet);
            currentRot.x = auxDistance;
            Target.transform.rotation = Quaternion.Euler(currentRot);
            yield return new WaitForEndOfFrame();
        }
        currentRot.x = endRot;
        Target.transform.rotation = Quaternion.Euler(currentRot);
    }

    public void SetTarget(Transform[] transform)
    {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        foreach (Transform t in transform)
        {
            CinemachineTargetGroup.Target my_target = new CinemachineTargetGroup.Target();
            my_target.target = t;
            my_target.weight = 1f;
            my_target.radius = MainDistance;
            targets.Add(my_target);
        }
        Target.m_Targets = targets.ToArray();
    }
    public void SetTarget(Transform transform)
    {
        CinemachineTargetGroup.Target[] targets = new CinemachineTargetGroup.Target[1];
        CinemachineTargetGroup.Target my_target = new CinemachineTargetGroup.Target();
        my_target.target = transform;
        my_target.weight = 1f;
        my_target.radius = MainDistance;
        targets[0] = my_target;
        Target.m_Targets = targets;
    }

    public Transform GetFirstTarget()
    {
        return Target.m_Targets[0].target;
    }

    #region Controlador de Pesos
    public void SetWeightTarget(int index, float weight)
    {
        CinemachineTargetGroup.Target[] targets = Target.m_Targets;
        if (index < targets.Length) {
            targets[index].weight = weight;
            Target.m_Targets = targets;
        } else Debug.LogWarning("[CameraController] Posición de target no existe.");
    }
    public void SetAllWeightTarget(float weight)
    {
        CinemachineTargetGroup.Target[] targets = Target.m_Targets;
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].weight = weight;
        }
        Target.m_Targets = targets;
    }
    #endregion
}
