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
    private bool beingHit;
    private bool hasIFrames;
    [SerializeField] private float iFrames;
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
        else
        {
            if (beingHit)
            {
                StartCoroutine(GettingHit());
            }
            else
            {
                StopCoroutine(GettingHit());
            }

            if (hitCount >= hp)
            {
                deathCount = 1;
            }
        }

        
           
        

        //ANIMATION
        UpdateHealthAnimation();

        Debug.Log("The hit CD is " + hitCoolDown);
        Debug.Log("The I-frames are " + iFrames);
        //Debug.DrawRay(transform.position, Vector2.right * .7f, Color.blue);

    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1f, ~ignoreCol);
        if (hit.collider.CompareTag("Enemy"))
        {
            beingHit = true;
        }
        else
        {
            beingHit = false;
        }
    }

    private IEnumerator DeathRestart()
    {
        yield return new WaitForSeconds(dRTime);
        SceneManager.LoadScene("nothing");
    }

    private IEnumerator GettingHit()
    {
        yield return new WaitUntil(() => !hasIFrames);
        hitCoolDown -= 1 * Time.deltaTime;

        yield return new WaitUntil(() => hitCoolDown <= 0);
        hitCoolDown = hitCD;
        hasIFrames = true;

        if (hasIFrames)
        {
            hitCount++;
            yield return new WaitForSeconds(iFrames);
            hasIFrames = false;
        }

    }

    private IEnumerator HitAnim()
    {
        anim.SetTrigger("Hurt");
        yield return new WaitForSeconds(iFrames);
    }

    private void UpdateHealthAnimation()
    {

        if (beingHit)
        {
            StartCoroutine(HitAnim());
        }
        else
        {
            StopCoroutine(HitAnim());
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


