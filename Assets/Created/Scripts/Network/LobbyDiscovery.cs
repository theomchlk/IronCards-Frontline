using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using System.Collections.Concurrent;
using FishNet.Transporting;

public class LobbyDiscovery : MonoBehaviour
{
    public static LobbyDiscovery Instance;
    private UdpClient _client;
    private Thread _thread;
    private bool _isListening = false;
    [SerializeField] private int timeOut = 1000;
    private LobbyData _lobbyDataReceived;
    private List<LobbyData> lobbyList = new();
    private int _currentLobbyIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartListening();
    }

    private LobbyData ByteToLobbyData(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        LobbyData lobbyData = JsonUtility.FromJson<LobbyData>(json);
        return lobbyData;
    }

    private void StartListening()
    {
        //Un peu moche mais c'est uniquement avec le multiplayer play mode donc...
        try
        {
            _client = new UdpClient(new IPEndPoint(IPAddress.Any, 7779));
            _client.EnableBroadcast = true;
            _client.Client.ReceiveTimeout = timeOut;

            _thread = new Thread(ListenLobbyData)
            {
                IsBackground = true
            };
            _thread.Start();
        }
        catch
        {
            Debug.Log("Can't open UDP client : Port 7779 already used");
        }

    }
    
    
    private ConcurrentQueue<LobbyData> _pendingLobbies = new();

    private void ListenLobbyData()
    {
        _isListening = true;
        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        while (_isListening)
        {
            try
            {
                
                byte[] data = _client.Receive(ref remote);
                LobbyData lobbyData = ByteToLobbyData(data);
            
                _pendingLobbies.Enqueue(lobbyData);
            }
            catch (SocketException) { }
        }
        StopBroadcast();
    }
    private void Update()
    {
        while (_pendingLobbies.TryDequeue(out LobbyData lobbyData))
        {
            OnLobbyDataReceive(lobbyData);
        }
    }
    
    private void OnLobbyDataReceive(LobbyData lobbyData)
    {
        for (var i = 0; i < lobbyList.Count; i++)
        {
            if (lobbyList[i].id == lobbyData.id)
            {
                UpdateLobbyDataInList(lobbyData, i);
                return;
            }
        }
        AddLobbyDataInList(lobbyData);
    }
    
    private void AddLobbyDataInList(LobbyData lobbyData)
    {
        var uiLobbyList = UILobbyList.Local;
        
        AddLobby(lobbyData);
        uiLobbyList.AddNewPanel(lobbyData);
    }
    
    private void AddLobby(LobbyData lobbyData)
    {
        lobbyList.Add(lobbyData);
        _currentLobbyIndex++;
        Debug.Log($"AddLobby + currentLobbyIndex {_currentLobbyIndex}");
    }
    

    private void UpdateLobbyDataInList(LobbyData lobbyData, int index)
    {
        var uiLobbyList = UILobbyList.Local;
        lobbyList[index] = lobbyData;
        uiLobbyList.UpdatePanel(index, lobbyData);
    }
    
    public void RemoveLobbyDataInList(string lobbyId)
    {
        for (var i = 0; i < lobbyList.Count; i++)
        {
            if (lobbyList[i].id == lobbyId) lobbyList.RemoveAt(i);
        }
    }

    public void StopListening()
    {
        _isListening = false;
    }

    private void StopBroadcast()
    {
        _client?.Close();
        _client = null;
    }
    
    public void JoinLobby(string idLobby)
    {
        Debug.Log($"[Server] ServerJoinLobby appelé pour lobby {idLobby}");
        foreach (LobbyData lobby in lobbyList)
        {
            if (lobby.id == idLobby)
            {
                ConnectToLobby(lobby);
                return;
            }
        }
        ConnectionFailed("Error: This lobby id don't match with any id in the list", 3 , 1);
    }

    private void ConnectToLobby(LobbyData lobbyData)
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnConnectionResult;
        InstanceFinder.ClientManager.StartConnection(lobbyData.address, lobbyData.port);
        MessagePanelBehavior.Local.SetMessage("Connection....", 10, 1);
    }

    private void OnConnectionResult(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            // Connexion échouée
            ConnectionFailed("Connection failed", 3, 1);
            InstanceFinder.ClientManager.OnClientConnectionState -= OnConnectionResult;
        }
        else if (args.ConnectionState == LocalConnectionState.Started)
        {
            // Connexion réussie
            InstanceFinder.ClientManager.OnClientConnectionState -= OnConnectionResult;
        }
    }

    public void ConnectionFailed(string message, float timeStay, float timeFade)
    {
        MessagePanelBehavior.Local.SetMessage(message, timeStay, timeFade);
    }
    


}
