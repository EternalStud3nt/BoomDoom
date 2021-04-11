using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientHandle
{
    public static void Welcome(Packet packet)
    {
        int clientID = packet.ReadInt();
        Client.Instance.clientID = clientID;
        GameManager.Instance.SpawnLocalPlayer(clientID);
        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet packet)
    {
        int newClientID = packet.ReadInt();
        GameManager.Instance.SpawnOnlinePlayer(newClientID);
    }

    public static void SetPosition(Packet packet)
    {
        int clientID = packet.ReadInt();
        Vector2 position = packet.ReadVector2();
        GameManager.Instance.players.TryGetValue(clientID, out GameObject player);
        if(player)
        {
            player.GetComponent<NetworkPlayer>().SetPosition(position);
        }
    }
}
