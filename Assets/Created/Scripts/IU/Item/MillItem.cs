using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class MillItem : IItem
{
    [SerializeField] private MillSO data;
    public int Cost(PurchaseContext context) => context.playerState.millCost.Value;

    [Server]
    public bool CanBePurchased(PurchaseContext context)
    {
        return context.playerState.CanAfford(context.playerState.millCost.Value);
    }

    [Server]
    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(context.playerState.millCost.Value);
        context.playerState.NewCostItemByMultiplier(context.playerState.millCost,data.costMultiplier);
        context.playerState.nbMills.Value++;
    }

    public void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public string GetIdentifier()
    {
        return data.Id;
    }
    
}
