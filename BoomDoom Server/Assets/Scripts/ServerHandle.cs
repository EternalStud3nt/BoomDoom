﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public static class ServerHandle
{
    public static void WelcomeReceived(int fromClientID, Packet packet)
    {
        foreach(Client client in Server.clients.Values)
        {
            if(client.id != fromClientID)
            {
                // Spawn the client that just connected to all other client instances
                ServerSend.SpawnPlayer(fromClientID, client.id);
                // Spawn existing clients to the just-connected client
                ServerSend.SpawnPlayer(client.id, fromClientID);
            }  
        }
    }
}