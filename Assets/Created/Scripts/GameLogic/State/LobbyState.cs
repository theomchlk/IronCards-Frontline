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
        InstanceFinder.SceneManager.LoadGlobalScenes(new SceneLoadData("Théo"));
    }

    public void Enter()
    {
        
    }

    public void Update()
    {
        
    }

    public void OnPlayerEnter(PlayerState ps)
    {

    }

    public void OnPlayerExit(PlayerState playerState)
    {
        throw new System.NotImplementedException();
    }

}
