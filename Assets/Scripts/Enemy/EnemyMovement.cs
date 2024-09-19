using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject pGo;

    [Header("Walking & Running")]
    private Vector2 moveDir;
    [SerializeField] private float speed;

    [Header("Taking Damage")]
    private Vector2 pAttackDistance;
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private Attack attackScript;
    private Vector2 playerDis;
    private bool eKnockBack; 
    [SerializeField] private float eKnockBackPower;
    private Vector2 eKnockBackDir;

    public LayerMask ignoreCol;

    private void Start()
    {
        eKnockBack = false;
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.left * 10, Color.red);
        if (Vector2.Distance(transform.position, playerScript.playerPos) > 0f)
        {
            transform.position += transform.position * (Time.deltaTime * speed);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (hit.collider.CompareTag("Player") == true)
        {
            Debug.Log("Hitting player");

        }
        else
        {
            return;
        }
       
    }
    void HitByRay()
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
            pAttackDistance = playerDis.normalized  * eKnockBackPower;
            rb.AddForce(pAttackDistance.normalized, ForceMode2D.Impulse);
        }
    }
}
