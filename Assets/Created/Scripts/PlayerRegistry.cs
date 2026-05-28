using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;

public static class PlayerRegistry
{
    private static Dictionary<int, PlayerState> players = new();

    public static void Register(int clientId, PlayerState ps)
    {
        ps.IdPlayer = clientId;
        players[clientId] = ps;
        if (players.Count == 1) ps.SetLobbyLeader();
    }

    public static void Unregister(int clientId)
    {
        players.Remove(clientId);
    }

    public static PlayerState GetPlayerState(int clientId)
    {
        return players.TryGetValue(clientId, out var ps) ? ps : null;
    }
    
    public static IEnumerable<PlayerState> GetAll => players.Values;
    public static int PlayerCount => players.Count;
    
    public static void Clear()
    {
        players.Clear();
    }

}
