using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public abstract class AItem : NetworkBehaviour
{
    public abstract int Cost(PurchaseContext context);
    
    public abstract bool CanBePurchased(PurchaseContext context);
    
    public abstract void Purchase(PurchaseContext context);
    
    public abstract void Accept(IItemVisitor visitor);

    public abstract string GetIdentifier();
    

}
