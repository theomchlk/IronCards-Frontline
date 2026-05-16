using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance;
    [SerializeField] private GameObject buttonStartGame;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;
    [SerializeField] private UILobbyList uiLobbyList;
    [SerializeField] private GameObject lobbyPanel;

    public void Awake()
    {
        Instance = this;
        UILobbyList.Local = uiLobbyList;
    }

    public void SetLobbyPanelActive()
    {
        for (var i = 0 ; i < transform.childCount ; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        lobbyPanel.SetActive(true);
    }
    
    
    
    public void OnClickCreateLobby()
    {
        LobbyManager.Instance.ServerCreateLobby(nameInputField.text, maxPlayersDropdown.value + 2);
    }


    public void OnClickLeaveLobby()
    {
        InstanceFinder.ClientManager.StopConnection();
        
        // Si c'est l'hôte, on arrête aussi le serveur
        if (InstanceFinder.ServerManager.Started)
            InstanceFinder.ServerManager.StopConnection(false);
        
        InstanceFinder.ClientManager.StartConnection("127.0.0.1", 7780);
    }

    public void OnClickStartGame()
    {
        GameStateController.Instance.ServerStartGame();
    }

    public void SetLobbyStateUI(bool isLobbyLeader)
    {
        buttonStartGame.SetActive(isLobbyLeader);
    }

    public void OnClickExitGame()
    {
        Application.Quit();
    }

}
