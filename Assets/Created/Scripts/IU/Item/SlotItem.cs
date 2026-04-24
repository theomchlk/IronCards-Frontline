using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : AItem
{
    private bool _isFree = true;
    [SerializeField] private SlotSO data;

    public override string Id => "slot";
    public override int Cost(PurchaseContext context) => context.playerState.slotCost.Value;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;

    [Server]
    public override bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerWallet.CanAfford(Cost(context)))
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

    [Server]
    public override void Purchase(PurchaseContext context)
    {
        context.playerWallet.RemoveMoney(Cost(context));
        context.playerState.NewCostItemByMultiplier(context.playerState.slotCost,data.costMultiplier);
        context.playerState.nbSlots.Value++;
        context.playerState.nbFreeSlots.Value++;
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }

}
  