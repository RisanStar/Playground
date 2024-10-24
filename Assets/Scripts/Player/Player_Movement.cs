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
    [SerializeField] private Transform[] ledgesPos;

    [Header("Scripts")]
    [SerializeField] private Player_RealAttack playerRAttack;
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
    [SerializeField] private LayerMask ledge;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallSlideSpeed;
    private bool isSliding;
    private bool canWallJump;
    private Transform ledgePos;

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
    }

    private void Update()
    {
        if (!deathScript.pIsDead)
        {
            //WALKING & RUNNING PHYS
            moveDir = playerMovement.ReadValue<Vector2>();

            ra = RollAnim();

            //JUMP BUFFER
            if (Keyboard.current.spaceKey.IsActuated(.4f) && !IsWalled())
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

            //WALL JUMP
            if (Keyboard.current.spaceKey.IsPressed() && IsWalled())
            {
                canWallJump = true;
                isSliding = false;
            }
            else
            {
                canWallJump = false;
                isSliding = true;
            }

            //ROLL BUFFER
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
            if (!IsLedged())
            {
                if (moveDir.x < 0f)
                {
                    spriteRenderer.flipX = true;
                }

                if (moveDir.x > 0f)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }

        float minDist = Mathf.Infinity;
        foreach (Transform t in ledgesPos)
        {
            float dist = Vector2.Distance(t.position, transform.position);

            if (dist < minDist)
            {
                ledgePos = t;
                minDist = dist;
            }
        }
    

        //ANIMATION
        UpdateMovementAnimation();
    }

    private void FixedUpdate()
    {
        //WALKING & RUNNING PHYS
        if (!playerRAttack.pKnockBack && !deathScript.pIsDead && !canRoll)
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

        //WALL JUMPING PHYS
        if (canWallJump)
        {
            var rDiagonal = (new Vector2(moveDir.x, 0) * new Vector2((Time.deltaTime * 5), moveDir.y));
            var lDiagonal = (new Vector2(moveDir.x, 0) * new Vector2((Time.deltaTime * -5), moveDir.y));

            if (moveDir.x < 0f)
            {
                rb.AddForce(rDiagonal * jumpForce, ForceMode2D.Impulse);
            }

            if(moveDir.x > 0f)
            {
                rb.AddForce(lDiagonal * jumpForce, ForceMode2D.Impulse);
            }
        }

        //COYOTE & LANDING PHYS
        if (!IsGrounded() && !canWallJump)
        {
            coyoteTime -= Time.deltaTime;
            if (rb.velocity.y > 0f && !IsLedged())
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
            
            if (moveDir.x > 0f)
            {
                rb.AddForce(Vector2.right * rollForce, ForceMode2D.Impulse);
            }
        }

        //CLIMBING PHYS
        if (IsWalled())
        {
            if (isSliding && !canWallJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            }
        }

        //LEDGE PHYS
        if (IsLedged())
        {
            if (Keyboard.current.spaceKey.IsPressed())
            {
                rb.simulated = true;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                transform.position = ledgePos.position;
                rb.simulated = false;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, .5f, wall);
    }

    private bool IsLedged()
    {
        return Physics2D.OverlapCircle(wallCheck.position, .5f, ledge);
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
            if (rb.velocity.y == 0)
            {
                anim.SetBool("Grounded", true);
                anim.SetBool("canLand", false);
            }
            else
            {
                anim.SetBool("Grounded", false);
                anim.SetBool("canLand", true);
                anim.SetFloat("AirSpeedY", -.1f);
            }

            //RUN & IDLE ANIM
            if (!IsWalled())
            {
                if (rb.velocity.x > 0f || rb.velocity.x < 0f)
                {
                    state = AnimState.running;
                }
                else
                {
                    state = AnimState.idle;
                }

                anim.SetInteger("AnimState", (int)state);
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

            //WALLSLIDE ANIM
            if (IsWalled() && !IsLedged() && rb.velocity.x != 0 )
            {
                anim.SetBool("WallSlide", true);
            }
            else
            {
                anim.SetBool("WallSlide", false);
            }

            //LEDGEGRAB ANIM
            if (IsLedged())
            {
                anim.SetBool("LedgeGrab", true);
            }
            else
            {
                anim.SetBool("LedgeGrab", false);
            }


            //JUMP ANIM
            if (rb.velocity.y > 0 && !IsWalled())
            {
                anim.SetTrigger("Jump");
                canLandTimer -= 1 * Time.deltaTime;
                if (canLandTimer <= 0) { canLandTimer = 0; }
                if (canLandTimer == 0)
                {
                    anim.SetBool("canLand", true);
                }
            }
        }
    }
}
