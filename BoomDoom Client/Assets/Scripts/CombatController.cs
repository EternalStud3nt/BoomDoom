using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    private void Awake()
    {
        AttackJoystick.FireValue += Shoot;
    }

    private void Shoot(Vector2 direction)
    {
        Instantiate(bullet, transform.position + (Vector3)direction, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90));
    }
}
