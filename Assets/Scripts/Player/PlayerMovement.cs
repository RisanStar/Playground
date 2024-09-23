using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerMovement;
    [SerializeField] private InputAction playerJump;
    [SerializeField] private InputAction playerRun;

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [SerializeField] private AttackScript attackScript;

    public Vector2 moveDir { get; private set; }

    [Header("Walking & Running")]
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpMulti;
    [SerializeField] private float gravity;
    [SerializeField] private float canLandCount;
    private float canLandTimer;
    [SerializeField] private float coyoteJump;
    [SerializeField] private float jumpBuffTime;
    [SerializeField] private bool jumped;

    private enum AnimState {idle, running, jumping, rolling}
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

        playerRun = playerCntrls.Player.Run;
        playerRun.Enable();

    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        playerRun.Disable();
    }

    private void Start()
    {
        canLandTimer = canLandCount;
<<<<<<< HEAD

        canAttackTimer = canAttackCount;
        knockbackTimer = knockbackCount;

        canAttack = true;
        pKnockBack = false;
=======
>>>>>>> 282a9153908e275803e650fa0c8c690447515b0f
    }

    private void Update()
    {
<<<<<<< HEAD
        attackMoment.x = transform.position.x - enemy.transform.position.x;

        if (!IsGrounded())
=======
        //JUMPING
        if (rb.velocity.y > 0f && !IsGrounded())
>>>>>>> 282a9153908e275803e650fa0c8c690447515b0f
        {
            coyoteJump -= Time.deltaTime;
            if (rb.velocity.y > 0f)
            {
                jumped = true;
                rb.velocity += Vector2.up * Physics2D.gravity.y * (gravity - 1) * Time.deltaTime;
            }
        }
        else
        {
            coyoteJump = .2f;
            jumped = false;
        }

        if (Keyboard.current.spaceKey.IsActuated(.4f))
        {
            jumpBuffTime = .2f;
        }
        else
        {
            jumpBuffTime -= Time.deltaTime;
        }

        //MOVING
        if (moveDir.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDir.x > 0f)
        {
            spriteRenderer.flipX = false;
        }

    }

    private void FixedUpdate()
    {
        //WALKING
        moveDir = playerMovement.ReadValue<Vector2>();
        if (!attackScript.pKnockBack)
        {
            rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);
        }

        playerJump.performed += Jump;
        playerRun.performed += Run;
    }

    private void LateUpdate()
    {
        //ANIMATION
        UpdateMovementAnimation();
    }

    private void Jump(InputAction.CallbackContext jump)
    {
        if (coyoteJump > 0 && jumpBuffTime > 0 && jumped != true)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            coyoteJump = 0;
            jumpBuffTime = 0f;
        }
    } 

    private void Run(InputAction.CallbackContext run)
    {
        if (IsGrounded())
        {
            moveDir = new Vector2(rb.velocity.x * runSpeed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }

    private void UpdateMovementAnimation()
    {
        AnimState state;
        if (IsGrounded())
        {
            anim.SetBool("Grounded", true);
            anim.SetBool("canLand", false);
        }
        if (rb.velocity.x > 0f || rb.velocity.x < 0f && IsGrounded())
        {
            state = AnimState.running;
        }
        else
        {
            state = AnimState.idle;
        }

        if (rb.velocity.y > 0 && !IsGrounded())
        {
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
            canLandTimer -= 1 * Time.deltaTime;
            if (canLandTimer <= 0) {canLandTimer = 0;}
            if (canLandTimer == 0)
            {
                anim.SetBool("canLand", true);
            }
        }

        anim.SetInteger("AnimState", (int)state);
    }
}
