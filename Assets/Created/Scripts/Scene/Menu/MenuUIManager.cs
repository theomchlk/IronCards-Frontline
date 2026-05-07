using FishNet;
using FishNet.Managing;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonStartGame;
    
    public void OnClickCreateLobby()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void OnClickJoinLobby(string address)
    {
        InstanceFinder.ClientManager.StartConnection(address);
    }

    public void OnClickStartGame()
    {
        GameStateController.Instance.ServerStartGame();
    }

}
