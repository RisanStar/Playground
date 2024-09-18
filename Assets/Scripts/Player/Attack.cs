using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool inRange { get; private set; }
    public float attackDistance { get; private set; }
    private Vector2 heightCorrect;


    private void Start()
    {
      heightCorrect = new Vector2(transform.position.x, transform.position.y - 1);
    }

    private void Update()
    {
        Debug.DrawRay(heightCorrect, Vector2.right * 10, Color.green);
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(heightCorrect, Vector2.right);
        if (hit.collider != null)
        {
            attackDistance = hit.point.x - transform.position.x;
            Debug.Log(attackDistance);
        }
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            inRange = true;
            attackDistance = Vector2.Distance(transform.position, collision.transform.position);
            Debug.Log("Is in range and is within: " + attackDistance);
        }
        else
        {
            inRange = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            inRange = true;
            attackDistance = Vector2.Distance(transform.position, collision.transform.position);
            Debug.Log("Is in range and is within: " + attackDistance);
        }
        else
        {
            inRange = false;
        }
    }
    */
}
