using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombaColorManager : MonoBehaviour
{
    public GameObject bomba;
    [HideInInspector]
    public Floor target;
    private Vector3 localPos;
    private Vector3 initialPos;
    private float currentTime;
    // Start is called before the first frame update

    public void StartAnimation(Floor target, float delay)
    {
        this.target = target;
        GetComponent<PlayerController>().SetAreaBombaColor(target);
        localPos = bomba.transform.localPosition;
        initialPos = bomba.transform.position;
        StartCoroutine(LanzarBomba(delay));
    }

    IEnumerator LanzarBomba(float delay)
    {
        while(currentTime < delay)
        {
            bomba.transform.position = Vector3.Lerp(initialPos, target.transform.position, currentTime/delay);
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        currentTime = 0;
        bomba.transform.position = transform.TransformPoint(localPos);
        GetComponent<PlayerController>().SetAreaBombaColorNormal(target);
    }
}
