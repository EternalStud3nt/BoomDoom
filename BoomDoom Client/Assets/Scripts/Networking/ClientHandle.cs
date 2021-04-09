using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientHandle
{
    public static void Welcome(Packet packet)
    {
        int clientID = packet.ReadInt();
        Debug.Log("Received ID: " + clientID);
        Client.Instance.clientID = clientID;
        GameManager.Instance.SpawnLocalPlayer(clientID);
        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet packet)
    {
        int newClientID = packet.ReadInt();
        GameManager.Instance.SpawnOnlinePlayer(newClientID);
    }
}
