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
        GameManager.Instance.SpawnNetworkPlayer(newClientID);
    }

    public static void SetPosition(Packet packet)
    {
        int clientID = packet.ReadInt();
        Vector2 position = packet.ReadVector2();
        GameManager.Instance.players.TryGetValue(clientID, out PlayerManager player);
        if(player)
        {
            player.SetPosition(position);
        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int clientID = packet.ReadInt();
        GameManager.Instance.DisconncectPlayer(clientID);
        Debug.Log($"Client {clientID} has left");
    }
}
