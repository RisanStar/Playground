using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool inRange { get; private set; }
    public float attackDistance { get; private set; }
    private Vector2 heightCorrect;

    public LayerMask ignoreCol;


    private void Start()
    {
       heightCorrect = new Vector2(transform.position.x, transform.position.y - 1.5f);
    }

    private void Update()
    {
        Debug.DrawRay(heightCorrect, Vector2.right * 10, Color.green);
        if (attackDistance >= 1.2f)
        {
            inRange = false;
        }
        else
        {
            inRange = true;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(heightCorrect, Vector2.right, 5, ~ignoreCol);
        if (hit.collider != null)
        {
            hit.transform.SendMessage("HitByRay");
            attackDistance = hit.point.x - transform.position.x;
            Debug.Log(attackDistance);
        }
    }
}
