using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private Vector2 attackMoment;
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private Attack attackScript;
    private Rigidbody2D playerRb;
    private Vector2 playerDis;
    private bool eKnockBack; 
    [SerializeField] private float eKnockBackPower;
    private Vector2 eKnockBackDir;

    private Vector2 heightCorrect;
    public LayerMask ignoreCol;

    private void Start()
    {
        eKnockBack = false;
        heightCorrect = new Vector2(transform.position.x, transform.position.y);
    }
    private void Update()
    {
        Debug.DrawRay(heightCorrect, Vector2.left * 10, Color.red);
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(heightCorrect, Vector2.left, 5, ~ignoreCol);
        if (hit.collider != null)
        {
            playerRb = hit.collider.attachedRigidbody;
            playerDis = transform.position - playerRb.transform.position;
        }
       
    }
    private void HitByRay()
    {
        if (playerScript.pKnockBack)
        {
            eKnockBack = true;
        }
        else
        {
            eKnockBack = false;
        }

        if (eKnockBack)
        {
            rb.AddForce(playerDis.normalized * -500f);
        }
    }
}
