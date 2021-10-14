using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Button atacar;

    // Start is called before the first frame update
    void Start()
    {
        atacar.onClick.AddListener(() => DoAtacar());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DoAtacar()
    {
        Debug.Log("Atacar");
    }
}
