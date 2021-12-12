using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraTargetSwitcher : MonoBehaviour
{
    //Enums
    public enum Mode
    {
        FREE_LOOK = 0,
        MOVING = 1,
        TARGET = 2
    }

    //Referencia
    private CameraController cc;
    [HideInInspector]
    public Mode mode = Mode.FREE_LOOK;

    //Editor
    [ContextMenuItem("Cambiar a Target", "SwitchToTarget")]
    [ContextMenuItem("Cambiar a Vista Libre", "SwitchToFreeCamera")]
    public Transform freeLook;
    public Transform target;
    [Range(0f, 5f)]
    public float timeToGet = 2f;
    [Range(0f, 0.3f)]
    public float deltaDistance = 0.05f;

    private void Awake()
    {
        cc = GetComponent<CameraController>();
    }

    public void SwitchToFreeCamera()
    {
        if (mode == Mode.FREE_LOOK) return;
        if (mode != Mode.MOVING)
        {
            Transform firstTarget = cc.GetFirstTarget();
            freeLook.transform.position = firstTarget.position;
            freeLook.transform.rotation = firstTarget.rotation;
        }
        else
        {
            StopAllCoroutines();
        }
        cc.SetTarget(freeLook);
        mode = Mode.FREE_LOOK;
    }

    public void SwitchToTarget()
    {
        StopAllCoroutines();
        SwitchToFreeCamera();
        StartCoroutine(DoSwitchToTarget());
    }

    //Realiza una transición desde la freelook hasta un objetivo.
    private IEnumerator DoSwitchToTarget()
    {
        mode = Mode.MOVING;
        float startTime = Time.realtimeSinceStartup;
        Vector3 initialPos = freeLook.position;
        Vector3 initialRot = freeLook.rotation.eulerAngles;
        while (Vector3.Distance(freeLook.position, target.position) > deltaDistance)
        {
            freeLook.position = Vector3.Lerp(initialPos, target.position,
                (Time.realtimeSinceStartup - startTime) / timeToGet);
            freeLook.rotation = Quaternion.Euler(Vector3.Lerp(initialRot,
                target.rotation.eulerAngles, (Time.realtimeSinceStartup - startTime) / timeToGet));
            yield return new WaitForEndOfFrame();
        }
        cc.SetTarget(target);
        mode = Mode.TARGET;
    }
}
