using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Connection;

public class InLobbyUI : MonoBehaviour
{
    public static InLobbyUI Instance;

    public void Awake()
    {
        Instance = this;
    }
    
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Transform playerLocation;
    [SerializeField] private TMP_Text lobbyName;
    private List<(PlayerState, PlayerPanel)> players = new ();

    private void Start()
    {
        LobbyCache.TryApply();
    }
    
    
    public void AddNewPlayer(PlayerState ps ,string namePlayer, Color color)
    {
        var go = Instantiate(playerPanel, playerLocation);
        var pp = go.GetComponent<PlayerPanel>();
        pp.SetPlayerPanel(namePlayer, color);
        pp.index = players.Count;
        players.Add((ps, pp));
        Debug.Log($"Player panel added {namePlayer}");

    }

    public void RemovePlayer(PlayerState ps)
    {
        for (var i = 0; i < players.Count; i++)
        {
            if (ps != players[i].Item1) continue;
            players.RemoveAt(i);
            Destroy(playerLocation.GetChild(i).gameObject);
        }
    }

    public void RemoveAllPlayers()
    {
        for (var i = 0; i < playerLocation.childCount; i++)
        {
            Destroy(playerLocation.GetChild(i).gameObject);
        }
        players.Clear();
    }
    

    public void SetLobby(string nameLobby)
    {
        lobbyName.text = nameLobby;
    }
}