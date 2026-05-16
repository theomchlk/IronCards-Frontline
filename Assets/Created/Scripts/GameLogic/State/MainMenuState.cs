using UnityEngine;
using System.Collections.Generic;

public class MainMenuState : IGameState
{
    public GameStateType GameStateType => GameStateType.MainMenu;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Lobby };
    
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
