using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class LobbyState : IGameState
{
    public GameStateType GameStateType => GameStateType.Lobby;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Preparation };

    public void Exit()
    {
        Debug.Log($"LobbyState Exit");
    }

    public void Enter()
    {
        Debug.Log($"LobbyState Enter");
        if (InstanceFinder.ClientManager.Started)
        {
            foreach (var ps in PlayerRegistry.GetAll)
            {
                GameStateController.Instance.TargetEnterLobbyState(ps.Owner,ps.IsLobbyLeader());
            }
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
