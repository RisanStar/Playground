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
    [SerializeField] private float swingTime;
    private float swingTimeCount;
    public bool isAttacking { get; private set; }
    private bool eAttack1;
    private bool eAttack2;

    private void Start()
    {
        eAttack1 = true;
        swingTimeCount = swingTime;
    }


    private void Update()
    {

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

    private void UpdateAttackAnimation()
    {
        if (!eneDeath.eIsDead)
        {
            if (eAttack1 && anim.GetNextAnimatorStateInfo(0).normalizedTime <= 0)
            {
                anim.SetTrigger("Attack1");
                eAttack1 = false;
                if (anim.GetNextAnimatorStateInfo(0).IsName("Attack1"))
                {
                    anim.ResetTrigger("Attack1");
                    eAttack2 = true;
                }
            }

            if (eAttack2 && anim.GetNextAnimatorStateInfo(0).normalizedTime <= 0)
            {
                anim.SetTrigger("Attack2");
                eAttack2 = false;
                if (anim.GetNextAnimatorStateInfo(0).IsName("Attack2"))
                {
                    anim.ResetTrigger("Attack2");
                    eAttack1 = true;
                }
            }

        }


    }

}
