using UnityEngine;
using System.Collections.Generic;
public interface IGameState
{
    public GameStateType GameStateType { get; }

    public List<GameStateType> AllowedTransitions();
    
    public void ExitServer();
    public void ExitClient();
    public void EnterServer();
    public void EnterClient();
    
    public void Update();
    
    public void OnPlayerEnter(PlayerState playerState);
    public void OnPlayerExit(PlayerState playerState);
}
