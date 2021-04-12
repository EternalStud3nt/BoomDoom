using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class ServerSend
{
    #region Send Data
    private static void SendData(int toClient, Packet packet)
    {
        packet.InsertDataSize();
        Server.clients.TryGetValue(toClient, out Client client);
        if(client!=null)
            client.SendData(packet.ToArray());
    }

    private static void SendDataToAll(Packet packet)
    {
        packet.InsertDataSize();
        foreach (Client client in Server.clients.Values)
        {
            client.SendData(packet.ToArray());
        }
    }

    private static void SendDataToAll(int exceptClientID, Packet packet)
    {
        packet.InsertDataSize();
        foreach(Client client in Server.clients.Values)
        {
            int i = client.id;
            if (i != exceptClientID)
            {
                Server.clients[i].SendData(packet.ToArray());
            }
        }
    }
    #endregion
    public static void Welcome(int toClient)
    {
        Packet packet = new Packet((int)ServerPackets.Welcome);
        packet.Write(toClient);
        SendData(toClient, packet);
    }

    public static void SpawnPlayer(int clientToSpawn, int toClient)
    {
        Packet packet = new Packet((int)ServerPackets.SpawnPlayer);
        packet.Write(clientToSpawn);
        SendData(toClient, packet);
    }

    public static void SetPosition(int clientID, Vector2 position)
    {
        Packet packet = new Packet((int)ServerPackets.SetPosition);
        packet.Write(clientID);
        packet.Write(position);
        SendDataToAll(clientID, packet);
    }

    internal static void PlayerDisconnected(int id)
    {
        Packet packet = new Packet((int)ServerPackets.PlayerDisconnected);
        packet.Write(id);
        SendDataToAll(packet);
    }
}