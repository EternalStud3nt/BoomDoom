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
        { (int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived}
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
        HandleData(receiveBuffer);
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    public void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        packet.SetBytes();
        packetActions[packet.ReadInt()](id, packet);
    }
}
