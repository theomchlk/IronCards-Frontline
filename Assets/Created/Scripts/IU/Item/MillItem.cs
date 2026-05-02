using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class MillItem : IItem
{
    private MillSO _data;
    
    
    public void SetData(MillSO data) => _data = data;
    
    public int Cost(PurchaseContext context, ItemSO itemData) => context.playerState.millCost.Value;    

    public bool CanBePurchased(PurchaseContext context, ItemSO itemData)
    {
        return context.playerState.CanAfford(Cost(context, itemData));
    }

    public void Purchase(PurchaseContext context, ItemSO itemData)
    {
        if (itemData is not MillSO data)
        {
            Debug.LogWarning("itemData is not a MillSO");
            return;
        }
        context.playerState.RemoveMoney(Cost(context, data));
        context.playerState.NewCostItemByMultiplier(context.playerState.millCost,data.costMultiplier);
        context.playerState.IncreaseNbItem(context.playerState.nbMills);
        
    }

    public void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public string GetIdentifier()
    {
        return _data.Id;
    }
    
}
