using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{
    [Header("Hitboxes")]
    [SerializeField] private Collider2D playerHB;
    [SerializeField] private Collider2D hitbox;

    [Header("Scripts")]
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Enemy_Death eneDeath;

    [Header("I-Frames")]
    [SerializeField] private float pIFrames;
    public float pIFramesCount {get; private set;}

    private void FixedUpdate()
    {
        if (!eneDeath.eIsDead)
        {

            if (playerMove.rollTimeCount > 0)
            {
                pIFramesCount = pIFrames;
                Physics2D.IgnoreCollision(playerHB, hitbox, true);
            }
            else
            {
                pIFramesCount -= Time.fixedDeltaTime;
                Debug.Log("The iframes count is:" + pIFramesCount);
            }

            if (pIFramesCount <= 0) { pIFramesCount = 0; }
            if (pIFramesCount == 0)
            {
                Physics2D.IgnoreCollision(playerHB, hitbox, false);
            }
        }
    }
}
