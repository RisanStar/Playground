using System.Collections;
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
    [SerializeField] private Enemy_Death eneDeath;
    [SerializeField] private Player_Death playerDeath;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    private float lAttackRange;
    private float rAttackRange;
    private IEnumerator sa1;
    private IEnumerator sa2;
    private IEnumerator sa3;
    [SerializeField] private float swingTime;
    private bool canAttack;
    private bool canAttackAgain;
    private bool inRange;
    [SerializeField] private GameObject enemy;
    public bool pKnockBack { get; private set; }
    [SerializeField] private float pKnockBackPower;
    [SerializeField] private float pKnockBackCount;
    private float pKnockBackTimer;
    private bool pAttack1;
    private bool pAttack2;
    private bool pAttack3;
    private float attackBuffTime;

    public bool eDamage {  get; private set; }

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
        pAttack1 = true;
        pKnockBackTimer = pKnockBackCount;

        pKnockBack = false;
    }
    private void Update()
    {
        //RANGE
        if (lAttackRange <= 1.2f || rAttackRange <= 1.2f)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        if (canAttack)
        {
            if (inRange)
            {
                pKnockBack = true;
                eDamage = true;
            }
            else
            {
                eDamage = false;
            }
        }
        else
        {
            eDamage = false;
        }

        sa1 = SwingAnim1();
        sa2 = SwingAnim2();
        sa3 = SwingAnim3();

        if (pAttack2 || pAttack3)
        {
            attackBuffTime -= Time.deltaTime;
            if (attackBuffTime <= 0)
            {
                attackBuffTime = 0;
                pAttack1 = true;
            }
        }

        Debug.DrawRay(transform.position, Vector2.right * 5, Color.green);
        //Debug.Log("Player is in range: " + inRange);
        //Debug.Log(inRange);

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
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, Vector2.right, 5, ~ignoreCol);
        if (lefthit || righthit)
        {
            if (lefthit.collider.CompareTag("Enemy") || righthit.collider.CompareTag("Enemy"))
            {
                lAttackRange = lefthit.point.x - transform.position.x;
                rAttackRange = righthit.point.x - transform.position.x;
            }
        }
        else
        {
            lAttackRange = lefthit.point.x - transform.position.x;
            rAttackRange = righthit.point.x - transform.position.x;
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

    private IEnumerator SwingAnim1()
    {
        pAttack2 = false;
        pAttack3 = false;
        canAttack = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttack || canAttackAgain);
        anim.SetTrigger("Attack1");
        pAttack1 = false;
        canAttack = false;
        canAttackAgain = false;
        yield return new WaitForSeconds(swingTime);
        pAttack2 = true;
        canAttackAgain = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack1");
    }

    private IEnumerator SwingAnim2()
    {
        canAttack = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttack);
        anim.SetTrigger("Attack2");
        pAttack2 = false;
        canAttack = false;
        yield return new WaitForSeconds(swingTime);
        pAttack3 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack2");
    }

    private IEnumerator SwingAnim3()
    {
        canAttack = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttack);
        anim.SetTrigger("Attack3");
        pAttack3 = false;
        canAttack = false;
        yield return new WaitForSeconds(swingTime);
        pAttack1 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack3");
    }

    private void UpdateAttackAnimation()
    {
        if (playerAttack.WasPressedThisFrame() && !playerDeath.pIsDead)
        {
            if (pAttack1)
            {
               StartCoroutine(sa1);
               attackBuffTime = .4f;
            }

            if (pAttack2 && attackBuffTime > 0)
            {
               StartCoroutine(sa2);
               attackBuffTime = .4f;
            }
            else
            {
                StopCoroutine(sa2);
            }

            if (pAttack3 && attackBuffTime > 0)
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
