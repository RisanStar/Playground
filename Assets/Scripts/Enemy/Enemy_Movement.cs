using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D pRb;
    [SerializeField] private GameObject pGo;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Player_Attack pAttack;
    [SerializeField] private Enemy_Death eneDeath;

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

            if (InRange())
            {
                //PLAYER-FOLLOW
                if (Vector2.Distance(transform.position, pGo.transform.position) > 2)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(pGo.transform.position.x, transform.position.y), speed * Time.deltaTime);
                    moving = true;
                }
                else
                {
                    moving = false;
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
                }
            }
            else
            {
                spriteRenderer.flipX = false;
            }



            if (eKnockBack)
            {

            }
        }

    }

    private bool InRange()
    {
        return Physics2D.OverlapCircle(transform.position, 5f, 7);
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
}