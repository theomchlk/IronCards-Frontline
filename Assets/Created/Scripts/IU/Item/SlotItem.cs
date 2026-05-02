using System;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : ASpawnableItem
{
    private bool _isFree = true;
    private SlotSO _data;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;
    
    [TargetRpc]
    public override void TargetSpawnItem(NetworkConnection conn)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiSlotShop.BuyNewSlot(this);
    }
    
    public override void Init(ItemSO itemData) => _data = (SlotSO)itemData;
    
    public SlotSO Data => _data;
}
  