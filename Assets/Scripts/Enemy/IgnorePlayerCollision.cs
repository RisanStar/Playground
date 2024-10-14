using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private float pIFrames;
    public float pIFramesCount {get; private set;}

    private void FixedUpdate()
    {
        if (playerMove.rollTimeCount > 0)
        {
            pIFramesCount = pIFrames;
            Physics2D.IgnoreLayerCollision(3, 2, true);
        }
        else
        {
            pIFramesCount -= Time.fixedDeltaTime;
            Debug.Log("The iframes count is:" +  pIFramesCount);
        }

        if (pIFramesCount <= 0) { pIFramesCount = 0; }
        if (pIFramesCount == 0)
        {
            Physics2D.IgnoreLayerCollision(3, 2, false);
        }
    }
}
