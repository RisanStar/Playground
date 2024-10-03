using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Death : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D cCollider;
    [SerializeField] private Animator anim;

    [SerializeField] private LayerMask ignoreCol;
    private Vector2 bluePos;

    [Header("Health")]
    [SerializeField] private float hp;
    private bool beingHit;
    private IEnumerator bh;
    private IEnumerator ha;
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
        bh = GettingHit();
        ha = HitAnim();

        if (transform.position.y < -5)
        {
            SceneManager.LoadScene("nothing");
        }

        if (beingHit)
        {
            StartCoroutine(bh);
        }
        
        if (!beingHit && !hasIFrames)
        {
            StopCoroutine(bh);
        }

        if (hitCount >= hp)
        {
            deathCount = 1;
        }

        //ANIMATION
        UpdateHealthAnimation();

        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"));

        bluePos = new Vector2(transform.position.x, transform.position.y - .1f);
        Debug.DrawRay(bluePos, Vector2.right * .5f, Color.blue);

    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, .5f, ~ignoreCol);
        if (hit)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                beingHit = true;
            }
        }
        else
        {
            beingHit = false;
        }
    }

    private IEnumerator DeathRestart()
    {
        isDead = true;
        hitCD = 0;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.SetBool("noBlood", false);
        anim.SetTrigger("Death");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Death");

        yield return new WaitForSeconds(dRTime);
        SceneManager.LoadScene("nothing");
    }

    private IEnumerator GettingHit()
    {
        yield return new WaitUntil(() => !hasIFrames);
        hitCoolDown -= 1 * Time.deltaTime;

        if (hitCoolDown < 0) { hitCoolDown = 0; }

        if (hitCoolDown <= 0)
        {
            hitCoolDown = hitCD;
            hasIFrames = true;
            hitCount++;
            yield return new WaitForSeconds(iFrames);
            hasIFrames = false;
        }
    }

    private IEnumerator HitAnim()
    {
        anim.SetTrigger("Hurt");
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        anim.ResetTrigger("Hurt");
    }

    private void UpdateHealthAnimation()
    {
        if (!isDead)
        {
            if (beingHit && !hasIFrames)
            {
                StartCoroutine(ha);
            }
            else
            {
                StopCoroutine(ha);
            }
        }

        if (deathCount == 1)
        {
            StartCoroutine(DeathRestart());
        }
        else
        {
            isDead = false;
        }
    }
}


