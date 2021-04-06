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
    int dataBufferSize = 4096;

    public void Connect(TcpClient socket)
    {
        receiveBuffer = new byte[dataBufferSize];
        this.socket = socket;
        stream = this.socket.GetStream();
        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        int dataSize = stream.EndRead(ar);
        Debug.Log("I received something");
        Debug.Log(BitConverter.ToInt32(receiveBuffer, 0));
    }
}
