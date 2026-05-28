using FishNet;
using FishNet.Connection;
using FishNet.Managing.Client;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private LobbyData _data;

    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyAddressText;
    [SerializeField] private TMP_Text lobbyPlayersText;
    [SerializeField] private Button joinLobbyButton;
    public float timerBeforeDelete = 5;
    private float _lastTimeSeen;

    void Awake()
    {
        ResetTimerSeen();
    }

    void Update()
    {
        _lastTimeSeen -= Time.deltaTime;
        if (_lastTimeSeen > 0) return;
        LobbyDiscovery.Instance.RemoveLobbyDataInList(_data.id);
        Destroy(gameObject);
        
    }

    public void SetUI(LobbyData data)
    {
        _data = data;
        lobbyNameText.text = data.lobbyName;
        lobbyAddressText.text = data.address;
        SetNbPlayersUI(data.nbPlayers, data.maxPlayers);
    }


    public void ResetTimerSeen()
    {
        _lastTimeSeen = timerBeforeDelete;
    }
    
    public void SetNbPlayersUI(int nbPlayers, int nbPlayersMax)
    {
        lobbyPlayersText.text = $"{nbPlayers} / {nbPlayersMax}";
    }

    public void OnClickJoinButton()
    {
        LobbyDiscovery.Instance.JoinLobby(_data.id);
        
    }
    
    public string LobbyId => _data.id;
    

    public void SetJoinFailed()
    {
        Debug.Log("Join failed");
    }
}
