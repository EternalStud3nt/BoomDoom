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
    public static Dictionary<int, Action<Packet>> packetActions = new Dictionary<int, Action<Packet>>
    {
        { (int)ClientPackets.Hello,  ServerHandle.HelloReceived },
        { (int)ClientPackets.MyPosition, ServerHandle.MyPosition }
    };
    
    public void Connect(TcpClient socket)
    {
        receiveBuffer = new byte[dataBufferSize];
        this.socket = socket;
        stream = this.socket.GetStream();
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    public void SendData(byte[] data)
    {
        stream.BeginWrite(data, 0, data.Length, null, null);
    }

    public static void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        packetActions[packet.ReadInt()](packet);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {       
        int dataSize = stream.EndRead(ar);
        byte[] data = new byte[dataSize];
        Array.Copy(receiveBuffer, data, dataSize);
        HandleData(receiveBuffer);
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }
}
