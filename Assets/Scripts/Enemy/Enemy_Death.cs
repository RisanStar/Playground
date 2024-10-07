using System.Collections;
using UnityEngine;

public class Enemy_Death : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private Player_Attack playerAttack;

    [Header("Health")]
    [SerializeField] private float eHp;
    private IEnumerator d;
    public bool eIsDead { get; private set; }
    private bool deathIsDone;

    private void Start()
    {
        deathIsDone = false;
    }

    private void Update()
    {
        d = Death();

        Debug.Log(playerAttack.eDamage);

        if (playerAttack.eDamage)
        {
            eHp--;
        }
    
        if (eHp <= 0)
        {
            eIsDead = true;
            hitbox.enabled = false;
        }

        Debug.Log("The death anim is finsihed:  " + deathIsDone);
        UpdateHealthAnimation();
    }

    private IEnumerator Death()
    {
        anim.SetTrigger("Death");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Death");
        deathIsDone = true;
    }

    private void UpdateHealthAnimation()
    {
        if (eIsDead) 
        {
            if (!deathIsDone)
            {
                StartCoroutine(d);
            }
            else
            {
                StopCoroutine(d);
            }
        }
    }
}
