using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FishNet.Broadcast;
using UnityEngine;
using FishNet;

public class LobbyBroadcaster : MonoBehaviour
{
    public static LobbyBroadcaster Instance;
    private UdpClient _client;
    private LobbyData _hostLobbyData;
    private ushort _portServer = 7791;
    private ushort _portBroadcast = 7779;
    private Coroutine _sendDataCoroutine;
    private bool _isSending = false;

    void Awake()
    {
        Instance = this;
    }

    public void CreateNewLobby(string lobbyName, int maxPlayers)
    {
        var lobbyData = new LobbyData(
            lobbyName,
            GetLocalIPAddress(),
            _portServer,
            maxPlayers);
        _hostLobbyData = lobbyData;
        Debug.Log($"Max players: {maxPlayers}");
        StartHostLobby();
        SendLobbyDataBroadcast();
    }
    
    public LobbyData GetLobbyData() => _hostLobbyData;

    private void StartHostLobby()
    {
        /*LobbyManager.Instance.ServerCreateLobbyConnection(lobbyData.address, lobbyData.port);*/
        InstanceFinder.ServerManager.StartConnection(_hostLobbyData.port);
        InstanceFinder.ClientManager.StartConnection(_hostLobbyData.address, _hostLobbyData.port);
    }
    
    private string GetLocalIPAddress()
    {
        try
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address.ToString();
        }
        catch
        {
            return "127.0.0.1"; // Fallback localhost
        }
    }
    
    private byte[] LobbyDataToJson(LobbyData lobbyData)
    {
        string json = JsonUtility.ToJson(lobbyData);
        byte[] data = Encoding.UTF8.GetBytes(json);
        return data;
    }
    
    private void SendLobbyDataBroadcast()
    {
        if (_sendDataCoroutine != null)
        {
            StopCoroutine(_sendDataCoroutine);
            _sendDataCoroutine = null;
        }
        _client?.Close();
        
        _client = new UdpClient();
        _client.EnableBroadcast = true;
        _isSending = true;
        _sendDataCoroutine = StartCoroutine(SendByte());

    }
    
    public void UpdateLobbyData(LobbyData lobbyData) => _hostLobbyData = lobbyData;

    private IEnumerator SendByte()
    {
        while (_isSending)
        {
       
            byte[] data = LobbyDataToJson(_hostLobbyData);
            var _broadcast = new IPEndPoint(IPAddress.Broadcast, _portBroadcast);
            _client.Send(data, data.Length, _broadcast);
            Debug.Log("LobbyBroadcast::Data send");
            Debug.Log($"LobbyBroadcast::_isSending: {_isSending}");
            Debug.Log($"LobbyBroadcast::_sendCoroutine: {_sendDataCoroutine}");
            yield return new WaitForSeconds(2f);
        }
    }
    
    private void OnApplicationQuit()
    {
        StopBroadcast();
    }

    private void OnDestroy()
    {
        StopBroadcast();
    }
    
    
    public void StopBroadcast()
    {
        if (!_isSending) return;
        
        _isSending = false;
        
        if (_sendDataCoroutine != null)
        {
            StopCoroutine(_sendDataCoroutine);
            _sendDataCoroutine = null;
        }

        _client?.Close();
        _client = null;
    }
}
