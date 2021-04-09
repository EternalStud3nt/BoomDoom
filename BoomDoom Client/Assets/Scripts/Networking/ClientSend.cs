using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientSend 
{
    public static void SendPacket(Packet packet)
    {
        Client.Instance.SendData(packet.ToArray());
    }

    public static void WelcomeReceived()
    {
        Packet packet = new Packet((int)ClientPackets.WelcomeReceived);
        packet.Write(Client.Instance.clientID);
        SendPacket(packet);
    }
}
