using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShake : MonoBehaviour
{
    public float intensity = 0.5f;
    private CinemachineTargetGroup target;
    private float pendingShakeDuration = 0f;
    private bool isShaking = false;

    private void Start()
    {
        target = GetComponent<CameraController>().Target;
    }


    [ContextMenu("Shake")]
    public void TestShake()
    {
        Shake(2);
    }

    public void Shake(float duration)
    {
        if (duration > 0)
        {
            pendingShakeDuration += duration;
            if(!isShaking) StartCoroutine(DoShake());
        }
    }

    IEnumerator DoShake()
    {
        isShaking = true;
        float lastShake = 0;
        var startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + pendingShakeDuration)
        {
            Vector3 rot = target.transform.rotation.eulerAngles;
            rot.z -= lastShake;
            lastShake = Random.Range(-1, 1f) * intensity;
            rot.z += lastShake;
            target.transform.rotation = Quaternion.Euler(rot);
            yield return null;
        }
        Vector3 lastRot = target.transform.rotation.eulerAngles;
        lastRot.z -= lastShake;
        target.transform.rotation = Quaternion.Euler(lastRot);
        pendingShakeDuration = 0f;
        isShaking = false;
    }
}
