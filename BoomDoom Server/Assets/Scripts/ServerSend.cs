using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class ServerSend
{
    #region Send Data
    private static void SendData(int toClient, Packet packet)
    {
        Server.clients[toClient].SendData(packet.ToArray());
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
    public static void Welcome(int toClient)
    {
        Packet packet = new Packet((int)ServerPackets.Welcome);
        packet.Write(toClient);
        SendData(toClient, packet);
    }

    /// <summary>
    /// Sends a packet to someone indicating that they should spawn this player
    /// </summary>
    /// <param name="clientToSpawn"></param>
    public static void SpawnPlayer(int clientToSpawn, int toClient)
    {
        Packet packet = new Packet((int)ServerPackets.SpawnPlayer);
        packet.Write(clientToSpawn);
        SendData(toClient, packet);
    }
}