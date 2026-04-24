using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : AItem
{
    [SerializeField] private CardsSO data;

    public override string Id => data.id;
    public override int Cost(PurchaseContext context) => data.cost;

    [Server]
    public override bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerWallet.CanAfford(Cost(null))) return false;
        if (!context.playerState.HaveFreeSlot()) return false;
        return true;
    }

    [Server]
    public override void Purchase(PurchaseContext context)
    {
        context.playerWallet.RemoveMoney(Cost(null));
        CardCollection.AddCard(context.playerState.cardsOwned,Id);
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
