using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class Enemy_Death : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private Player_Attack playerAttack;

    [Header("Health")]
    private bool eIsDead;
    [SerializeField] private float eHp;

    void Start()
    {

    }

    void Update()
    {
        if(playerAttack.eDamage)
        {
            eHp--;
        }
    
        if (eHp <= 0)
        {
            eIsDead = true;
        }

        UpdateHealthAnimation();
    }

    private IEnumerator Death()
    {
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.SetTrigger("Death");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Death");
    }

    private void UpdateHealthAnimation()
    {
        if (eIsDead)
        {
            StartCoroutine(Death());
        }
    }
}
