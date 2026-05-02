using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public interface IItem
{
    public int Cost(PurchaseContext context, ItemSO itemData);
    
    public bool CanBePurchased(PurchaseContext context, ItemSO itemData);
    
    public void Purchase(PurchaseContext context, ItemSO itemData);
    
    public void Accept(IItemVisitor visitor);

    public string GetIdentifier();

}
