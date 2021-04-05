using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    LineRenderer lineRenderer;
    public static Aim instance;
    [SerializeField] Transform follow;
    [SerializeField] Gradient color;
    [SerializeField] float speed = 5;
    [Range(0, 30)]
    [SerializeField] float ray;
    private void Awake()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.colorGradient = color;
    }

    public void DrawLine(Vector2 start, Vector2 finish)
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, finish);
    }
    public void DrawLine(Vector2 direction)
    {     
        DrawLine(transform.position, transform.position + (Vector3) direction.normalized * ray);
    }

    public void StopDrawing()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        Follow(follow);
    }

    private void Follow(Transform follow)
    {
        transform.position = Vector3.MoveTowards(transform.position, follow.position, speed * Time.deltaTime);
    }

}
