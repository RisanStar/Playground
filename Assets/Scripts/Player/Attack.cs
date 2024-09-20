using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool inRange { get; private set; }
    public float attackDistance { get; private set; }
    private Vector2 heightCorrect;

    public LayerMask ignoreCol;

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.right * 10, Color.green);
        if (attackDistance >= 1.2f)
        {
            inRange = false;
        }
        else
        {
            inRange = true;
            Debug.Log(inRange);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 5, ~ignoreCol);
        if (hit.collider.CompareTag("Enemy"))
        {
            attackDistance = hit.point.x - transform.position.x;
            //Debug.Log(attackDistance);
        }
    }
}
