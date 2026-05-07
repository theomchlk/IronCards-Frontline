using UnityEngine;
using System.Collections.Generic;

public class PlanificationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Planification;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.War };
    
    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
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
