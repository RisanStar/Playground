using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Rigidbody2D pRb;
    [SerializeField] private GameObject pGo;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Walking & Running")]
    [SerializeField] private float speed;
    private bool moving;

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    private Vector3 gravity;

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
        Debug.DrawRay(transform.position, Vector2.left * 5, Color.red);

        gravity += Physics.gravity * Time.deltaTime;

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
        if (!IsGrounded())
        {
            transform.position += gravity * Time.deltaTime;
        }

        if (inRange())
        {
            //PLAYER-FOLLOW
            if (Vector2.Distance(transform.position, pGo.transform.position) > 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(pGo.transform.position.x, transform.position.y), speed * Time.deltaTime);
                moving = true;
            }
        }
        else
        {
            moving = false;
        }

        //E-RAYCAST
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (lefthit)
        {
            if (lefthit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = true;
                Debug.Log("Hitting player");
            }
        }
        else
        {
            spriteRenderer.flipX = false;
        }


        if (eKnockBack)
        {
            rb.AddForce(Vector2.right * eKnockBackPower, ForceMode2D.Impulse);
        }

    }

    private bool inRange()
    {
        return Physics2D.OverlapCircle(transform.position, 5f, 7);
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
        if (moving && IsGrounded())
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
