using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;

public static class PlayerRegistry
{
    private static Dictionary<NetworkConnection, PlayerState> players = new();

    public static void Register(NetworkConnection conn, PlayerState ps)
    {
        players[conn] = ps;
        if (players.Count == 1) ps.SetLobbyLeader();
    }

    public static void Unregister(NetworkConnection conn)
    {
        players.Remove(conn);
    }

    public static PlayerState GetPlayerState(NetworkConnection conn)
    {
        return players.TryGetValue(conn, out var ps) ? ps : null;
    }
    
    public static IEnumerable<PlayerState> GetAll => players.Values;

    public static void DisplayAllInformations()
    {
        Debug.Log("Current Players in Registry:");
        Debug.Log($"Total Players: {players.Count}");
        foreach (var conn in players)
        {
            var ps = conn.Value;
            Debug.Log($"Player : IsLeader={ps.IsLobbyLeader()} | isOwner={ps.IsOwner}"); 
        }
    }
}
