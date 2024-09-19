using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private bool knockBack;
    [SerializeField] private float knockbackPower;
    [SerializeField] private float knockbackCount;
    private float knockbackTimer;
    private void HitByRay()
    {

    }
}
