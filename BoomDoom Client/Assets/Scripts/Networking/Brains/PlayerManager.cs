using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType { None, Local,  Network}
public class PlayerManager : MonoBehaviour
{
    int clientID;
    public PlayerType playerType;

    public void Initialize(PlayerType type, int id)
    {
        this.clientID = id;
        playerType = type;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    private void FixedUpdate()
    {
        if(playerType == PlayerType.Local)
            ClientSend.MyPosition(transform.position);
    }
}