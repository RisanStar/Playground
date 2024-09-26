using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackScript : MonoBehaviour
{
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerAttack;

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [SerializeField] private PlayerMovement playerMove;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    private bool canAttack;
    private float attackRange;
    [SerializeField] private float canAttackCount;
    private float canAttackTimer;
    private bool inRange;
    [SerializeField] private GameObject enemy;
    public bool pKnockBack { get; private set; }
    [SerializeField] private float pKnockBackPower;
    [SerializeField] private float pKnockBackCount;
    private float pKnockBackTimer;

    private void Awake()
    {
        playerCntrls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerAttack = playerCntrls.Player.Attack;
        playerAttack.Enable();
    }

    private void OnDisable()
    {
        playerAttack.Disable();
    }

    private void Start()
    {
        canAttackTimer = canAttackCount;
        pKnockBackTimer = pKnockBackCount;

        canAttack = true;
        pKnockBack = false;
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.right * 5, Color.green);
        //Debug.Log(inRange);
        //RANGE
        if (attackRange <= 1.2f)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
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
        playerAttack.performed += Attack;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 5, ~ignoreCol);
        if (hit.collider.CompareTag("Enemy"))
        {
            attackRange = hit.point.x - transform.position.x;
            //Debug.Log(attackRange);
        }
        else
        {
            Debug.Log("Player not found!");
        }

        if (pKnockBack)
        {
            if (playerMove.moveDir.x < 0f)
            {
                rb.AddForce(Vector2.right * pKnockBackPower, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.left * pKnockBackPower, ForceMode2D.Impulse);
            }
        }
    }
    private void Attack(InputAction.CallbackContext attack)
    {
        if (attack.performed)
        {
            if (inRange)
            {
                pKnockBack = true;
            }
        }
    }

    private void LateUpdate()
    {
        UpdateAttackAnimation();
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
