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

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [SerializeField] private AttackScript attackScript;
    [SerializeField] private PlayerDeath deathScript;

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
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
    }

    private void Start()
    {
        canLandTimer = canLandCount;
    }

    private void Update()
    {
        //JUMP BUFFER
        if (Keyboard.current.spaceKey.IsActuated(.4f))
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

        //SPRITE DIRECTION
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
        //WALKING & RUNNING
        moveDir = playerMovement.ReadValue<Vector2>();
        if (!attackScript.pKnockBack)
        {
            rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);
            if (Keyboard.current.shiftKey.IsActuated(.4f))
            {
                rb.velocity = new Vector2(moveDir.x * runSpeed, rb.velocity.y);
            }
        }

        //JUMPING
        if (jumpBuffTime > 0f && coyoteTime > 0f)
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

        //ANIMATION
        UpdateMovementAnimation();
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
