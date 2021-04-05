using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float travelSpeed;
    [SerializeField] LayerMask whatIsEnemy;
    Vector3 lastPosition;
    private void OnEnable()
    {
        Invoke("Deactivate", 2);
    }
    private void FixedUpdate()
    {
        lastPosition = transform.position;
        transform.position += Time.deltaTime * transform.up * travelSpeed;
        if (CheckCollision())
        {
            //todo: apply damage to target
        }
        Debug.DrawLine(lastPosition, transform.position, Color.red) ;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private bool CheckCollision()
    {
        return Physics2D.Raycast(lastPosition, (transform.position - lastPosition).normalized, (transform.position - lastPosition).magnitude, whatIsEnemy);
    }

}
