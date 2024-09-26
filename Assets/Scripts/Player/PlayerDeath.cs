using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D cCollider;
    [SerializeField] private Animator anim;

    [Header("Health")]
    [SerializeField] private float hp;
    private bool beingHit;
    [SerializeField] private float hitCD;
    private float hitCoolDown;
    private float hitCount;
    private float deathCount; 
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

    }

    private void FixedUpdate()
    {
        //ANIMATION
        UpdateHealthAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            beingHit = true;
        }
        else
        {
            beingHit = false;
        }
    }

    private void UpdateHealthAnimation()
    {
        if (beingHit)
        {
            Debug.Log("colliding");
            if (hitCoolDown == hitCD)
            {
                anim.SetTrigger("Hurt");
                hitCount++;
            }

            hitCoolDown -= 1 * Time.deltaTime;
            if (hitCoolDown <= 0) { hitCoolDown = 0; }
            if (hitCoolDown == 0)
            {
                hitCoolDown = hitCD;
            }


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
;           }
        }
        else
        {
            isDead = false;
        }
    }

}
