using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public abstract class AItem : NetworkBehaviour
{
    [SerializeField] protected PlayerWallet playerWallet;
    public abstract string Id { get; }
    public abstract int Cost { get; }
    
    public abstract bool CanBePurchased();
    
    public abstract void Purchase();

}
