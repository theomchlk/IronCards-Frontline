using System;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using LiteNetLib;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateController : NetworkBehaviour
{
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private int preparationDuration = 30;

    public static GameStateController Instance;
    /*private int _nbRounds = 0;*/

    public readonly SyncVar<int> preparationTimeRemaining = new();
    private Coroutine _preparationTimerCoroutine;
    
    public IGameState CurrentState { get; private set; }
    private IGameState _serverState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ClientManager.OnClientConnectionState += OnClientStateChanged;
    }
    

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log($"OnDestroy {ClientManager} and {InstanceFinder.ClientManager}");
        
    }

    
    /*public override void OnStopClient()
    {
        base.OnStopClient();
        CurrentState = null;
    }*/

    private void OnClientStateChanged(ClientConnectionStateArgs args)
    {
        
        Debug.Log($"IsServerStarted {InstanceFinder.IsServerStarted} and IsHost {InstanceFinder.IsHostStarted}");
        if (args.ConnectionState != LocalConnectionState.Stopped) return;
        
        if (InstanceFinder.IsServerStarted) 
        {
            Debug.Log("RemoveInLobbyList");
            LobbyBroadcaster.Instance.StopBroadcast();
            ServerManager.StopConnection(false);
            /*return;*/
            
        }
            
            
        if (CurrentState == null)
        {
            Debug.Log("CurrentState = null");
            FullReset();
            return;
        }
    
        /*if (CurrentState.GameStateType == GameStateType.Lobby)
        {
            Debug.Log("CurrentState = Lobby");
            InLobbyUI.Instance.RemoveAllPlayers();
            CurrentState = null;
            return;
        }*/
    
        Debug.Log("FullReset");
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientStateChanged;
        FullReset();
    }
    


    public override void OnStartServer()
    {
        base.OnStartServer();
        SetState(CreateState(GameStateType.Lobby));
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientStateChange;
    }

    private void OnClientStateChange(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (!IsServerStarted) return;
        if (conn.IsHost) return;
        if (args.ConnectionState != RemoteConnectionState.Stopped) return;
        if (CurrentState == null) return;
        if (_serverState == null) return;

        var ps = PlayerRegistry.GetPlayerState(conn.ClientId);
        if (ps == null) return;

        CurrentState.OnPlayerExit(ps);
    }


    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerEnter(PlayerState ps, NetworkConnection conn = null)
    {
        if (PlayerRegistry.GetPlayerState(conn.ClientId) != ps) return;
        _serverState.OnPlayerEnter(ps);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerExit(int playerId, NetworkConnection conn = null)
    {
        var ps = PlayerRegistry.GetPlayerState(playerId);
        if (ps == null) return;
        if (ps.IsHostStarted) return;
        if (PlayerRegistry.GetPlayerState(conn.ClientId) != ps) return;
        _serverState.OnPlayerExit(ps);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        PlayerRegistry.Clear();
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientStateChange;
        _serverState = null;
    }
    
    

    [ServerRpc(RequireOwnership = false)]
    public void ServerSetState(GameStateType type)
    {
        if (!_serverState.AllowedTransitions().Contains(type)) return;
        var newState = CreateState(type);
        Debug.Log($"ServerSetState {type}");
        SetState(newState);
    }

    [Server]
    private void SetState(IGameState newState)
    {
        Debug.Log($"SetState {newState.GetType().Name}");
        _serverState?.ExitServer();
        _serverState = newState;
        _serverState.EnterServer();
        
        ObserversSetState(newState.GameStateType);
    }

    /*private void Update()
    {
        CurrentState?.Update();
    }*/

    [ObserversRpc(BufferLast = true)]
    private void ObserversSetState(GameStateType type, NetworkConnection conn = null)
    {
        Debug.Log($"ObserversSetState {type}");
        Debug.Log($"State: {CurrentState} and type {type}");
        /*if (CurrentState != null && CurrentState.GameStateType == type) return;*/
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
    public void ServerStartGame(NetworkConnection conn = null)
    {
        if (!conn.IsHost) return;

        Debug.Log("ServerStartGame");

        SceneLoadData data = new SceneLoadData("Noah_shop");
        SceneManager.OnLoadEnd += OnGameSceneLoaded;
        SceneManager.LoadGlobalScenes(data);

        ObserversHideLobbyCanvas();
    }

    [Server]
    public void TransitionToWar()
    {
        AssignCampIndicesRandomly();
        SetState(new WarState());
    }

    [Server]
    private void AssignCampIndicesRandomly()
    {
        var allPlayers = new List<PlayerState>(PlayerRegistry.GetAll);
        if (UnityEngine.Random.Range(0, 2) == 1) allPlayers.Reverse();
        for (int i = 0; i < allPlayers.Count; i++)
            allPlayers[i].SetCampIndex(i);
    }

    [Server]
    public void StartPreparationTimer()
    {
        preparationTimeRemaining.Value = preparationDuration;
        if (_preparationTimerCoroutine != null) StopCoroutine(_preparationTimerCoroutine);
        _preparationTimerCoroutine = StartCoroutine(PreparationTimerCoroutine());
    }

    [Server]
    public void StopPreparationTimer()
    {
        if (_preparationTimerCoroutine == null) return;
        StopCoroutine(_preparationTimerCoroutine);
        _preparationTimerCoroutine = null;
    }

    private IEnumerator PreparationTimerCoroutine()
    {
        while (preparationTimeRemaining.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            preparationTimeRemaining.Value--;
        }
        TransitionToWar();
    }

    [ObserversRpc]
    private void ObserversHideLobbyCanvas()
    {
        LobbyFightManager.Instance.SetActive(false);
        if (lobbyCanvas != null)
            lobbyCanvas.gameObject.SetActive(false);
    }

    private void OnGameSceneLoaded(SceneLoadEndEventArgs args)
    {
        SceneManager.OnLoadEnd -= OnGameSceneLoaded;
        Debug.Log("Scene loaded !");
        SetStateWhenClientsReady(GameStateType.Preparation);

    }

    private void HandleClientsReadyForSettingState(GameStateType type)
    {
        OnAllClientsReady -= HandleClientsReadyForSettingState;
        ServerSetState(type);
    }

    [Server]
    public void SetStateWhenClientsReady(GameStateType type)
    {
        OnAllClientsReady += HandleClientsReadyForSettingState;
        _stateWhenAllClientsReady = type;
        AskClientIfReady();
        
    }

    private event Action<GameStateType> OnAllClientsReady;
    private GameStateType _stateWhenAllClientsReady;
    private int _nbClients;
    private int _nbClientReady;

    [Server]
    private void AskClientIfReady()
    {
        _nbClientReady = 0;
        _nbClients = ServerManager.Clients.Count;
        ObserversClientAreReady();
    }

    [ObserversRpc]
    private void ObserversClientAreReady()
    {
        Debug.Log("Client is Ready ?");
        // Au lieu de confirmer immédiatement, on attend que la scène soit chargée
        StartCoroutine(WaitForSceneAndConfirm());
    }

    private IEnumerator WaitForSceneAndConfirm()
    {
        // On attend que UIManager soit disponible
        yield return new WaitUntil(() => UIManager.Instance != null && 
                                         UIManager.Instance.shopItemUI != null);
        ServerConfirmReady();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerConfirmReady(NetworkConnection conn = null)
    {
        _nbClientReady++;
        Debug.Log($"client ready = {_nbClientReady}/{_nbClients}");
        if (_nbClients > _nbClientReady) return;
        try
        {
            Debug.Log("ServerConfirmReady");
            OnAllClientsReady?.Invoke(_stateWhenAllClientsReady);
        }
        catch
        {
            Debug.Log($"_stateWhenAllClientsReady {_stateWhenAllClientsReady}");
        }
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
    
    /*[TargetRpc]
    public void TargetEnterLobbyState(NetworkConnection conn, bool isLobbyLeader)
    {
        Debug.Log($"GameStateController::IsLobbyLeader: {isLobbyLeader}");
        MenuUIManager.Instance.SetLobbyStateUI(isLobbyLeader);
    }*/
    
    /*[ObserversRpc]
    public void ObserverEnterLobbyState(NetworkConnection conn, bool isLobbyLeader)
    {
        Debug.Log($"GameStateController::IsLobbyLeader: {isLobbyLeader}");
        MenuUIManager.Instance.SetLobbyStateUI(isLobbyLeader);
    }*/

    private void FullReset()
    {
        if (InstanceFinder.ServerManager.Started)
            InstanceFinder.ServerManager.StopConnection(true);
    
        if (InstanceFinder.ClientManager.Started)
            InstanceFinder.ClientManager.StopConnection();
    
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start Menu");
    }

}
