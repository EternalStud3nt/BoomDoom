using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : Singleton<Client>
{
    [SerializeField] string serverIP;
    [SerializeField] int serverPort = 23000;
    
    private TcpClient socket;
    private NetworkStream stream;

    private void Start()
    {
        Connect(serverIP, serverPort);
    }

    private void Connect(string ip, int port)
    {
        socket = new TcpClient();
        if (ip == "localhost")
            socket.BeginConnect(IPAddress.Parse("127.0.0.1"), port, OnConnectedToServerCallback, null); 
        else
            socket.BeginConnect(IPAddress.Parse(ip), port, OnConnectedToServerCallback, null);
    }

    private void OnConnectedToServerCallback(IAsyncResult ar)
    {
        socket.EndConnect(ar);
        stream = socket.GetStream();
        Test();
    }

    public void SendData(byte[] data)
    {
        Debug.Log("Trying to send data...");
        stream.Write(data, 0, data.Length);
    }

    public void Test()
    {
        Packet packet = new Packet();
        packet.Write(69);
        ClientSend.SendPacket(packet);
    }
}
