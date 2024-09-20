using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject pGo;

    [Header("Walking & Running")]
    [SerializeField] private float speed;

    [Header("Taking Damage")]
    private Vector2 pAttackDistance;
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private Attack attackScript;
    private bool eKnockBack;
    [SerializeField] private float eKnockBackPower;

    public LayerMask ignoreCol;

    private void Start()
    {
        eKnockBack = false;
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.left * 10, Color.red);

        //PLAYER-FOLLOW
        if (Vector2.Distance(transform.position, pGo.transform.position) > 0f)
        {
            transform.position = Vector2.MoveTowards(transform.position, pGo.transform.position, speed * Time.deltaTime);
        }

        //E-KNOCKBACK
        if (playerScript.pKnockBack)
        {
            eKnockBack = true;
        }
        else
        {
            eKnockBack = false;
        }
        
    } 
   private void FixedUpdate()
   {
        //E-RAYCAST
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (hit.collider.CompareTag("Player") == true)
        {

           Debug.Log("Hitting player");

        }
        else
        {
           return;
        }

        if (eKnockBack)
        {
           pAttackDistance = pAttackDistance.normalized * eKnockBackPower;
           rb.AddForce(pAttackDistance.normalized, ForceMode2D.Impulse);
        }

   }
}
