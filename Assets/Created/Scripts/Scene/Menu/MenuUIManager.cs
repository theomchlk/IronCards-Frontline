using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance;
    [SerializeField] private GameObject buttonStartGame;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;
    [SerializeField] private UILobbyList uiLobbyList;
    [SerializeField] private InLobbyUI lobbyPanel;
    [SerializeField] private GameObject personalisationPanel;
    [SerializeField] private TMP_InputField playerNameInputField;


    public void Awake()
    {
        Instance = this;
        UILobbyList.Local = uiLobbyList;
        InLobbyUI.Instance = lobbyPanel;
        personalisationPanel.SetActive(true);
        playerNameInputField.text = LocalPlayerPrefs.PlayerName;
    }

    public void OnEndEditName()
    {
        LocalPlayerPrefs.PlayerName = playerNameInputField.text;
    }

    public void SetLobbyPanelActive(bool isHost, string lobbyName)
    {
        for (var i = 0 ; i < transform.childCount-3 ; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        lobbyPanel.gameObject.SetActive(true);
        buttonStartGame.SetActive(isHost);
        lobbyPanel.SetLobby(lobbyName);
    }

    public void OnClickGenerateNameButton()
    {
        playerNameInputField.text = LocalPlayerPrefs.GenerateRandomName();
    }
    
    
    
    public void OnClickCreateLobby()
    {
        LobbyBroadcaster.Instance.CreateNewLobby(lobbyNameInputField.text, maxPlayersDropdown.value + 2);
        SetLobbyPanelActive(true, lobbyNameInputField.text);
    }


    public void OnClickLeaveLobby()
    {
        InstanceFinder.ClientManager.StopConnection();
        
        // Si c'est l'hôte, on arrête aussi le serveur
        /*
        if (InstanceFinder.ServerManager.Started)
            InstanceFinder.ServerManager.StopConnection(false);
            */
        
        /*InstanceFinder.ClientManager.StartConnection("127.0.0.1", 7780);*/
    }

    public void OnClickStartGame()
    {
        GameStateController.Instance.ServerStartGame();
    }

    /*public void SetLobbyStateUI(bool isLobbyLeader)
    {
        buttonStartGame.SetActive(isLobbyLeader);
    }*/

    public void OnClickExitGame()
    {
        Application.Quit();
    }

}
