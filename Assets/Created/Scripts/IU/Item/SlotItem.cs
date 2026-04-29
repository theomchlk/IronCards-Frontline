using System;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : ASpawnableItem, IItem
{
    private bool _isFree = true;
    private SlotSO _data;
    
    public int Cost(PurchaseContext context) => context.playerState.slotCost.Value;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;
    
    public override void OnStartServer()
    {
        _data = (SlotSO)DataBaseItem.Instance.GetDataItem("slot");
    }

    [Server]
    public bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerState.CanAfford(Cost(context)))
        {
            Debug.Log("Not enough money to afford");
            return false;
        }
        if (_data.nbSlotMax <= context.playerState.slotCost.Value)
        {
            Debug.Log("Max slots reached");
            return false;
        }
        return true;


    }

    /*public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            TestSpawn(Owner);
        }
    }

    private void TestSpawn(NetworkConnection conn)
    {
        //Test
        GameObject go = Instantiate(data.goItem, transform);
        go.name = "truc";
        InstanceFinder.ServerManager.Spawn(go, conn);
    }*/

    [Server]
    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(context));
        context.playerState.NewCostItemByMultiplier(context.playerState.slotCost,_data.costMultiplier);
        context.playerState.nbSlots.Value++;
        context.playerState.nbFreeSlots.Value++;
        
        SpawnItem(Owner);
    }
    
    [Server]
    public override void SpawnItem(NetworkConnection conn)
    {
        var nob = Instantiate(_data.goItem).GetComponent<SlotItem>();
        InstanceFinder.ServerManager.Spawn(nob.gameObject, conn);
        TargetSpawnItem(conn,nob);
    }

    [TargetRpc]
    public void TargetSpawnItem(NetworkConnection conn, SlotItem slotItem)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiSlotShop.BuyNewSlot(slotItem);
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
  