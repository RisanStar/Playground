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
    private IEnumerator a;
    [SerializeField] private float swingTime;
    public bool isAttacking { get; private set; }
    private bool inRange;
    private bool canAttack;


    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 5, ~ignoreCol);
        if (hit)
        {
            if (hit.collider.CompareTag("Player"))
            {
;
            }
        }
        else
        {

        }
    }

    public virtual void OnTargetReached()
    {
        if (aIPath.reachedEndOfPath)
        {
            UpdateAttackAnimation();
        }
        else
        { 

        }
    }

    private void UpdateAttackAnimation()
    {
        if (!eneDeath.eIsDead)
        {
            anim.SetTrigger("Attack1");
 
        }

    }

}
