using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;
    private LobbyData _hostLobbyData;
    
    void Awake()
    {
        Instance = this;
    }
    

    public override void OnStartClient()
    {
        base.OnStartClient();
        LobbyDiscovery.Instance.StopListening();
        if (IsServerStarted) return;
        Debug.Log("LobbyManager::OnStartClient");
        ServerTryJoinLobby();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        LobbyDiscovery.Instance.StartListening();
    }

    /*public override void OnStartClient()
    {
        base.OnStartClient();
        lobbyList.OnChange += OnLobbyListChanged;
        Debug.Log("LobbyManager::OnStartClient");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        lobbyList.OnChange -= OnLobbyListChanged;
    }
    
    private void OnDestroy()
    {
        if (!IsOwner) return;
        lobbyList.OnChange -= OnLobbyListChanged;
    }*/
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerManager.OnRemoteConnectionState += OnClientConnectionState;
        _hostLobbyData = LobbyBroadcaster.Instance.GetLobbyData();
    }
    
    private void OnClientConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        Debug.Log($"OnClientConnectionState : {conn.ClientId} → {args.ConnectionState}");
        if (args.ConnectionState != RemoteConnectionState.Stopped) return;

        PlayerLeaveInLobby(conn);

    }
    
    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerManager.OnRemoteConnectionState -= OnClientConnectionState;
    }
    
    

    [ServerRpc(RequireOwnership = false)]
    public void ServerTryJoinLobby(NetworkConnection conn = null)
    {
        if (!_hostLobbyData.isOpen)
        {
            TargetJoinLobbyFailed(conn, "The lobby is closed");
            return;
        }
        TargetJoinLobbySucceeded(conn, _hostLobbyData);
        AddPlayerInLobby(conn);
        
        return;
    }

    [TargetRpc]
    private void TargetJoinLobbySucceeded(NetworkConnection conn, LobbyData lobbyData)
    {
        LobbyDiscovery.Instance.ConnectionState("Connected !", 1, 1);
        MenuUIManager.Instance.SetLobbyPanelActive(false, lobbyData.lobbyName);
    }
    
    
    
    [TargetRpc]
    private void TargetJoinLobbyFailed(NetworkConnection conn, string message)
    {
        LobbyDiscovery.Instance.ConnectionState(message, 1, 1);
        conn.Disconnect(true);
    }

    public event Action OnAddPlayerInLobby;
    
    [Server]
    private void AddPlayerInLobby(NetworkConnection conn)
    {
        Debug.Log("AddPlayer in lobby ++");
        _hostLobbyData.nbPlayers++;
        if (_hostLobbyData.nbPlayers >= _hostLobbyData.maxPlayers)
        {
            _hostLobbyData.isOpen = false;
        }
        OnAddPlayerInLobby?.Invoke();
        _hostLobbyData.hasBeenModified = true;
        LobbyBroadcaster.Instance.UpdateLobbyData(_hostLobbyData);
    }

    [Server]
    private void PlayerLeaveInLobby(NetworkConnection conn)
    {
        Debug.Log($"PlayerLeaveInLobby appelé ! nbPlayers avant : {_hostLobbyData.nbPlayers}");
        if (_hostLobbyData.IsFull()) _hostLobbyData.isOpen = true;
        _hostLobbyData.nbPlayers--;
        Debug.Log($"nbPlayers après : {_hostLobbyData.nbPlayers}");
        Debug.Log($"conn.IsHost = {conn.IsHost}");
        /*if (conn.IsLocalClient || _hostLobbyData.nbPlayers <= 0 )
        {
            Debug.Log("RemoveInLobbyList");
            LobbyBroadcaster.Instance.StopBroadcast();
            ServerManager.StopConnection(false);
            return;
        }*/
        _hostLobbyData.hasBeenModified = true;
        LobbyBroadcaster.Instance.UpdateLobbyData(_hostLobbyData);
        ObserversSendMessage($"Player {conn.ClientId} left the lobby");
    }

    [ObserversRpc]
    private void ObserversSendMessage(string msg)
    {
        MessagePanelBehavior.Local.SetMessage(msg, 1, 1);
    }
    
    

}
