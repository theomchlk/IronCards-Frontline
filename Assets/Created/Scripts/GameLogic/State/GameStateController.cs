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
        CurrentState?.ExitServer();
        CurrentState = newState;
        CurrentState.EnterServer();
        
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
        CurrentState?.ExitClient();
        CurrentState = CreateState(type);
        CurrentState.EnterClient();
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

    [ServerRpc(RequireOwnership = false)]
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
        SceneManager.OnLoadEnd -= OnGameSceneLoaded;
        ServerSetState(GameStateType.Preparation);
    }
    
    /*private int _clientsLoaded = 0;
    private int _clientsReady = 0;
    private int _expectedClients = 0;

    [ServerRpc(RequireOwnership = false)]
    public void ServerStartGame()
    {
        if (!IsHostStarted) return;

        _clientsLoaded = 0;
        _clientsReady  = 0;
        _sceneLoadHandled = false; // ← reset
        _expectedClients = InstanceFinder.ServerManager.Clients.Count;

        Debug.Log($"Lancement avec {_expectedClients} clients");

        SceneLoadData data = new SceneLoadData("Theo");
        InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoadEnd;
        InstanceFinder.SceneManager.LoadGlobalScenes(data);
    }

    private bool _sceneLoadHandled = false;

    private void OnSceneLoadEnd(SceneLoadEndEventArgs args)
    {
        Debug.Log($"OnLoadEnd — AsServer:{args.QueueData.AsServer} " +
                  $"IsServer:{IsServerStarted} " +
                  $"Scenes:{string.Join(", ", System.Array.ConvertAll(args.LoadedScenes, s => s.name))}");

        if (!IsServerStarted) return; // ← utilise ça à la place
        if (_sceneLoadHandled) return;
        if (args.LoadedScenes == null || args.LoadedScenes.Length == 0) return;

        bool theoLoaded = false;
        foreach (var scene in args.LoadedScenes)
            if (scene.name == "Theo") theoLoaded = true;
        if (!theoLoaded) return;

        _sceneLoadHandled = true;
        InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoadEnd;

        Debug.Log("Scène Theo chargée → RpcAskClientsReady");
        RpcAskClientsReady();
    }

    [ObserversRpc]
    private void RpcAskClientsReady()
    {
        ServerConfirmReady();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerConfirmReady()
    {
        _clientsReady++;
        Debug.Log($"Clients prêts : {_clientsReady} / {_expectedClients}");

        if (_clientsReady >= _expectedClients)
        {
            // ✅ Appel direct au lieu de passer par un ServerRpc
            var newState = CreateState(GameStateType.Preparation);
            SetState(newState);
        }
    }   
    */



    
    [ObserversRpc]
    public void ObserversEnterPreparationState()
    {
        
        UIManager.Instance.shopItemUI.OpenShuttereUI();
    }
    
    [TargetRpc]
    public void TargetEnterLobbyState(NetworkConnection conn, bool isLobbyLeader)
    {
        Debug.Log($"GameStateController::IsLobbyLeader: {isLobbyLeader}");
        MenuUIManager.Instance.SetLobbyStateUI(isLobbyLeader);
    }
    
    


}
