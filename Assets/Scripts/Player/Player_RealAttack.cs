using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RealAttack : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Collider2D attackHB;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Scripts")]
    [SerializeField] private Player_Movement playerMove;
    [SerializeField] private Player_Death playerDeath;

    [Header("Attacking")]
    [SerializeField] private Transform[] enemiesPos;
    [SerializeField] private float pKnockBackPower;
    [SerializeField] private float pKnockBackCount;
    private Transform enemyPos;
    public bool pKnockBack { get; private set; }
    private float pKnockBackTimer;

    public bool eDamage { get; private set; }

    private void Start()
    {
        pKnockBackTimer = pKnockBackCount;
        pKnockBack = false;
        attackHB = GetComponentInChildren<BoxCollider2D>();
    }

    private void Update()
    {

        if (pKnockBack)
        {
            pKnockBackTimer -= 1 * Time.deltaTime;
            if (pKnockBackTimer <= 0) { pKnockBackTimer = 0; }
            if (pKnockBackTimer == 0)
            {
                pKnockBack = false;
                pKnockBackTimer = pKnockBackCount;
            }
        }

        Type type = Type.GetType("IgnorePlayerCollision");
        var comp = GameObject.FindAnyObjectByType(type);
        int i = GameObject.FindObjectsOfType(type).Length;
        if (comp is Component component)
        {
            Transform[] eAmount = new Transform[i];
            enemiesPos = eAmount;
            for (int a = 0; a < eAmount.Length; a++)
            {
                enemiesPos[a] = component.transform;
            }

        }

        float minEne = Mathf.Infinity;
        foreach (Transform e in enemiesPos)
        {
            float ene = Vector2.Distance(e.position, transform.position);

            if (ene < minEne)
            {
                enemyPos = e;
                minEne = ene;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bandit_Movement>() != null)
        {
            Debug.Log(collision.GetComponent<Bandit_Movement>());
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
            {
                pKnockBack = true;
                eDamage = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (pKnockBack)
        {
            if (playerMove.moveDir.x < 0f)
            {
                rb.AddForce(Vector2.right * pKnockBackPower, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.left * pKnockBackPower, ForceMode2D.Impulse);
            }
        }
    }
}
