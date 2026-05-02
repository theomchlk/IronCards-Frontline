using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using UnityEngine;

public abstract class ASpawnableItem : NetworkBehaviour
{
    private readonly SyncVar<string> _itemId = new SyncVar<string>();
    protected string ItemId => _itemId.Value;
    protected void SetItemId(string itemId) => _itemId.Value = itemId;
    public abstract void SpawnItem(NetworkConnection conn, ItemSO itemData);
}
