using UnityEngine;
using System.Collections.Generic;

public class EndState : IGameState
{
    public GameStateType GameStateType => GameStateType.End;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Lobby };
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
