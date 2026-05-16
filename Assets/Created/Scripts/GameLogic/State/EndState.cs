using UnityEngine;
using System.Collections.Generic;

public class EndState : IGameState
{
    public GameStateType GameStateType => GameStateType.End;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Lobby };
    
    public void ExitServer()
    {
        Debug.Log($"EndState ExitServer");
        
    }

    public void ExitClient()
    {
        Debug.Log($"EndState ExitClient");
    }
        
    public void EnterClient()
    {
        Debug.Log($"EndState EnterClient");
    }

    public void EnterServer()
    {
        Debug.Log($"EndState EnterServer");
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
