using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class MillItemLogic : IItem
{
    private MillSO _data;

    public MillItemLogic(MillSO data)
    {
        _data = data;
    }
    
    public int Cost(PurchaseContext context) => context.playerState.millCost.Value;    

    public bool CanBePurchased(PurchaseContext context)
    {
        return context.playerState.CanAfford(Cost(context));
    }

    public void Purchase(PurchaseContext context)
    {
        context.playerState.RemoveMoney(Cost(context));
        context.playerState.NewCostItemByMultiplier(context.playerState.millCost,_data.costMultiplier);
        context.playerState.IncreaseNbItem(context.playerState.nbMills);
    }

    public void Accept(IItemVisitor visitor)
    {
        /*
        visitor.Visit(this);
    */
    }
    
    public string GetIdentifier()
    {
        return _data.Id;
    }
    
}
