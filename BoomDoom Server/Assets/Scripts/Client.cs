using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using UnityEngine;
using System.Net.Sockets;

public class Client
{
    TcpClient socket;
    NetworkStream stream;
    private byte[] receiveBuffer;
    readonly int dataBufferSize = 4096;
    public int id { get; private set; }
    public static Dictionary<int, Action<int, Packet>> packetActions = new Dictionary<int, Action<int, Packet>>
    {
        { (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived }, 
        { (int)ClientPackets.MyPosition, ServerHandle.MyPosition }
    };
    
    public Client(int id)
    {
        this.id = id;
    }

    public void Connect(TcpClient socket)
    {
        receiveBuffer = new byte[dataBufferSize];
        this.socket = socket;
        stream = this.socket.GetStream();
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        ServerSend.Welcome(id);
    }

    public void SendData(byte[] data)
    {
        stream.BeginWrite(data, 0, data.Length, null, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {       
        int dataSize = stream.EndRead(ar);
        byte[] data = new byte[dataSize];
        Array.Copy(receiveBuffer, data, dataSize);
        ThreadManager.ExecuteOnMainThread(() => HandleData(data));
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        packet.SetBytes();
        int currentPacketData = packet.ReadInt();   // first packet to arrive size (except int that contains size info)
        int unreadData = packet.GetUndreadData();   // whole packet size (except int that contains first packet's size info)
        if (unreadData < 4)
            return;
        int i = 0;
        while (unreadData >= currentPacketData)
        {
            Debug.Log("num of packets in one stream: " + i);
            i++;
            byte[] subPacketData = packet.ReadBytes(currentPacketData);
            Packet subPacket = new Packet(subPacketData);
            subPacket.SetBytes();
            packetActions[subPacket.ReadInt()](id, subPacket);
            unreadData = packet.GetUndreadData();
            if (unreadData < 4) // there must be at least one more int to read when we finish each packet, else we reached the end
                break;
            currentPacketData = packet.ReadInt();
        }
    }
}
