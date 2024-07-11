using System.Collections;
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

    [Header("Walking & Running")]
    private Vector2 moveDir;
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
    }

    private void Update()
    {
        if (rb.velocity.y > 0f && !IsGrounded())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravity - 1) * Time.deltaTime;
        }

        if (rb.velocity.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x > 0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        //WALKING
        moveDir = playerMovement.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);

        //EXTRA 
        playerJump.performed += Jump;
        playerRun.performed += Run;
    }

    private void LateUpdate()
    {
        UpdateAnimation();
    }

    private void Jump(InputAction.CallbackContext jump)
    {
        if (jump.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }  
    } 

    private void Run(InputAction.CallbackContext run)
    {
        if (run.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x * runSpeed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .05f, ground);
    }

    private void UpdateAnimation()
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
