using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class LobbyState : IGameState
{
    public GameStateType GameStateType => GameStateType.Lobby;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Preparation };
        public void ExitServer()
        {
            Debug.Log($"LobbyState ExitServer");
        }

        public void ExitClient()
        {
            Debug.Log($"LobbyState ExitClient");
        }
        
        public void EnterClient()
        {
            Debug.Log($"LobbyState EnterClient");
        }



    public void EnterServer()
    {
        Debug.Log($"LobbyState EnterServer");
        // TargetRpc ne peut être envoyé que par le serveur

        foreach (var ps in PlayerRegistry.GetAll)
        {
            GameStateController.Instance.TargetEnterLobbyState(ps.Owner, ps.IsLobbyLeader());
        }
        
    }



    public void Update()
    {
        
    }

    public void OnPlayerEnter(PlayerState ps)
    {
        Debug.Log($"LobbyState OnPlayerEnter");
        GameStateController.Instance.ObserversSendMessage($"Player {InstanceFinder.ClientManager.Connection} connected");
    }

    public void OnPlayerExit(PlayerState playerState)
    {
        Debug.Log($"LobbyState OnPlayerExit");
        GameStateController.Instance.ObserversSendMessage($"Player {InstanceFinder.ClientManager.Connection} disconnected");
    }

}
