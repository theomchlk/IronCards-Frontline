using FishNet;
using UnityEngine;
using System.Collections.Generic;
using Created.Scripts.IU.Shop;
using FishNet.Managing.Scened;

public class PreparationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Preparation;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Planification };
    
    public void Exit()
    {
        Debug.Log($"Preparation Exit");
        if (InstanceFinder.ClientManager.Started)
        {
            UIManager.Instance.shopItemUI.CloseShuttereUI();
        }
    }

    public void Enter()
    {
        Debug.Log($"Preparation Enter");
        var isFirstRound = GameStateController.Instance.NbRounds == 0;

        if (InstanceFinder.ServerManager.Started)
        {
            foreach (PlayerState ps in PlayerRegistry.GetAll)
            {
                if (isFirstRound)
                {
                    ps.InitItemsFromDatabase();
                    ps.GetComponent<CardStallTable>().SetCardStallsOnTableByDataBase();
                }
                ps.SetNewMoney();
                
            }
            if (InstanceFinder.ClientManager.Started)
            {
                UIManager.Instance.shopItemUI.OpenShuttereUI();
            }
            /*GameStateController.Instance.ObserversEnterPreparationState();*/
        }
    }
    


    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerEnter(PlayerState playerState)
    {
        Debug.Log($"Preparation OnPlayerEnter");
    }

    public void OnPlayerExit(PlayerState playerState)
    {
        Debug.Log($"Preparation OnPlayerExit");
    }
}
