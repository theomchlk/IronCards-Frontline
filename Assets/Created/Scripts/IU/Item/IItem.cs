using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public interface IItem
{
    public int Cost(PurchaseContext context);
    
    public bool CanBePurchased(PurchaseContext context);
    
    public void Purchase(PurchaseContext context);
    
    public void Accept(IItemVisitor visitor);

    public string GetIdentifier();

}
