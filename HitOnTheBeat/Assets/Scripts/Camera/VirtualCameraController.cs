using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class VirtualCameraController : MonoBehaviour
{
    public float mainDistance = 400f;
    public float mainAngle = 30f;
    public bool invertVertical = false;
    public bool invertHorizontal = false;
    private CinemachineTransposer transposer;
    private CinemachineTargetGroup target;
    private bool isMoving = false;
    private Vector3 auxPos;
    private float startTime;

    //Eventos para cada movimiento (ocultos)
    public UnityEvent atStartEvent;
    public UnityEvent delayEvent;
    public UnityEvent atEndEvent;

    // Start is called before the first frame update
    void Awake()
    {
        transposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        target = FindObjectOfType<CinemachineTargetGroup>();
        SetPos(mainDistance, mainAngle, 0, 0);
    }

    public void SetPos(float d, float r, float timeToGet, float delay)
    {
        StopAllCoroutines();
        if (timeToGet <= 0 && isMoving == false)
        {
            if(!invertVertical) transposer.m_FollowOffset.y = d * Mathf.Sin(Mathf.Deg2Rad * r);
            else transposer.m_FollowOffset.y = - d * Mathf.Sin(Mathf.Deg2Rad * r);
            if (!invertHorizontal) transposer.m_FollowOffset.z = -d * Mathf.Cos(Mathf.Deg2Rad * r);
            else transposer.m_FollowOffset.z = d * Mathf.Cos(Mathf.Deg2Rad * r);
        }
        else
        {
            isMoving = true;
            startTime = Time.realtimeSinceStartup;
            auxPos = transposer.m_FollowOffset;
            atStartEvent.Invoke();
            StartCoroutine(DoSetPos(new Vector3(transposer.m_FollowOffset.x, d * Mathf.Sin(Mathf.Deg2Rad * r),
                -d * Mathf.Cos(Mathf.Deg2Rad * r)), timeToGet, delay));
        }
    }

    IEnumerator DoSetPos(Vector3 end, float timeToGet, float delay)
    {
        while (Vector3.Distance(transposer.m_FollowOffset, end) > 0.05)
        {
            transposer.m_FollowOffset = Vector3.Lerp(auxPos, end, (Time.realtimeSinceStartup - startTime) / timeToGet);
            yield return new WaitForEndOfFrame();
        }

        delayEvent.Invoke();
        if (delay > 0) yield return new WaitForSeconds(delay);

        transposer.m_FollowOffset = end;
        isMoving = false;
        atEndEvent.Invoke();
    }
    public bool GetMovingState()
    {
        return isMoving;
    }

    public void SetMovingState(bool state)
    {
        isMoving = state;
    }

    public void EraseAllEvents()
    {
        atStartEvent.RemoveAllListeners();
        delayEvent.RemoveAllListeners();
        atEndEvent.RemoveAllListeners();
    }

    public void SetTarget(Transform[] transform)
    {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        foreach(Transform t in transform)
        {
            CinemachineTargetGroup.Target my_target = new CinemachineTargetGroup.Target();
            my_target.target = t;
            my_target.weight = 1f;
            targets.Add(my_target);
        }
        target.m_Targets = targets.ToArray();
    }
    public void SetTarget(Transform transform)
    {
        CinemachineTargetGroup.Target[] targets = new CinemachineTargetGroup.Target[1];
        CinemachineTargetGroup.Target my_target = new CinemachineTargetGroup.Target();
        my_target.target = transform;
        my_target.weight = 1f;
        targets[0] = my_target;
        target.m_Targets = targets;
    }
}
