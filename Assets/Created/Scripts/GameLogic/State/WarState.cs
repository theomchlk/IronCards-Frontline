using UnityEngine;
using System.Collections.Generic;
using FishNet;

public class WarState : IGameState
{
    public GameStateType GameStateType => GameStateType.War;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.End , GameStateType.Planification };
    
    public void Exit()
    {
        if (InstanceFinder.ServerManager.Started)
        {
            GameStateController.Instance.IncreaseNbRounds();
        }
    }

    public void Enter()
    {
        
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerEnter(PlayerState playerState)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerExit(PlayerState playerState)
    {
        throw new System.NotImplementedException();
    }
}
