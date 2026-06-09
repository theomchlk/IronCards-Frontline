using UnityEngine;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;

public class PlanificationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Planification;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.War };

    public void EnterServer()
    {
        Debug.Log($"PlanificationState EnterServer");
        InstanceFinder.SceneManager.LoadGlobalScenes(new SceneLoadData(GameStateController.Instance.Scenes.Planification));
        GameStateController.Instance.StartPreparationTimer();
    }

    public void ExitServer()
    {
        Debug.Log($"PlanificationState ExitServer");
        GameStateController.Instance.StopPreparationTimer();
        InstanceFinder.SceneManager.UnloadGlobalScenes(new SceneUnloadData(GameStateController.Instance.Scenes.Planification));
    }

    public void EnterClient()
    {
        Debug.Log($"PlanificationState EnterClient");
    }

    public void ExitClient()
    {
        Debug.Log($"PlanificationState ExitClient");
    }

    public void Update() { }

    public void OnPlayerEnter(PlayerState playerState) { }

    public void OnPlayerExit(PlayerState playerState) { }
}
