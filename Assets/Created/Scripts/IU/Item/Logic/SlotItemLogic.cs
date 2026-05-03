using UnityEngine;

public class SlotItemLogic : IItem
{
    private SlotSO _data;
    
    public SlotItemLogic(SlotSO data)
    {
        _data = data;
    }
    
    public int Cost(PurchaseContext context) => _data.cost;
    
    
    public bool CanBePurchased(PurchaseContext context)
    {
        if (!context.playerState.CanAfford(Cost(context)))
        {
            Debug.Log("Not enough money to afford");
            return false;
        }
        if (_data.nbSlotMax <= context.playerState.nbSlots.Value)
        {
            Debug.Log("Max slots reached");
            return false;
        }
        return true;
    }
    
    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(context));
        context.playerState.NewCostItemByMultiplier(context.playerState.slotCost,_data.costMultiplier);
        context.playerState.IncreaseNbItem(context.playerState.nbSlots);
        context.playerState.IncreaseNbItem(context.playerState.nbFreeSlots);
        
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

    public SlotSO GetData() => _data;
}
