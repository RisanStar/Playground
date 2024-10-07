using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerJump;
    [SerializeField] private InputAction playerRoll;

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

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
    private float rollTimeCount;

    public bool canRoll { get; private set;}
    private IEnumerator ra;

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

        if (playerRoll.WasPressedThisFrame() && IsGrounded())
        {
            canRoll = true;
            rollTimeCount = rollTime;
        }
        else
        {
            canRoll = false;
            rollTimeCount -= Time.deltaTime;
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
        //WALKING & RUNNING
        moveDir = playerMovement.ReadValue<Vector2>();

        if (!attackScript.pKnockBack && !deathScript.pIsDead && !canRoll)
        {
            rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);

            if (Keyboard.current.shiftKey.IsActuated(.4f))
            {
                rb.velocity = new Vector2(moveDir.x * runSpeed, rb.velocity.y);
            }
        }

        //JUMPING
        if (jumpBuffTime > 0f && coyoteTime > 0f && !canRoll)
        {
           rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        //COYOTE & LANDING
        if (!IsGrounded())
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
        
        if (rollTimeCount > 0)
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
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
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

            //ROLL ANIM
            if (canRoll)
            {
                StartCoroutine(ra);
            }
            else
            {
                StopCoroutine(ra);
            }


            //JUMP ANIM
            if (rb.velocity.y > 0 && !IsGrounded())
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
