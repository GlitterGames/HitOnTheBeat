using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayersForce : MonoBehaviourPun
{
    [SerializeField]
    private Sprite[] imageList = new Sprite[12];
    [SerializeField]
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!photonView.IsMine)
        {
            spriteRenderer.sprite = imageList[0];
        }
        else
        {
            spriteRenderer.sprite = imageList[11];
        }
    }

    public void changePlayerForce(int force)
    {
        if (!photonView.IsMine)
        {
            spriteRenderer.sprite = imageList[force];
        }
    }
}
