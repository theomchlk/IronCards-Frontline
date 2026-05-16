using UnityEngine;
using FishNet;
using FishNet.Transporting;


public class NetworkBootstrap : MonoBehaviour
{
    [SerializeField] private string masterServerAddress = "127.0.0.1";
    [SerializeField] private ushort masterServerPort = 7776;

#if UNITY_EDITOR
    [SerializeField] private bool isMasterServer = false;
#endif

    void Start()
    {
#if !UNITY_EDITOR
        #if UNITY_SERVER
            StartMasterServer();
            return;
        #endif
#endif

        if (isMasterServer)
        {
            StartMasterServer();
            return;
        }

        StartClient();
    }

    private void StartMasterServer()
    {
        Application.targetFrameRate = 5;
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerStarted;
        InstanceFinder.ServerManager.StartConnection(masterServerPort);
    }

    private void StartClient()
    {
        Debug.Log($"[Client] Connexion au Master Server {masterServerAddress}:{masterServerPort}");
        InstanceFinder.ClientManager.StartConnection(masterServerAddress, masterServerPort);
    }

    private void OnServerStarted(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
            Debug.Log($"[Master Server] Démarré ! Adresse : {masterServerAddress} | Port : {masterServerPort}");
    }
}