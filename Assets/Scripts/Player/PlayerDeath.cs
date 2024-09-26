using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D cCollider;
    [SerializeField] private Animator anim;

    [SerializeField] private LayerMask ignoreCol;

    [Header("Health")]
    [SerializeField] private float hp;
    private float hitRange;
    private bool beingHit;
    [SerializeField] private float hitCD;
    private float hitCoolDown;
    private float hitCount;
    private float deathCount;
    [SerializeField] private float dRTime;
    public bool isDead { get; private set; }

    private void Awake()
    {
        isDead = false;
        deathCount = 0;
    }

    private void Start()
    {
        hitCoolDown = hitCD;
    }

    private void Update()
    {
        if (transform.position.y < -5)
        {
            SceneManager.LoadScene("nothing");
        }

        if (isDead)
        {
            StartCoroutine(DeathRestart());
        }
    }

    private void FixedUpdate()
    {
        //ANIMATION
        UpdateHealthAnimation();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, .7f, ~ignoreCol);
        if (hit.collider.CompareTag("Enemy"))
        {
            beingHit = true;
        }
        else
        {
            beingHit = false;
        }
    }
    private IEnumerator HitCheck()
    {
        if (beingHit)
        {
            yield return new WaitForSeconds(hitCD);
            hitCount++;
            anim.SetTrigger("Hurt");
        }
        else
        {
            StopCoroutine(HitCheck());
        }
    }

    private IEnumerator DeathRestart()
    {
        yield return new WaitForSeconds(dRTime);
        SceneManager.LoadScene("nothing");
    }

    private void UpdateHealthAnimation()
    {
        if (!isDead)
        {
   
                StartCoroutine(HitCheck());

                
                if (hitCount >= 3) { hitCount = 3; }
                if (hitCount == 3)
                {
                    deathCount = 1;
                }


                if (deathCount == 1)
                {
                    anim.SetBool("noBlood", false);
                    anim.SetTrigger("Death");

                    hitCD = 0;
                    isDead = true;
      
                }
                else
                {
                    isDead = false;
                }
            }
        }
    }


