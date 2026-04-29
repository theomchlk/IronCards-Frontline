using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : ASpawnableItem, IItem
{
    [SerializeField] private CardsSO data;
    
    public int Cost(PurchaseContext context) => data.cost;

    [Server]
    public bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerState.CanAfford(Cost(null))) return false;
        if (!context.playerState.HaveFreeSlot()) return false;
        return true;
    }

    [Server]
    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(null));
        CardCollection.AddCard(context.playerState.cardsOwned,data.Id);
    }

    public void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public string GetIdentifier()
    {
        return data.Id;
    }

    public override void SpawnItemLocally(Transform spawnLocation)
    {
        data.goItemUI
    }
}
