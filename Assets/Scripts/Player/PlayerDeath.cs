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
    private float hitCount;

    private void Start()
    {
        hitCount = hitCD;
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
            if (hitCount == hitCD)
            {
                anim.SetTrigger("Hurt");
            }

            hitCount -= 1 * Time.deltaTime;
            if (hitCount <= 0) { hitCount = 0; }
            if (hitCount == 0)
            {
                hitCount = hitCD;
            }
        }
        else
        {
            return;
        }
    }

}
