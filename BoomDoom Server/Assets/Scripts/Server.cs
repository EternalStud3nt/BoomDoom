using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public static class Server
{
    public static int Port { get; private set; }
    private static TcpListener listener;
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

    public static void Start(int port)
    {
        Debug.Log("Starting server...");
        Port = port;
        listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        listener.BeginAcceptTcpClient(AcceptTCPClientCallback, null);
    }

    private static void AcceptTCPClientCallback(IAsyncResult ar)
    {
        Debug.Log("NEW CLIENT");
        TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
        Client client = new Client();
        clients.Add(clients.Count, client);
        client.Connect(tcpClient);
        listener.BeginAcceptTcpClient(AcceptTCPClientCallback, null);

    }
}
