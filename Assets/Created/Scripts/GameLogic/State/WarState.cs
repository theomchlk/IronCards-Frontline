using UnityEngine;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;

public class WarState : IGameState
{
    public GameStateType GameStateType => GameStateType.War;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.End , GameStateType.Planification };
    
    public void ExitServer()
    {
        Debug.Log($"WarState ExitServer");
        GameManager.Instance.IncreaseNbRounds();
    }

    public void ExitClient()
    {
        Debug.Log($"WarState ExitClient");
    }
        
    public void EnterClient()
    {
        Debug.Log($"WarState EnterClient");
    }

    public void EnterServer()
    {
        Debug.Log("WarState EnterServer");
        InstanceFinder.SceneManager.UnloadGlobalScenes(new SceneUnloadData("Noah_shop"));
        InstanceFinder.SceneManager.LoadGlobalScenes(new SceneLoadData("Noah"));
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
