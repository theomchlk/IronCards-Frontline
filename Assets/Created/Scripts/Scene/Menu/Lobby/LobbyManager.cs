using System;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    void Awake()
    {
        Instance = this;
    }

    private readonly SyncList<LobbyData> lobbyList = new();

    public override void OnStartClient()
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
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerManager.OnRemoteConnectionState += OnClientConnectionState;
    }

    private void OnClientConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != RemoteConnectionState.Stopped) return;
    
        // Trouver le lobby du joueur qui s'est déconnecté
        for (int i = 0; i < lobbyList.Count; i++)
        {
            if (lobbyList[i].address == conn.GetAddress())
            {
                PlayerLeaveInLobby(i);
                return;
            }
        }
    }

    /*public override void OnStartServer()
    {
        base.OnStartServer();
        ServerManager.OnRemoteConnectionState += OnClientConnectionState;
    }

    private void OnClientConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {

        }
    }*/

    private void OnLobbyListChanged(SyncListOperation op, int index, LobbyData oldData, LobbyData newData, bool asServer)
    {
        if (asServer) return;
        Debug.Log($"Op: {op} | index: {index} | old: {oldData.lobbyName} | new: {newData.lobbyName}");
        var uiLobbyList = UILobbyList.Local;
        Debug.Log($"OnLobbyListChanged {uiLobbyList}");
        switch (op)
        {
            case SyncListOperation.Add:
                if (uiLobbyList.PanelExists(newData.id))
                    uiLobbyList.UpdatePanel(newData.id, newData);
                else
                    uiLobbyList.AddNewPanel(newData);
                break;
            case SyncListOperation.RemoveAt:
                Debug.Log("LobbyList.RemoveAt");
                uiLobbyList.DestroyPanel(index);
                break;
            
            case SyncListOperation.Set:
                Debug.Log("LobbyList.Set");
                uiLobbyList.UpdatePanel(index, newData);
                break;
            
            
            case SyncListOperation.Complete:
                Debug.Log("LobbyList.Complete");
                break;
        }
            
    }
    
    private int currentLobbyIndex = 0;
     
    [ServerRpc(RequireOwnership = false)]
    public void ServerCreateLobby(string lobbyName, int maxPlayers,NetworkConnection conn = null)
    {
        var lobbyData = new LobbyData(
            currentLobbyIndex,
            lobbyName,
            conn.GetAddress(),
            7755,
            conn.ClientId,
            maxPlayers);
        Debug.Log($"ServerCreateLobby + currentLobbyIndex {currentLobbyIndex}");
        AddLobby(lobbyData);
        TargetOnLobbyCreated(conn, lobbyData.address, (ushort)lobbyData.port);
    }

    [Server]
    private void AddLobby(LobbyData lobbyData)
    {
        lobbyList.Add(lobbyData);
        currentLobbyIndex++;
        Debug.Log($"AddLobby + currentLobbyIndex {currentLobbyIndex}");
    }
    
    [TargetRpc]
    private void TargetOnLobbyCreated(NetworkConnection conn, string address, ushort port)
    {
        // Master Server a enregistré le lobby
        // Maintenant tu démarres ton Game Server
        InstanceFinder.ServerManager.StartConnection(port);
        InstanceFinder.ClientManager.StartConnection(address, port);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerJoinLobby(int idLobby, NetworkConnection conn = null)
    {
        Debug.Log($"[Server] ServerJoinLobby appelé pour lobby {idLobby}");
        for (var i = 0; i < lobbyList.Count; i++)
        {
            var lobby = lobbyList[i];
            if (lobby.id == idLobby)
            {
                if (lobby.IsFull())
                {
                    TargetJoinLobbyFailed(conn);
                    return;
                }
                AddPlayerInLobby(i, conn);
                TargetJoinLobby(conn, lobbyList[i]);
                return;
            }
        }
    }



    [TargetRpc]
    public void TargetJoinLobby(NetworkConnection conn, LobbyData lobbyData)
    {
        MenuUIManager.Instance.SetLobbyPanelActive();
        ClientManager.StopConnection();
        ClientManager.StartConnection(lobbyData.address, (ushort)lobbyData.port);
    }
    
    [TargetRpc]
    public void TargetJoinLobbyFailed(NetworkConnection conn)
    {
        MessagePanelBehavior.Local.SetMessage("The lobby is full", 1, 1);
    }

    public event Action OnAddPlayerInLobby;
    
    private void AddPlayerInLobby(int index, NetworkConnection conn)
    {
        var lobbyData = lobbyList[index];
        lobbyData.nbPlayers++;
        if (lobbyData.nbPlayers >= lobbyData.maxPlayers)
        {
            lobbyData.isOpen = false;
        }

        OnAddPlayerInLobby?.Invoke();
        Debug.Log($"[Server] Avant modification : {lobbyList[index].nbPlayers}");
        lobbyList[index] = lobbyData;
        Debug.Log($"[Server] Après modification : {lobbyList[index].nbPlayers}");
        
    }

    private void PlayerLeaveInLobby(int index)
    {
        LobbyData lobbyData = lobbyList[index];
        lobbyData.nbPlayers--;
        Debug.Log($"PlayerLeaveInLobby {lobbyData.nbPlayers}");
        if (lobbyData.nbPlayers <= 0)
        {
            Debug.Log("RemoveInLobbyList");
            lobbyList.RemoveAt(index);
            return;
        }
        if (lobbyData.IsFull()) lobbyData.isOpen = true;
        lobbyList[index] = lobbyData;
    }

}
