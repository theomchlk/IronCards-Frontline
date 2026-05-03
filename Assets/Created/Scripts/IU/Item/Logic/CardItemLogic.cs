using UnityEngine;

public class CardItemLogic : IItem
{
    private CardsSO _data;
    
    public CardItemLogic(CardsSO data)
    {
        _data = data;
    }
    
    public int Cost(PurchaseContext context) => _data.cost;
    
    
    public bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerState.CanAfford(Cost(null))) return false;
        if (!context.playerState.HaveFreeSlot()) return false;
        return true;
    }
    
    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(null));
        CardCollection.AddCard(context.playerState.cardsOwned, _data.Id);
        context.playerState.DecrementFreeSlot();
        context.shopItem.SpawnItemForPlayer(context.connection,_data);
    }

    public void Accept(IItemVisitor visitor)
    {
        /*visitor.Visit(this);*/
    }

    public string GetIdentifier()
    {
        return _data.Id;
    }

    public CardsSO GetData() => _data;
}

