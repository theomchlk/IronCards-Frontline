using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public abstract class ASpawnableItem : NetworkBehaviour
{
    public abstract void SpawnItem(NetworkConnection conn);
}
