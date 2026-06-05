using FishNet;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Created.Scripts.IU.Shop;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Managing.Scened;
using FishNet.Managing.Server;

public class PreparationState : IGameState
{
    public GameStateType GameStateType => GameStateType.Preparation;
    public List<GameStateType> AllowedTransitions() => new() { GameStateType.Planification, GameStateType.War };
    
    public void ExitServer()
    {
        Debug.Log($"PreparationState ExitServer");
        GameStateController.Instance.StopPreparationTimer();
    }

    public void ExitClient()
    {
        Debug.Log($"PreparationState ExitClient");
       
        UIManager.Instance.shopItemUI.CloseShuttereUI();
    }
    
    public void EnterClient()
    {
        Debug.Log($"PreparationState EnterClient");
        var uiManager = UIManager.Instance;
        var gameManager = GameManager.Instance;
        var ps = PlayerRegistry.GetPlayerState(InstanceFinder.ClientManager.Connection.ClientId);
        
        Debug.Log($"Connection {InstanceFinder.ClientManager.Connection} and id {InstanceFinder.ClientManager.Connection.ClientId}");
        Debug.Log($"UIManager {UIManager.Instance} )");
        Debug.Log($"and ShopItemUI {UIManager.Instance.shopItemUI}");
        Debug.Log($"and ps {ps}");
        uiManager.Bind(ps);
        uiManager.shopItemUI.OpenShuttereUI();
        if (gameManager.IsFirstRound) uiManager.uiCamp.SetUI(ps, gameManager.nbRow.Value, gameManager.nbCol.Value);

    }
    

    public void EnterServer()
    {
        var gameManager = GameManager.Instance;
        Debug.Log($"Preparation EnterServer");
        Debug.Log($"PlayerRegistry {PlayerRegistry.GetAll}");
        if (gameManager.IsFirstRound) gameManager.InitGame(PlayerRegistry.GetAll.ToList());
        else gameManager.InitRound();
        
        foreach (PlayerState ps in PlayerRegistry.GetAll) 
        {
            Debug.Log($"Player in PlayerRegistry {ps}");
            if (gameManager.IsFirstRound)
            { 
                
                ps.InitItemsFromDatabase(); 
                ps.GetComponent<CardStallTable>().SetCardStallsOnTableByDataBase();
                /*UIManager.Instance.uiCamp.SetUI(ps);*/
            }
            else ps.SetNewMoney();
            /*UIManager.Instance.SetEnnemy(gameManager.GetOpponent(ps));*/
        }

        GameStateController.Instance.StartPreparationTimer();
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
