using FishNet.Connection;
using FishNet.Object;
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
    

    [ServerRpc]
    public void ServerSetState(GameStateType type)
    {
        if (!CurrentState.AllowedTransitions().Contains(type)) return;
        var newState = CreateState(type);
        SetState(newState);
    }

    [Server]
    private void SetState(IGameState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        
        ObserversSetState(newState.GameStateType);
    }

    private void Update()
    {
        CurrentState?.Update();
    }

    [ObserversRpc]
    private void ObserversSetState(GameStateType type)
    {
        CurrentState?.Exit();
        CurrentState = CreateState(type);
        CurrentState.Enter();
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

    [ServerRpc]
    public void ServerStartGame()
    {
        if (!IsHostStarted) return;
        ServerSetState(GameStateType.Preparation);
    }

}
