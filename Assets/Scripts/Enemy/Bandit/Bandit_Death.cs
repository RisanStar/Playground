using System.Collections;
using UnityEngine;

public class Bandit_Death : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private Collider2D playerHB;

    [Header("Scripts")]
    [SerializeField] private Player_RealAttack playerRAttack;

    [Header("Health")]
    [SerializeField] private float eHp;
    public bool eIsDead { get; private set; }
    private bool deathIsDone;

    private void Start()
    {
        deathIsDone = false;
    }

    private void Update()
    {
        //Debug.Log(playerAttack.eDamage);

        if (playerRAttack.eDamage)
        {
            eHp--;
            anim.SetTrigger("Hurt");
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                anim.ResetTrigger("Hurt");
            }
        }
    
        if (eHp <= 0)
        {
            eIsDead = true;
            Physics2D.IgnoreCollision(playerHB, hitbox, true);
        }

        //Debug.Log("The death anim is finsihed:  " + deathIsDone);
        UpdateHealthAnimation();
    }

    private void UpdateHealthAnimation()
    {
        if (eIsDead) 
        {
            if (!deathIsDone)
            {
                anim.SetTrigger("Death");
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    deathIsDone = true;
                }
            }
        }
    }
}