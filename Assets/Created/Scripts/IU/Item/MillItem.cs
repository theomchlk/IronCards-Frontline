using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class MillItem : AItem
{
    [SerializeField] private MillSO data;

    public override string Id => "mill";
    public override int Cost(PurchaseContext context) => context.playerState.millCost.Value;

    [Server]
    public override bool CanBePurchased(PurchaseContext context)
    {
        return context.playerWallet.CanAfford(context.playerState.millCost.Value);
    }

    [Server]
    public override void Purchase(PurchaseContext context)
    {
        context.playerWallet.RemoveMoney(context.playerState.millCost.Value);
        context.playerState.NewCostItemByMultiplier(context.playerState.millCost,data.costMultiplier);
        context.playerState.nbMills.Value++;
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
