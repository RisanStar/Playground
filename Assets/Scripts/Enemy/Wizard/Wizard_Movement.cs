using UnityEngine;
using Pathfinding;
public class Wizard_Movement : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Rigidbody2D pRb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Scripts")]
    [SerializeField] private AIPath aiPath;
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Player_RealAttack pRAttack;
    [SerializeField] private Wizard_Death wizDeath;
    [SerializeField] private Wizard_Attack wizAttack;
    public bool moving { get; private set; }

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    private Vector3 gravity;

    [Header("Damage")]
    private bool eKnockBack;
    [SerializeField] private float eKnockBackPower;

    [Header("Layers")]
    public LayerMask ignoreCol;

    private enum AnimState { idle, cIdle, running, jumping, }
    private void Start()
    {
        eKnockBack = false;
    }
    private void Update()
    {
        //E-RAYCAST
        Debug.DrawRay(transform.position, Vector2.left * 5, Color.red);
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, Vector2.right, 5, ~ignoreCol);
        if (lefthit)
        {
            if (lefthit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = false;
            }
        }
        else if (righthit)
        {
            if (righthit.collider.CompareTag("Player"))
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            return;
        }

        //gravity += Physics.gravity * Time.deltaTime;

        //E-KNOCKBACK
        if (pRAttack.pKnockBack)
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
        if (!wizDeath.eIsDead)
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
        if (!wizDeath.eIsDead)
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
