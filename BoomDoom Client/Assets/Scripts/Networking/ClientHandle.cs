using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientHandle
{
    public static void HelloReceived(Packet packet)
    {
        string message = packet.ReadString();
        Debug.Log(message);
    }

    public static void PlayerPosition(Packet packet)
    {
        string message = packet.ReadString();
        Debug.Log(message);
    }
}
