using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Singleton<LocalPlayer>
{
    private void FixedUpdate()
    {
        ClientSend.MyPosition(transform.position);
    }
}
