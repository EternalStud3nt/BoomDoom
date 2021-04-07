using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientSend 
{
    public static void SendData(byte[] data)
    {
        Client.Instance.SendData(data);
    }

    public static void Hello()
    {
        Packet packet = new Packet((int)ClientPackets.Hello);
        packet.Write("Hello server!");
        SendData(packet.ToArray());
    }

    public static void MyPosition(Vector2 position)
    {
        Packet packet = new Packet((int)ClientPackets.MyPosition);
        packet.Write(position);
        SendData(packet.ToArray());
    }
}
