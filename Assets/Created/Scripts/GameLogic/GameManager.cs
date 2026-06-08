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
        /*SetPlayerStillInGame();*/
        SetOpponent();
    }
    
    private int SetPlayerStillInGame()
    {
        //On enlève les joueurs ayant perdu
        for (var i = playersInGame.Count - 1; i >= 0; i--)
        {
            if (playersInGame[i] == null || playersInGame[i].Hp <= 0) playersInGame.RemoveAt(i);
        }
        //On ajoute un joueur null si nombre de joueur impaire
        if (playersInGame.Count % 2 == 1)
        {
            playersInGame.Add(null);
            return playersInGame.Count - 1;
        }
        return playersInGame.Count;
    }

    private void SetOpponent()
    {
        // Copie locale pour ne pas modifier la liste originale
        List<PlayerState> list = new(playersInGame);

        // Si impair → on ajoute un joueur fantôme
        if (list.Count % 2 != 0)
            list.Add(null);

        int n = list.Count;
        int half = n / 2;

        // On calcule le round actuel (NbRounds commence à 1)
        int roundIndex = (NbRounds - 1) % (n - 1);

        // Rotation round-robin
        // On fixe le premier joueur, on fait tourner les autres
        List<PlayerState> rotated = new(list);
        for (int r = 0; r < roundIndex; r++)
        {
            var last = rotated[n - 1];
            rotated.RemoveAt(n - 1);
            rotated.Insert(1, last);
        }

        // On vide les duels
        _duelDictionary.Clear();

        // On génère les paires du round
        for (int i = 0; i < half; i++)
        {
            PlayerState p1 = rotated[i];
            PlayerState p2 = rotated[n - 1 - i];

            int id1 = p1 != null ? p1.IdPlayer : -1;
            int id2 = p2 != null ? p2.IdPlayer : -1;

            // On remplit le dictionnaire pour chaque joueur réel
            if (id1 != -1)
                _duelDictionary[id1] = id2;

            if (id2 != -1)
                _duelDictionary[id2] = id1;
        }
    }


    private void DuelChanged(SyncDictionaryOperation op, int key, int value, bool asServer)
    {
        if (asServer)
        {
            var conn = InstanceFinder.ServerManager.Clients[key];
            TargetSetOpponent(conn, value);
        }
    }

    
    [TargetRpc]
    private void TargetSetOpponent(NetworkConnection conn, int opponentId)
    {
        var opponent = PlayerRegistry.GetPlayerState(opponentId);
        UIManager.Instance.SetEnnemy(opponent, _nbRow, _nbCol);
    }

    public int nbDuelsEnd;
    public void SetEndDuel()
    {
        nbDuelsEnd++;

        var nbDuels = playersInGame.Count % 2 == 1 ? (playersInGame.Count - 1) / 2 : playersInGame.Count / 2;
        if (nbDuelsEnd >= nbDuels)
        {
            nbDuelsEnd = 0;
            OnEndWar();
        }
    }

    public void OnEndWar()
    {
        var gameStateController = GameStateController.Instance;
        int playersLastInGame = SetPlayerStillInGame();
        if (playersLastInGame == 1) gameStateController.SetState(new EndState());
        else gameStateController.SetState(new PreparationState());
    }

    public int GetWinner()
    {
        if (playersInGame.Count == 2 && playersInGame[1] == null || playersInGame.Count == 1) return playersInGame[0].IdPlayer;
        return -1;
    }

    
    
    
}
