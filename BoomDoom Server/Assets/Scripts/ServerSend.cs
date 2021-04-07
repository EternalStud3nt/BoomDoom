using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class ServerSend
{
    #region Send Data
    private static void SendData(int clientID, Packet packet)
    {
        Server.clients[clientID].SendData(packet.ToArray());
    }

    private static void SendDataToAll(Packet packet)
    {
        foreach(Client client in Server.clients.Values)
        {
            client.SendData(packet.ToArray());
        }
    }

    private static void SendDataToAll(int exceptClientID, Packet packet)
    {
        for(int i = 0; i <Server.clients.Count; i++)
        {
            if(i != exceptClientID)
            {
                Server.clients[i].SendData(packet.ToArray());
            }
        }
    }
    #endregion
    public static void HelloReceived()
    {
        Packet packet = new Packet((int)ServerPackets.HelloReceived);
        packet.Write("I received your hello, welcome");
        SendDataToAll(packet);
    }

    public static void ClientPosition(int clientID, Vector2 position)
    {
        Packet packet = new Packet((int)ServerPackets.PlayerPosition);
        packet.Write($"The position of client: {clientID} is {position}.");
        SendDataToAll(packet);
    }
}