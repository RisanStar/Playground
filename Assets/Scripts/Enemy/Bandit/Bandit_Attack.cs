using Pathfinding;
using UnityEngine;


public class Bandit_Attack : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    [Header("Scripts")]
    [SerializeField] private AIPath aIPath;
    [SerializeField] private Bandit_Movement banMove;
    [SerializeField] private Bandit_Death banDeath;
    [SerializeField] private Player_Death playerDeath;

    [Header("Layers")]
    [SerializeField] private LayerMask ignoreCol;
    [SerializeField] private LayerMask player;

    [Header("Attacking")]
    [SerializeField] private float swingTime;
    [SerializeField] private Transform playerCheck;
    public bool isAttacking { get; private set; }
    public bool inCombat { get; private set; }

    private void Update()
    {
        if (InRange())
        {
            UpdateAttackAnimation();
            inCombat = true;
        }
        else
        {
            inCombat = false;
        }
    }

    private bool InRange()
    {
       return Physics2D.OverlapCircle(playerCheck.position, 2f, player);
    }

    private void UpdateAttackAnimation()
    {
        {
            anim.SetTrigger("Attack");
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                anim.ResetTrigger("Attack");
            }
        }
    }

}
