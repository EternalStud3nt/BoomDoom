using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : Singleton<Client>
{   
    private TcpClient socket;
    private NetworkStream stream;
    private readonly int dataBufferSize = 4096;
    private byte[] buffer;
    public Action OnConnection;
    public int clientID;

    public static Dictionary<int, Action<Packet>> packetActions = new Dictionary<int, Action<Packet>>
    {
        { (int)ServerPackets.Welcome, ClientHandle.Welcome },
        { (int)ServerPackets.SpawnPlayer, ClientHandle.SpawnPlayer }
    };

    public void Connect(string ip, int port)
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
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
        ThreadManager.ExecuteOnMainThread(() => { OnConnection?.Invoke(); });      
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        stream.EndRead(ar);
        ThreadManager.ExecuteOnMainThread(() => HandleData(buffer));
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        packet.SetBytes();
        packetActions[packet.ReadInt()](packet);
    }

    public void SendData(byte[] data)
    {
        stream.Write(data, 0, data.Length);
    }
}
