using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public int totalPlayers { get; private set; } = 0;
    [SerializeField] GameObject localPlayer;
    [SerializeField] GameObject pet;
    [SerializeField] GameObject onlinePlayer;

    public void SpawnLocalPlayer(int id)
    {
        GameObject newPlayer = Instantiate(localPlayer);
        Instantiate(pet);
        players.Add(id, newPlayer);
    }

    public void SpawnOnlinePlayer(int id)
    {
        GameObject newPlayer = Instantiate(onlinePlayer);
        players.Add(id, newPlayer);
    }
}
