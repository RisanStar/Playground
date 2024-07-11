using System.Collections;
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

    [Header("Moving")]
    private Vector2 moveDir;
    [SerializeField] private float speed;

    [Header("Jumping")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpMulti;
    [SerializeField] private float gravity;

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
    private void Update()
    {
        UpdateAnimation();


        if (rb.velocity.y > 0f && IsGrounded())
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
        playerJump.performed += Jump;
      

        moveDir = playerMovement.ReadValue<Vector2>();

        rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);

    }

    private void Jump(InputAction.CallbackContext jump)
    {
        if (jump.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }  
    } 

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .1f, ground);
    }

    private void UpdateAnimation()
    {
        AnimState state;
        if (IsGrounded())
        {
            anim.SetBool("Grounded", true);
        }
        if (rb.velocity.x > 0f || rb.velocity.x < 0f)
        {
            state = AnimState.running;
        }
        else
        {
            state = AnimState.idle;
        }

        if (playerJump.WasPerformedThisFrame())
        {
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
        }


        
        anim.SetInteger("AnimState", (int)state);
    }
}
