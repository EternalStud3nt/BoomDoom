using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientSend 
{
    public static void SendPacket(Packet packet)
    {
        packet.InsertDataSize();
        Client.Instance.SendData(packet.ToArray());
    }

    public static void WelcomeReceived()
    {
        Packet packet = new Packet((int)ClientPackets.WelcomeReceived);
        SendPacket(packet);
    }

    public static void MyPosition(Vector2 position)
    {
        Packet packet = new Packet((int)ClientPackets.MyPosition);
        packet.Write(position);
        SendPacket(packet);
    }
}
