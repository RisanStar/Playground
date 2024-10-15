using Pathfinding;
using System.Collections;
using UnityEngine;


public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    [SerializeField] private AIPath aIPath;
    [SerializeField] private Enemy_Movement eneMove;
    [SerializeField] private Enemy_Death eneDeath;
    [SerializeField] private Player_Death playerDeath;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    private float attackRange;
    private IEnumerator a1;
    private IEnumerator a2;
    [SerializeField] private float swingTime;
    public bool isAttacking { get; private set; }
    private bool inRange;
    private bool canAttack;
    private bool canAttackAgain;
    private bool eAttack1;
    private bool eAttack2;


    private void Update()
    {
        a1 = Attack1();

        Debug.DrawRay(transform.position, Vector3.right * 2, Color.black);

        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, 2f, ~ignoreCol);
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, Vector2.right, 2f, ~ignoreCol);
        if (lefthit)
        {
            if (lefthit.collider.CompareTag("Player"))
            {
                UpdateAttackAnimation();
            }
        }
        else if (righthit)
        {
            if (righthit.collider.CompareTag("Player"))
            {
                UpdateAttackAnimation();
            }
        }
        else
        {
            return;
        }
    }

    private IEnumerator Attack1()
    {
        yield return new WaitUntil(() => anim.GetNextAnimatorStateInfo(0).normalizedTime == 0f);
        anim.SetTrigger("Attack1");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Attack1");
    }

    private void UpdateAttackAnimation()
    {
        if (!eneDeath.eIsDead)
        {
            StartCoroutine(a1);
        }

    }

}
