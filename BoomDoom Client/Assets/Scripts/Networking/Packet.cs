using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ClientPackets
{
    Hello = 1, 
    MyPosition
}

public enum ServerPackets
{
    HelloReceived = 1, 
    PlayerPosition
}
public class Packet
{
    private byte[] arrayBuffer;
    private List<byte> listBuffer = new List<byte>();
    int readPos = 0;

    public Packet() {}

    public Packet(byte[] data)
    {
        
    }

    public Packet(int packetID)
    {
        Write(packetID);
    }

    #region Functions
    public byte[] ToArray()
    {
        arrayBuffer = listBuffer.ToArray();
        return arrayBuffer;
    }
    #endregion
    #region Write
    public void Write(byte value)
    {
        listBuffer.Add(value);
    }

    public void Write(byte[] value)
    {
        listBuffer.AddRange(value);
    }

    public void Write(int value)
    {
        listBuffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(float value)
    {
        listBuffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(bool value)
    {
        listBuffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(string value)
    {
        Write(value.Length); // number of characters in the string
        listBuffer.AddRange(Encoding.ASCII.GetBytes(value));
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(Vector2 value)
    {
        Write(value.x);
        Write(value.y);
    }
    #endregion
    #region Read
    public byte ReadByte()
    {
        byte value = arrayBuffer[readPos];
        readPos += 1;
        return value;
    }

    public byte[] ReadBytes(int length)
    {
        byte[] value = listBuffer.GetRange(readPos, length).ToArray();
        readPos += length;
        return value;
    }
    public int ReadInt()
    {
        int value = BitConverter.ToInt32(arrayBuffer, readPos);
        readPos += 4;
        return value;

    }

    public float ReadFloat()
    {
        float value = BitConverter.ToSingle(arrayBuffer, readPos);
        readPos += 4;
        return value;
    }

    public bool ReadBool()
    {
        bool value = BitConverter.ToBoolean(arrayBuffer, readPos);
        readPos += 1;
        return value;
    }

    public string ReadString()
    {
        int length = ReadInt();
        string value = Encoding.ASCII.GetString(arrayBuffer, readPos, length);
        readPos += length;
        return value;
    }

    public Vector3 ReadVector3()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        float z = ReadFloat();
        return new Vector3(x, y, z);
    }

    public Vector2 ReadVector2()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        return new Vector2(x, y);
    }
    #endregion
}
