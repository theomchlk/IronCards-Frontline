using FishNet.Managing;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public NetworkManager networkManager;
    
    public void CreateLobby()
    {
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
    }
}
