using FishNet;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using Created.Scripts.IU.Shop;
using FishNet.Managing.Scened;

public class PreparationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Preparation;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Planification };
    
    public void ExitServer()
    {
        Debug.Log($"PreparationState ExitServer");
    }

    public void ExitClient()
    {
        Debug.Log($"PreparationState ExitClient");
       
        UIManager.Instance.shopItemUI.CloseShuttereUI();
    }
    
    public void EnterClient()
    {
        Debug.Log($"PreparationState EnterClient");
        Debug.Log($"UIManager {UIManager.Instance} )");
        Debug.Log($"and ShopItemUI {UIManager.Instance.shopItemUI}");
        UIManager.Instance.shopItemUI.OpenShuttereUI();
    }
    

    public void EnterServer()
    {
        Debug.Log($"Preparation EnterServer");
        var isFirstRound = GameStateController.Instance.NbRounds == 0;
        
        foreach (PlayerState ps in PlayerRegistry.GetAll) 
        {
            if (isFirstRound)
            { 
                ps.InitItemsFromDatabase(); 
                ps.GetComponent<CardStallTable>().SetCardStallsOnTableByDataBase();
            }
            else ps.SetNewMoney();
                
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
