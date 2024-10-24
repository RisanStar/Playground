using UnityEngine;
using Pathfinding;
public class Bandit_Movement : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Scripts")]
    [SerializeField] private AIPath aiPath;
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Player_RealAttack pRAttack;
    [SerializeField] private Bandit_Death banDeath;
    [SerializeField] private Bandit_Attack banAttack;
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

        if (aiPath.desiredVelocity.x > 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            spriteRenderer.flipX = true;
        }

        if (aiPath.desiredVelocity.x < 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            spriteRenderer.flipX = false;
        }

        UpdateMovementAnimation();
    }
   private void FixedUpdate()
    {
        if (!banDeath.eIsDead)
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
        if (!banDeath.eIsDead)
        {
            AnimState state;

            if (IsGrounded())
            {
                anim.SetBool("Grounded", true);
            }

            //RUN & IDLE ANIM
            if (aiPath.desiredVelocity.x > 0 || aiPath.desiredVelocity.x < 0)
            {
                state = AnimState.running;
            }
            else if (banAttack.inCombat)
            {
                state = AnimState.cIdle;
            }
            else
            {
                state = AnimState.idle;
            }

            anim.SetInteger("AnimState", (int)state);
        }

    }
}
