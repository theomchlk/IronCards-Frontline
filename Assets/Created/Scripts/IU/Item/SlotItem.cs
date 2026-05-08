using System;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class SlotItem : ASpawnableItem
{
    /*private readonly SyncVar<string> _itemId = new SyncVar<string>();*/
    private SlotSO _data;
    private PlayerState _ownerPlayerState;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        // On prévient le serveur que ce slot est maintenant visible côté client.
        // RequireOwnership = false car l'Owner du SlotItem n'est PAS le joueur.
        NotifyReadyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void NotifyReadyServerRpc(NetworkConnection conn = null)
    {
        // Retrouve le PlayerState qui possède ce SlotItem
        if (_ownerPlayerState == null) return;
        if (conn != _ownerPlayerState.Owner) return;
        _ownerPlayerState.OnSlotClientReady(this, conn);
    }
    
    [Server]
    public void SetOwnerPlayerState(PlayerState ps)
    {
        _ownerPlayerState = ps;
    }
    
    [Server]
    public bool IsOwnerPlayerState(PlayerState ps) => _ownerPlayerState == ps;

    
    [TargetRpc]
    public override void TargetSpawnItem(NetworkConnection conn, string itemId)
    {
        /*if (conn != Owner)
        {
            Debug.Log("C'est pas à toi mon poulet ça");
            return;
        }*/
        var uiManager = UIManager.Instance;
        if (_data == null) _data = (SlotSO)DataBaseItem.Instance.GetDataItem(itemId);
        uiManager. uiSlotShop.BuyNewSlot(this);
    }
    
    public override void Init(ItemSO itemData)
    {
        Debug.Log($"Init for player {Owner.ClientId}, data = {itemData}, slotItem = {this}");
        _data = (SlotSO)itemData;
        /*itemId.Value = _data.Id;*/
    }
    
    public SlotSO Data => _data;
}
  