using UnityEngine;
using Pathfinding;
public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D pRb;
    [SerializeField] private GameObject pGo;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private AIPath aiPath;
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Player_Attack pAttack;
    [SerializeField] private Enemy_Death eneDeath;
    [SerializeField] private Enemy_Attack eneAttack;

    [Header("Walking & Running")]
    [SerializeField] private float speed;
    public bool moving { get; private set; }

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    private Vector3 gravity;

    [Header("Damage")]
    private bool eKnockBack;
    [SerializeField] private float eKnockBackPower;
    [SerializeField] private float pIFrames;
    private float pIFramesCount;


    public LayerMask ignoreCol;

    private enum AnimState { idle, running, jumping, }
    private void Start()
    {
        eKnockBack = false;
    }
    private void Update()
    {
        //E-RAYCAST
        Debug.DrawRay(transform.position, Vector2.left * 5, Color.red);
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (lefthit)
        {
            if (lefthit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            spriteRenderer.flipX = false;
        }

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

        if (playerMove.canRoll)
        {
            pIFramesCount = pIFrames;
            hitbox.enabled = false;
        }
        else
        {
            pIFramesCount -= Time.deltaTime;
        }
   
        if (pIFramesCount <= 0) { pIFramesCount = 0; }
        if (pIFramesCount == 0)
        {
            hitbox.enabled = true;
        }

        UpdateMovementAnimation();
    }
   private void FixedUpdate()
    {
        if (!eneDeath.eIsDead)
        {
            if (!IsGrounded())
            {
                transform.position += gravity * Time.deltaTime;
            }

            if (eKnockBack)
            {

            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }


    private void UpdateMovementAnimation()
    {
        //LAND ANIM
        if (!eneDeath.eIsDead)
        {
            AnimState state;

            if (IsGrounded())
            {
                anim.SetBool("Grounded", true);
            }

            //RUN & IDLE ANIM
            if (!aiPath.reachedEndOfPath)
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
}
