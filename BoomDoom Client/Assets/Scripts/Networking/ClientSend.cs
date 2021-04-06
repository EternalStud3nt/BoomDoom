using System.Collections;
using System.Collections.Generic;

public static class ClientSend 
{
    public static void SendPacket(Packet packet)
    {
        byte[] buffer = packet.ToArray();
        Client.Instance.SendData(buffer);
    }
}
