using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public int totalPlayers { get; private set; } = 0;
    [SerializeField] GameObject localPlayer;
    [SerializeField] GameObject pet;
    [SerializeField] GameObject onlinePlayer;

    public void SpawnLocalPlayer(int id)
    {
        GameObject newPlayer = Instantiate(localPlayer);
        PlayerManager manager = newPlayer.GetComponent<PlayerManager>();
        manager.Initialize(PlayerType.Local, id);
        Instantiate(pet);
        players.Add(id, manager);
    }

    public void SpawnNetworkPlayer(int id)
    {
        GameObject newPlayer = Instantiate(onlinePlayer);
        PlayerManager manager = newPlayer.GetComponent<PlayerManager>();
        manager.Initialize(PlayerType.Network, id);
        players.Add(id, manager);
    }

    public void DisconncectPlayer(int clientID)
    {
        Destroy(players[clientID].gameObject);
        players.Remove(clientID);
    }
}
