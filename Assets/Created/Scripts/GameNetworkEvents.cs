using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using UnityEngine;

public class GameNetworkEvents : MonoBehaviour
{
    private void OnEnable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientStateChange;
    }

    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientStateChange;
    }

    private void OnClientStateChange(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            Debug.Log($"[SERVER] Client {conn.ClientId} s'est déconnecté");
            CleanupPlayerObjects(conn);
        }
    }

    private void CleanupPlayerObjects(NetworkConnection conn)
    {
        int id = conn.ClientId;

        foreach (var obj in InstanceFinder.ServerManager.Objects.Spawned)
        {
            if (obj.Value.TryGetComponent(out SlotItem slot))
            {
                /*if (slot.ownerId == id)
                {
                    InstanceFinder.ServerManager.Despawn(slot.gameObject);
                }*/
            }
        }
    }
}