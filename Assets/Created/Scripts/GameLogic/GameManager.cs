using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Connection;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int _nbRow, _nbCol;
    
    public readonly SyncVar<int> nbRow = new();
    public readonly SyncVar<int> nbCol = new();

    void Awake()
    {
        Instance = this;
        
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _rounds.Value = 1;
        nbRow.Value = _nbRow;
        nbCol.Value = _nbCol;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _duelDictionary.OnChange += DuelChanged;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _duelDictionary.OnChange -= DuelChanged;
    }

    public bool IsFirstRound => _rounds.Value == 1;
    
    private readonly SyncVar<int> _rounds = new();
    public readonly SyncDictionary<int, int> _duelDictionary = new();

    public int GetOpponent(int clientId)
    {
        Debug.Log($"Getting opponent for {clientId} and count: {_duelDictionary.Count}");
        
        return _duelDictionary[clientId];
    }

    
    private List<PlayerState> playersInGame = new();
    
    public int NbRounds => _rounds.Value;
    public void IncreaseNbRounds() => _rounds.Value++;
    
    public void InitGame(List<PlayerState> playersInGame)
    {
        this.playersInGame = playersInGame;
        if (playersInGame.Count % 2 == 1) playersInGame.Add(null);
        Debug.Log($"Players in the game: {playersInGame.Count}");
        SetOpponent();
    }

    public void InitRound()
    {
        SetPlayerStillInGame();
        SetOpponent();
    }
    
    private void SetPlayerStillInGame()
    {
        //On enlève les joueurs ayant perdu
        for (var i = playersInGame.Count - 1; i >= 0; i--)
        {
            if (playersInGame[i] == null || playersInGame[i].Hp <= 0) playersInGame.RemoveAt(i);
        }
        //On ajoute un joueur null si nombre de joueur impaire
        if (playersInGame.Count % 2 == 1) playersInGame.Add(null);
    }

    private void SetOpponent()
    {
        var nbPlayers = playersInGame.Count;
        _duelDictionary.Clear();
        for (var i = 0; i < nbPlayers ; i++)
        {
            if (!playersInGame[i]) return;
            PlayerState opponent = playersInGame[(i + NbRounds) % nbPlayers];
            if (opponent) _duelDictionary[playersInGame[i].IdPlayer] = opponent.IdPlayer;
            else _duelDictionary[playersInGame[i].IdPlayer] = -1;
        }
    }

    private void DuelChanged(SyncDictionaryOperation op, int key, int value, bool asServer)
    {
        if (asServer)
        {
            NetworkConnection conn = InstanceFinder.ServerManager.Clients[key];
            TargetSetOpponent(conn, value);
        }
    }

    
    [TargetRpc]
    private void TargetSetOpponent(NetworkConnection conn, int opponentId)
    {
        PlayerState opponent = PlayerRegistry.GetPlayerState(opponentId);
        UIManager.Instance.SetEnnemy(opponent, _nbRow, _nbCol);
    }

    
    
    
}
