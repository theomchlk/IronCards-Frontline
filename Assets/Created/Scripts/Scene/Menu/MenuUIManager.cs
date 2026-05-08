using FishNet;
using FishNet.Managing;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance;
    [SerializeField] private GameObject buttonStartGame;

    public void Awake()
    {
        Instance = this;
    }
    
    
    
    public void OnClickCreateLobby()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void OnClickJoinLobby(string address)
    {
        InstanceFinder.ClientManager.StartConnection(address);
    }

    public void OnClickLeaveLobby()
    {
        InstanceFinder.ClientManager.StopConnection();
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
