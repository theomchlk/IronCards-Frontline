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



    public void SetUI(LobbyData data)
    {
        _data = data;
        lobbyNameText.text = data.lobbyName;
        lobbyAddressText.text = data.address;
        SetNbPlayersUI(data.nbPlayers, data.maxPlayers);

    }
    
    
    public void SetNbPlayersUI(int nbPlayers, int nbPlayersMax)
    {
        lobbyPlayersText.text = $"{nbPlayers} / {nbPlayersMax}";
    }

    public void OnClickJoinButton()
    {
        LobbyManager.Instance.ServerJoinLobby(_data.id);
        
    }
    
    public int LobbyId => _data.id;
    

    public void SetJoinFailed()
    {
        Debug.Log("Join failed");
    }
}
