using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Death : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D cCollider;
    [SerializeField] private Animator anim;

    [SerializeField] private IgnorePlayerCollision iPC;

    [SerializeField] private LayerMask ignoreCol;
    private Vector2 bluePos;

    [Header("Health")]
    [SerializeField] private float pHp;
    private bool beingHit;
    private IEnumerator gh;
    private IEnumerator ha;
    private bool hasIFrames;
    [SerializeField] private float iFrames;
    [SerializeField] private float hitCD;
    private float hitCoolDown;
    private float hitCount;
    private float deathCount;
    [SerializeField] private float dRTime;
    public bool pIsDead { get; private set; }

    private void Awake()
    {
        pIsDead = false;
        deathCount = 0;
    }

    private void Start()
    {
        hitCoolDown = hitCD;
    }

    private void Update()
    {
        gh = GettingHit();
        ha = HitAnim();

        if (transform.position.y < -5)
        {
            SceneManager.LoadScene("nothing");
        }

        if (beingHit)
        {
            StartCoroutine(gh);
        }
        
        if (!beingHit && !hasIFrames)
        {
            StopCoroutine(gh);
        }

        if (hitCount >= pHp)
        {
            deathCount = 1;
        }

        //ANIMATION
        UpdateHealthAnimation();


        bluePos = new Vector2(transform.position.x, transform.position.y - .1f);
        Debug.DrawRay(bluePos, Vector2.right * .5f, Color.blue);

    }

    private void FixedUpdate()
    {
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, .5f, ~ignoreCol);
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, Vector2.right, .5f, ~ignoreCol);
        if (iPC.pIFramesCount <= 0)
        {
            if (lefthit)
            {
                if (lefthit.collider.CompareTag("Enemy"))
                {
                    beingHit = true;
                }

                else if (righthit)
                {
                    if (righthit.collider.CompareTag("Enemy"))
                    {
                        beingHit = true;
                    }
                }
                else
                {
                    beingHit = false;
                }
            }

        }
    }

    private IEnumerator DeathRestart()
    {
        pIsDead = true;
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
        if (!pIsDead)
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
            pIsDead = false;
        }
    }
}


