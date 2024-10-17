using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerJump;
    [SerializeField] private InputAction playerRoll;

    [Header("Assets")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Scripts")]
    [SerializeField] private Player_Attack attackScript;
    [SerializeField] private Player_Death deathScript;

    public Vector2 moveDir { get; private set; }

    [Header("Walking & Running")]
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float jumpForce;
    [SerializeField] private float timeHeld;
    [SerializeField] private float jumpMulti;
    [SerializeField] private float gravity;
    [SerializeField] private float canLandCount;
    private float canLandTimer;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffTime;

    [Header("Rolling")]
    [SerializeField] private float rollForce;
    [SerializeField] private float rollTime;
    private float rollForceTime = .4f;
    public float rollTimeCount { get; private set; }
    private bool canRoll;
    private IEnumerator ra;

    [Header("Climbing")]
    [SerializeField] private LayerMask wall;
    private bool canWallClimb;

    private enum AnimState {idle, running, jumping}
    private void Awake()
    {
        playerCntrls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerMovement = playerCntrls.Player.Movement;
        playerMovement.Enable();

        playerJump = playerCntrls.Player.Jump;
        playerJump.Enable();

        playerRoll = playerCntrls.Player.Roll;
        playerRoll.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        playerRoll.Disable();
    }

    private void Start()
    {
        canLandTimer = canLandCount;
        rollForceTime = 0;
        canWallClimb = false;
    }

    private void Update()
    {
        ra = RollAnim();

        //JUMP BUFFER
        if (Keyboard.current.spaceKey.IsActuated(.4f) && !deathScript.pIsDead)
        {
            jumpBuffTime = .4f;
        }
        else
        {
            jumpBuffTime -= Time.deltaTime;
            if (jumpBuffTime < 0)
            {
                jumpBuffTime = 0;
            }
        }

        if (playerRoll.WasPressedThisFrame() && IsGrounded() && rollTimeCount <= 0)
        {
            canRoll = true;
            rollTimeCount = rollTime;
            rollForceTime = .4f;
        }
        else
        {
            canRoll = false;
            rollTimeCount -= Time.deltaTime;
            rollForceTime -= Time.deltaTime;
        }

        //SPRITE DIRECTION
        if (!deathScript.pIsDead)
        {
            if (moveDir.x < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else 
            {
                spriteRenderer.flipX = false;
            }
        }

        //ANIMATION
        UpdateMovementAnimation();
    }

    private void FixedUpdate()
    {
        //WALKING & RUNNING PHYS
        moveDir = playerMovement.ReadValue<Vector2>();

        if (!attackScript.pKnockBack && !deathScript.pIsDead && !canRoll)
        {
            rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);

            if (Keyboard.current.shiftKey.IsActuated(.4f))
            {
                rb.velocity = new Vector2(moveDir.x * runSpeed, rb.velocity.y);
            }
        }

        //JUMPING PHYS
        if (jumpBuffTime > 0f && coyoteTime > 0f && !canRoll)
        {
           rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        //COYOTE & LANDING PHYS
        if (!IsGrounded() && !canWallClimb)
        {
            coyoteTime -= Time.deltaTime;
            if (rb.velocity.y > 0f)
            {
                coyoteTime = 0f;
                rb.velocity += Vector2.up * Physics2D.gravity.y * (gravity - 1) * Time.deltaTime;
            }
        }
        else
        {
            coyoteTime = .2f;
        }
        
        //ROLLING PHYS
        if (rollForceTime > 0)
        {
            if (moveDir.x < 0f)
            {
                rb.AddForce(Vector2.left * rollForce, ForceMode2D.Impulse);
            }
            else 
            {
                rb.AddForce(Vector2.right * rollForce, ForceMode2D.Impulse);
            }
        }

        //CLIMBING PHYS
        if (canWallClimb)
        {
         
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("is True" + collision);
            canWallClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("is false" + collision);
            canWallClimb = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, wall);
    }

    private IEnumerator RollAnim()
    {
        anim.SetTrigger("Roll");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Roll");
    }

    private void UpdateMovementAnimation()
    {
        if (!deathScript.pIsDead)
        {
            AnimState state;
            //LAND ANIM
            if (IsGrounded() && !canWallClimb)
            {
                anim.SetBool("Grounded", true);
                anim.SetBool("canLand", false);
            }
            else
            {
                anim.SetFloat("AirSpeedY", -.1f);
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

            //ROLL ANIM
            if (canRoll)
            {
                StartCoroutine(ra);
            }
            else
            {
                StopCoroutine(ra);
            }

            if (canWallClimb)
            {
                anim.SetBool("WallSlide", true);
            }
            else
            {
                anim.SetBool("WallSlide", false);
            }


            //JUMP ANIM
            if (rb.velocity.y > 0 && !IsGrounded() && !canWallClimb)
            {
                anim.SetTrigger("Jump");
                anim.SetBool("Grounded", false);
                canLandTimer -= 1 * Time.deltaTime;
                if (canLandTimer <= 0) { canLandTimer = 0; }
                if (canLandTimer == 0)
                {
                    anim.SetBool("canLand", true);
                }
            }

            anim.SetInteger("AnimState", (int)state);
        }
    }
}
