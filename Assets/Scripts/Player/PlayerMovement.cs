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
    [SerializeField] private InputAction playerAttack;

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    public Vector2 playerPos {  get; private set; }

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

    [Header("Attacking")]
    private bool canAttack;
    private Vector2 attackDistance; 
    [SerializeField] private float canAttackCount;
    private float canAttackTimer;
    [SerializeField] private Attack attackScript;
    [SerializeField] private GameObject enemy;
    public bool pKnockBack { get; private set; } 
    [SerializeField] private float pKnockBackPower;
    [SerializeField] private float pKnockBackCount;
    private float pKnockBackTimer;

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

        playerAttack = playerCntrls.Player.Attack;
        playerAttack.Enable();
    }

    private void OnDisable()
    {
        playerMovement.Disable();
        playerJump.Disable();
        playerRun.Disable();
        playerAttack.Disable();
    }

    private void Start()
    {
        canLandTimer = canLandCount;
        canAttackTimer = canAttackCount;
        pKnockBackTimer = pKnockBackCount;

        canAttack = true;
        pKnockBack = false;
    }

    private void Update()
    {
        playerPos = transform.position;
        attackDistance.x = transform.position.x - enemy.transform.position.x;

        //JUMPING
        if (rb.velocity.y > 0f && !IsGrounded())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravity - 1) * Time.deltaTime;
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

        //ATTACKING
        if (canAttack)
        {
            canAttackTimer = canAttackCount;
        }

        if (pKnockBack)
        {
            pKnockBackTimer -= 1 * Time.deltaTime;
            if (pKnockBackTimer <= 0) { pKnockBackTimer = 0; }
            if (pKnockBackTimer == 0)
            {
                pKnockBack = false;
                pKnockBackTimer = pKnockBackCount;
            }
        }

    }

    private void FixedUpdate()
    {
        //WALKING
        moveDir = playerMovement.ReadValue<Vector2>();
        if (!pKnockBack)
        {
            rb.velocity = new Vector2(moveDir.x * speed, rb.velocity.y);
        }

        //EXTRA 
        playerJump.performed += Jump;
        playerRun.performed += Run;
        playerAttack.performed += Attack;
    }

    private void LateUpdate()
    {
        //ANIMATION
        UpdateMovementAnimation();
        UpdateAttackAnimation();
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
        if (IsGrounded())
        {
            moveDir = new Vector2(rb.velocity.x * runSpeed, rb.velocity.y);
        }
    }

    private void Attack(InputAction.CallbackContext attack)
    {
        if (attack.performed)
        {
            if (attackScript.inRange)
            {
                pKnockBack = true;
                attackDistance = attackDistance.normalized * pKnockBackPower;
                rb.AddForce(attackDistance, ForceMode2D.Impulse);
            }
            else
            {
                return;
            }
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

    private void UpdateAttackAnimation()
    {
        if (playerAttack.WasPressedThisFrame())
        {
            if (canAttack)
            {
                anim.SetTrigger("Attack1");
                canAttack = false;
            }
            else
            {
                canAttack = true;
            }

            if (!canAttack)
            {
                canAttackTimer -= 1 * Time.deltaTime;
                if (canAttackTimer <= 0) { canAttackTimer = 0; }
                if (canAttackTimer == 0)
                {
                    canAttack = true;
                }
            }
        }
    }
}
