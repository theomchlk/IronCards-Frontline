using System;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : ASpawnableItem
{

    private SlotSO _data;
    

    
    [TargetRpc]
    public override void TargetSpawnItem(NetworkConnection conn)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiSlotShop.BuyNewSlot(this);
    }
    
    public override void Init(ItemSO itemData) => _data = (SlotSO)itemData;
    
    public SlotSO Data => _data;
}
  