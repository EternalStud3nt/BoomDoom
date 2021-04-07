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
    private readonly int dataBufferSize = 4096;
    private byte[] buffer;

    public static Dictionary<int, Action<Packet>> packetActions = new Dictionary<int, Action<Packet>>
    {
        { (int)ServerPackets.HelloReceived, ClientHandle.HelloReceived },
        { (int)ServerPackets.PlayerPosition, ClientHandle.PlayerPosition }
    };

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
        buffer = new byte[dataBufferSize];
        stream = socket.GetStream();
        ClientSend.Hello();
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        stream.EndRead(ar);
        Packet packet = new Packet(buffer);
        packetActions[packet.ReadInt()](packet);
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    public void SendData(byte[] data)
    {
        Debug.Log("Trying to send data...");
        stream.Write(data, 0, data.Length);
    }
}
