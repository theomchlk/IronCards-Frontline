using UnityEngine;
using System.Collections.Generic;

public class PlanificationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Planification;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.War };
    
    public void ExitServer()
    {
        Debug.Log($"PlanificationState ExitServer");
        
    }

    public void ExitClient()
    {
        Debug.Log($"PlanificationState ExitClient");
    }
        
    public void EnterClient()
    {
        Debug.Log($"PlanificationState EnterClient");
    }

    public void EnterServer()
    {
        Debug.Log($"PlanificationState EnterServer");
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
