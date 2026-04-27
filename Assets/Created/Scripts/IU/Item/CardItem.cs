using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : AItem
{
    [SerializeField] private CardsSO data;
    
    public override int Cost(PurchaseContext context) => data.cost;

    [Server]
    public override bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerState.CanAfford(Cost(null))) return false;
        if (!context.playerState.HaveFreeSlot()) return false;
        return true;
    }

    [Server]
    public override void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(null));
        CardCollection.AddCard(context.playerState.cardsOwned,data.Id);
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public override string GetIdentifier()
    {
        return data.Id;
    }
}
