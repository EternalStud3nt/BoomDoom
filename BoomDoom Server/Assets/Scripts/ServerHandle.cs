using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class ServerHandle
{
    public static void HelloReceived(Packet packet)
    {
        Debug.Log(packet.ReadString());
    }

    public static void MyPosition(Packet packet)
    {
        Debug.Log(packet.ReadVector2());
    }
}