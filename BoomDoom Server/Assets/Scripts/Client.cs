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
    bool disconnecting = false;
    public static Dictionary<int, Action<int, Packet>> packetActions = new Dictionary<int, Action<int, Packet>>
    {
        { (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived }, 
        { (int)ClientPackets.MyPosition, ServerHandle.MyPosition },
        { (int)ClientPackets.RequestDisconnect, ServerHandle.RequestDisconnect }
    };
    
    public Client(int id)
    {
        this.id = id;
    }

    public void Connect(TcpClient socket)
    {
        disconnecting = false;
        receiveBuffer = new byte[dataBufferSize];
        this.socket = socket;
        stream = this.socket.GetStream();
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        ServerSend.Welcome(id);
    }

    public void SendData(byte[] data)
    {
        try
        {
            stream.BeginWrite(data, 0, data.Length, null, null);
        }
        catch
        {
            Disconnect();
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int dataSize = stream.EndRead(ar);
            if (dataSize == 0)
            {
                ThreadManager.ExecuteOnMainThread(() => Disconnect());
                return;
            }
            byte[] data = new byte[dataSize];
            Array.Copy(receiveBuffer, data, dataSize);
            ThreadManager.ExecuteOnMainThread(() => HandleData(data));
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch
        {
            ThreadManager.ExecuteOnMainThread(() => Disconnect());
        }
        
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

    public void Disconnect()
    {
        if(!disconnecting)
        {
            disconnecting = true;
            Server.clients.TryGetValue(id, out Client client);
            if (client != null)
            {
                int id = client.id;
                ServerSend.PlayerDisconnected(id);
                Server.clients.Remove(id);
                socket.Close();
                Debug.Log($"Client {id} left the server.");
            }
        }        
    }
}
