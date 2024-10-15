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
    private float attackCount = 0;

    private void Start()
    {
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
            if (attackCount <= 0 && anim.GetNextAnimatorStateInfo(0).normalizedTime <= 0)
            {
                anim.SetTrigger("Attack1");
                attackCount++;
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
                {
                    anim.ResetTrigger("Attack1");
                }
            }
            else if (attackCount > 0 && attackCount < 2 && anim.GetNextAnimatorStateInfo(0).normalizedTime <= 0)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
                {
                    anim.SetTrigger("Attack2");
                    attackCount++;
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
                    {
                        anim.ResetTrigger("Attack2");
                    }
                }
            }
            
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                swingTimeCount -= Time.deltaTime;
                if (swingTimeCount <= 0) { swingTimeCount = 0; }
                if (swingTimeCount == 0)
                {
                    attackCount = 0;
                }
            }
        }


    }

}
