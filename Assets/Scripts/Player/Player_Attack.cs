using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Attack: MonoBehaviour
{
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerAttack;

    [SerializeField] private GameObject sprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [SerializeField] private Player_Movement playerMove;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    private float attackRange;
    private IEnumerator sa1;
    private IEnumerator sa2;
    private IEnumerator sa3;
    [SerializeField] private float swingTime;
    private bool canAttackAgain;
    private bool inRange;
    [SerializeField] private GameObject enemy;
    public bool pKnockBack { get; private set; }
    [SerializeField] private float pKnockBackPower;
    [SerializeField] private float pKnockBackCount;
    private float pKnockBackTimer;
    private bool attack1;
    private bool attack2;
    private bool attack3;
    private float attackBuffTime;

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
        attack1 = true;
        pKnockBackTimer = pKnockBackCount;

        pKnockBack = false;
    }
    private void Update()
    {
        sa1 = SwingAnim1();
        sa2 = SwingAnim2();
        sa3 = SwingAnim3();

        if (attack2 || attack3)
        {
            attackBuffTime -= Time.deltaTime;
            if (attackBuffTime <= 0)
            {
                attackBuffTime = 0;
                attack1 = true;
            }
        }

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

        UpdateAttackAnimation();
    }

    private void FixedUpdate()
    {
        playerAttack.performed += Attack;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 5, ~ignoreCol);
        if (hit)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                attackRange = hit.point.x - transform.position.x;
                //Debug.Log(attackRange);
            }
        }
        else
        {
            return;
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
                eDamage++;
            }
        }
    }

    private IEnumerator SwingAnim1()
    {
        attack2 = false;
        attack3 = false;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttackAgain);
        anim.SetTrigger("Attack1");
        attack1 = false;
        canAttackAgain = false;
        yield return new WaitForSeconds(swingTime);
        attack2 = true;
        canAttackAgain = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack1");
    }

    private IEnumerator SwingAnim2()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.SetTrigger("Attack2");
        attack2 = false;
        yield return new WaitForSeconds(swingTime);
        attack3 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack2");
    }

    private IEnumerator SwingAnim3()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.SetTrigger("Attack3");
        attack3 = false;
        yield return new WaitForSeconds(swingTime);
        attack1 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack3");
    }

    private void UpdateAttackAnimation()
    {
        if (playerAttack.WasPressedThisFrame())
        {
            if (attack1)
            {
               StartCoroutine(sa1);
               attackBuffTime = .4f;
            }

            if (attack2 && attackBuffTime > 0)
            {
               StartCoroutine(sa2);
               attackBuffTime = .4f;
            }
            else
            {
                StopCoroutine(sa2);
            }

            if (attack3 && attackBuffTime > 0)
            {
               StartCoroutine(sa3); 
            }
            else
            {
                StopCoroutine(sa3);
            }

        }
        else
        {
            StopCoroutine(sa1);
            canAttackAgain = true;
        }

    }

}
