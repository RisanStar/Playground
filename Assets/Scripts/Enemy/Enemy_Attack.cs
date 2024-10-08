using System.Collections;
using UnityEngine;


public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    [SerializeField] private Enemy_Movement eneMove;
    [SerializeField] private Enemy_Death eneDeath;
    [SerializeField] private Player_Death playerDeath;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    private float attackRange;
    private IEnumerator a1;
    private IEnumerator a2;
    [SerializeField] private float swingTime;
    private bool canAttack;
    private bool inRange;
    private bool eAttack1;
    private bool eAttack2;

    void Start()
    {
        eAttack1 = true;
    }

    void Update()
    {
        a1 = Attack1();
        a2 = Attack2();

        //RANGE
        if (attackRange <= 2)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        UpdateAttackAnimation();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (hit)
        {
            if (hit.collider.CompareTag("Player"))
            {
                attackRange = hit.point.x - transform.position.x;
            }
        }
        else
        {
            attackRange = hit.point.x - transform.position.x;
        }
    }

    private IEnumerator Attack1()
    {
        eAttack2 = false;
        canAttack = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttack);
        anim.SetTrigger("Attack1");
        eAttack1 = false;
        canAttack = false;
        yield return new WaitForSeconds(swingTime);
        eAttack2 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack1");
    }

    private IEnumerator Attack2()
    {
        canAttack = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && canAttack);
        anim.SetTrigger("Attack2");
        eAttack2 = false;
        canAttack = false;
        yield return new WaitForSeconds(swingTime);
        eAttack1 = true;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack2");
    }
    private void UpdateAttackAnimation()
    {
        if (!eneDeath.eIsDead)
        {
            if (inRange && !eneMove.moving)
            {
                if (eAttack1)
                {
                    StartCoroutine(a1);
                }
                else
                {
                    StopCoroutine(a1);
                }

                if (eAttack2)
                {
                    StartCoroutine(a2);
                }
                else
                {
                    StopCoroutine(a2);
                }
            }
        }

    }

}