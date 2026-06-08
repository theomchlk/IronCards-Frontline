using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;
using UnityEngine;
using FishNet.Connection;

public class LobbyState : IGameState
{
    public GameStateType GameStateType => GameStateType.Lobby;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Preparation };
    public void ExitServer()
    {
        Debug.Log($"LobbyState ExitServer");
        LobbyBroadcaster.Instance.StopBroadcast();
    }

    public void ExitClient()
    {
        Debug.Log($"LobbyState ExitClient");
        LobbyFightManager.Instance.DesactiveCamera();
    }
    
    public void EnterClient()
    {
        Debug.Log($"LobbyState EnterClient");
        
        /*var isHost = InstanceFinder.IsHostStarted;
        if (!isHost)
        {
            LobbyDiscovery.Instance.StopListening();

        }*/
        /*MenuUIManager.Instance.SetLobbyStateUI(isHost);*/
    }



    public void EnterServer()
    {
        Debug.Log($"LobbyState EnterServer");
        /*foreach (var ps in PlayerRegistry.GetAll)
        {
            GameStateController.Instance.TargetEnterLobbyState(ps.Owner, ps.IsLobbyLeader());
        }*/
        
    }



    public void Update()
    {
        
    }

    public void OnPlayerEnter(PlayerState ps)
    {
        Debug.Log($"LobbyState OnPlayerEnter");
        GameStateController.Instance.ObserversSendMessage($"Player {InstanceFinder.ClientManager.Connection} connected");

    }

    public void OnPlayerExit(PlayerState ps)
    {
        Debug.Log($"LobbyState OnPlayerExit");
        GameStateController.Instance.ObserversSendMessage($"Player {InstanceFinder.ClientManager.Connection} disconnected");
        ps.ObserversRemovePanelPlayerInLobby(ps);
    }

}
