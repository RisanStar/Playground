using Pathfinding;
using UnityEngine;


public class Wizard_Attack : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    [Header("Scripts")]
    [SerializeField] private AIPath aIPath;
    [SerializeField] private Wizard_Movement wizMove;
    [SerializeField] private Wizard_Death wizDeath;
    [SerializeField] private Player_Death playerDeath;

    [Header("Layers")]
    [SerializeField] private LayerMask ignoreCol;

    [Header("Attacking")]
    [SerializeField] private float swingTime;
    private float swingTimeCount;
    public bool isAttacking { get; private set; }

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
        anim.SetTrigger("Attack1");
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
        {
            anim.ResetTrigger("Attack1");
        } 
    }

}
