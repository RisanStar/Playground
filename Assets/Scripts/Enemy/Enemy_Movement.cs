using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject pGo;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Walking & Running")]
    [SerializeField] private float speed;

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;

    [Header("Taking Damage")]
    [SerializeField] private Player_Attack pAttack;
    private bool eKnockBack;
    [SerializeField] private float eKnockBackPower;

    public LayerMask ignoreCol;

    private enum AnimState { idle, running, jumping, }
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

        if (transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        //E-KNOCKBACK
        if (pAttack.pKnockBack)
        {
            eKnockBack = true;
        }
        else
        {
            eKnockBack = false;
        }
        
       UpdateMovementAnimation();
    }
    private void FixedUpdate()
    {
        //E-RAYCAST
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (hit)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hitting player");
            }
        }
        else
        {
            return;
        }

        if (eKnockBack)
        {
            rb.AddForce(Vector2.right * eKnockBackPower, ForceMode2D.Impulse);
        }

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }

    private void UpdateMovementAnimation()
    {
        AnimState state;
        //LAND ANIM
        if (IsGrounded())
        {
            anim.SetBool("Grounded", true);
            anim.SetBool("canLand", false);
        }

        //RUN & IDLE ANIM
        if (rb.velocity.x > 0f || rb.velocity.x < 0f && IsGrounded())
        {
            state = AnimState.running;
        }
        else
        {
            state = AnimState.idle;
        }

        anim.SetInteger("AnimState", (int)state);
    }
}
