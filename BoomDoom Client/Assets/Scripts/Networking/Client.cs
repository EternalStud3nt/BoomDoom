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
    bool connected = false;

    public static Dictionary<int, Action<Packet>> packetActions = new Dictionary<int, Action<Packet>>
    {
        { (int)ServerPackets.Welcome, ClientHandle.Welcome },
        { (int)ServerPackets.SpawnPlayer, ClientHandle.SpawnPlayer },
        { (int)ServerPackets.SetPosition, ClientHandle.SetPosition },
        { (int)ServerPackets.PlayerDisconnected, ClientHandle.PlayerDisconnected }
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
        connected = true;
        buffer = new byte[dataBufferSize];
        stream = socket.GetStream();
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
        ThreadManager.ExecuteOnMainThread(() => { OnConnection?.Invoke(); });      
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        int byteCount = stream.EndRead(ar);
        byte[] data = new byte[byteCount];
        Array.Copy(buffer, data, byteCount);
        ThreadManager.ExecuteOnMainThread(() => HandleData(data));
        stream.BeginRead(buffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    // We are sure that each packet will arrive as a whole, but many packets might be combined into one big packet
    private void HandleData(byte[] data) 
    {
        Packet packet = new Packet(data);
        packet.SetBytes();
        int currentPacketData = packet.ReadInt();   // first packet to arrive size (except int that contains size info)
        int unreadData = packet.GetUndreadData();   // whole packet size (except int that contains first packet's size info)
        if (unreadData < 4)
            return;
        int i = 0;
        while(unreadData >= currentPacketData)
        {
            i++;
            byte[] subPacketData = packet.ReadBytes(currentPacketData);
            Packet subPacket = new Packet(subPacketData);
            subPacket.SetBytes();
            int packetID = subPacket.ReadInt();
            packetActions[packetID](subPacket);
            print(packetID);
            unreadData = packet.GetUndreadData();
            if (unreadData < 4) // there must be at least one more int to read when we finish each packet, else we reached the end
                break;
            currentPacketData = packet.ReadInt();
        }
    }

    public void SendData(byte[] data)
    {
        stream.BeginWrite(data, 0, data.Length, null, null);
    }

    private void OnApplicationQuit()
    {
        if(connected)
            ClientSend.RequestDisconnect();
    }
}
