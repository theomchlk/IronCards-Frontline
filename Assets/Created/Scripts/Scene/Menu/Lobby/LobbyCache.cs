using UnityEngine;
using System.Collections.Generic;
using FishNet.Connection;

public static class LobbyCache 
{
    public static List<(PlayerState, string, Color)> PendingPlayers = new ();

    public static void TryApply()
    {
        if (InLobbyUI.Instance == null) return;
        
        foreach (var ps in PendingPlayers)
            InLobbyUI.Instance.AddNewPlayer(ps.Item1, ps.Item2, ps.Item3);

        PendingPlayers.Clear();
    }
}
