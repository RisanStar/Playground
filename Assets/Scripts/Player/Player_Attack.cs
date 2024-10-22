using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Attack: MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private PlayerControls playerCntrls;
    [SerializeField] private InputAction playerAttack;

    [Header("Assets")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Scripts")]
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Bandit_Death banDeath;
    [SerializeField] private Player_Death playerDeath;

    [Header("Layers")]
    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    [SerializeField] private float swingTime;
    private IEnumerator sa1;
    private IEnumerator sa2;
    private IEnumerator sa3;
    private bool canAttack;
    private bool canAttackAgain;

    private bool pAttack1;
    private bool pAttack2;
    private bool pAttack3;
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
        pAttack1 = true;
    }
    private void Update()
    {
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

        //Debug.DrawRay(transform.position, Vector2.right * 5, Color.green);
        //Debug.Log("Player is in range: " + inRange);
        //Debug.Log(inRange);

        UpdateAttackAnimation();
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
