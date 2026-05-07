using FishNet;
using UnityEngine;
using System.Collections.Generic;

public class PreparationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Preparation;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Planification };
    
    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Enter()
    {
        var isFirstRound = GameStateController.Instance.NbRounds == 0;
        if (InstanceFinder.ServerManager.Started)
        {
            foreach (PlayerState ps in PlayerRegistry.GetAll)
            {
                if (isFirstRound) ps.InitItemsFromDatabase();
                ps.SetNewMoney();
            }
        }
        
        if (InstanceFinder.ClientManager.Started)
        {
            foreach (PlayerState ps in PlayerRegistry.GetAll)
            {
                ps.UIManager.SetPreparationStateUI();
            }
        }
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerEnter(PlayerState playerState)
    {
        
    }

    public void OnPlayerExit(PlayerState playerState)
    {
        throw new System.NotImplementedException();
    }
}
