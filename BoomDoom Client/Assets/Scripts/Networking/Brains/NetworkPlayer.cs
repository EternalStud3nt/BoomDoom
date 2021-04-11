using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
}
