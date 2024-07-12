using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool inRange { get; private set; }
    public float attackDistance { get; private set; }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            inRange = true;
            attackDistance = Vector2.Distance(transform.position, collision.transform.position);
            Debug.Log("Is in range and is within: " + attackDistance);
        }
    }
}
