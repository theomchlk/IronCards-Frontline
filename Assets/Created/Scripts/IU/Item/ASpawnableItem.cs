using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using UnityEngine;

public abstract class ASpawnableItem : NetworkBehaviour
{ 
    public abstract void Init(ItemSO itemData);

    public abstract void TargetSpawnItem(NetworkConnection conn, string itemId);
}
