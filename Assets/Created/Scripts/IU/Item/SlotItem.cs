using System;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : ASpawnableItem, IItem
{
    private bool _isFree = true;
    private SlotSO _data;
    
    public int Cost(PurchaseContext context, ItemSO itemData) => context.playerState.slotCost.Value;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;
    

    [Server]
    public bool CanBePurchased(PurchaseContext context, ItemSO itemData)
    {
        if (itemData is not SlotSO data)
        {
            Debug.LogWarning("WARNING: itemData is not a SlotSO");
            return false;
        }
        if (!context.playerState.CanAfford(Cost(context, data)))
        {
            Debug.Log("Not enough money to afford");
            return false;
        }
        if (data.nbSlotMax <= context.playerState.slotCost.Value)
        {
            Debug.Log("Max slots reached");
            return false;
        }
        return true;


    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            _data = (SlotSO)DataBaseItem.Instance.GetDataItem(ItemId);
        }
    }



    [Server]
    public void Purchase(PurchaseContext context, ItemSO itemData)
    {
        if (itemData is not SlotSO data) return;
        context.playerState.RemoveMoney(Cost(context,data));
        context.playerState.NewCostItemByMultiplier(context.playerState.slotCost,data.costMultiplier);
        context.playerState.IncreaseNbItem(context.playerState.nbSlots);
        context.playerState.nbFreeSlots.Value++;
        
        SpawnItem(Owner, data);
    }
    
    [Server]
    public override void SpawnItem(NetworkConnection conn, ItemSO itemData)
    {
        var nob = Instantiate(itemData.goItem).GetComponent<SlotItem>();
        nob.SetItemId(itemData.Id);
        InstanceFinder.ServerManager.Spawn(nob.gameObject, conn);
        TargetSpawnItem(conn,nob);
    }

    [TargetRpc]
    public void TargetSpawnItem(NetworkConnection conn, SlotItem nob)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiSlotShop.BuyNewSlot(nob);
    }

    public void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }

    public string GetIdentifier()
    {
        return _data.Id;
    }
    
    public SlotSO GetData() => _data;
    

    
}
  