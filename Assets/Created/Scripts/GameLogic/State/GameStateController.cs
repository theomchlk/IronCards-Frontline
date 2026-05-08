using FishNet.Connection;
using FishNet.Object;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine;

public class GameStateController : NetworkBehaviour
{
    
    public static GameStateController Instance;
    private int _nbRounds = 0;
    
    public IGameState CurrentState { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    public int NbRounds => _nbRounds;
    public void IncreaseNbRounds() => _nbRounds++;

    public override void OnStartServer()
    {
        SetState(new LobbyState());
    }
    

    [ServerRpc(RequireOwnership = false)]
    public void ServerSetState(GameStateType type)
    {
        if (!CurrentState.AllowedTransitions().Contains(type)) return;
        var newState = CreateState(type);
        Debug.Log("ServerSetState");
        SetState(newState);
    }

    [Server]
    private void SetState(IGameState newState)
    {
        Debug.Log($"SetState {newState.GetType().Name}");
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        
        ObserversSetState(newState.GameStateType);
    }

    /*private void Update()
    {
        CurrentState?.Update();
    }*/

    [ObserversRpc]
    private void ObserversSetState(GameStateType type)
    {
        if (CurrentState != null && CurrentState.GameStateType == type) return;
        CurrentState?.Exit();
        CurrentState = CreateState(type);
        CurrentState.Enter();
    }
    
    [ObserversRpc]
    public void ObserversSendMessage(string message)
    {
        Debug.Log($"GameStateController ObserversEnter");
        Debug.Log(message);
    }
    
    private IGameState CreateState(GameStateType type)
    {
        return type switch
        {
            GameStateType.MainMenu => new MainMenuState(),
            GameStateType.Lobby => new LobbyState(),
            GameStateType.Preparation => new PreparationState(),
            GameStateType.Planification => new PlanificationState(),
            GameStateType.War => new WarState(),
            GameStateType.End => new EndState(),
            _ => throw new System.Exception("Unknown state")
        };
    }

    /*[ServerRpc(RequireOwnership = false)]
    public void ServerStartGame()
    {
        if (!IsHostStarted) return;

        Debug.Log("ServerStartGame");

        SceneLoadData data = new SceneLoadData("Theo");
        SceneManager.OnLoadEnd += OnGameSceneLoaded; // ← callback attaché AVANT le chargement

       SceneManager.LoadGlobalScenes(data);
    }

    private void OnGameSceneLoaded(SceneLoadEndEventArgs args)
    {
        ServerSetState(GameStateType.Preparation);
    }*/
    
    [ServerRpc(RequireOwnership = false)]
    public void ServerStartGame()
    {
        if (!IsHostStarted) return;

        SceneLoadData data = new SceneLoadData("Theo");
        InstanceFinder.SceneManager.LoadGlobalScenes(data);

        // On écoute les clients
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientFinishedLoading;
    }

    private int clientsLoaded = 0;

    private void OnClientFinishedLoading(NetworkConnection conn, bool asServer)
    {
        clientsLoaded++;

        // Quand tous les joueurs sont prêts
        if (clientsLoaded == InstanceFinder.ServerManager.Clients.Count)
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientFinishedLoading;
            ServerSetState(GameStateType.Preparation);
        }
    }


    
    [ObserversRpc]
    public void ObserversEnterPreparationState()
    {
        
        UIManager.Instance.shopItemUI.OpenShuttereUI();
    }
    
    [TargetRpc]
    public void TargetEnterLobbyState(NetworkConnection conn, bool isLobbyLeader)
    {
        MenuUIManager.Instance.SetLobbyStateUI(isLobbyLeader);
    }
    
    


}
