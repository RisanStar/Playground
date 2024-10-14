using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private float pIFrames;
    private float pIFramesCount;

    private void FixedUpdate()
    {
        if (playerMove.canRoll)
        {
            pIFramesCount = pIFrames;
            Physics2D.IgnoreLayerCollision(3, 2);
        }
        else
        {
            pIFramesCount -= Time.deltaTime;
        }

        if (pIFramesCount <= 0) { pIFramesCount = 0; }
        if (pIFramesCount == 0)
        {

        }
    }
}
