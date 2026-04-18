using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : AItem
{
    [Server]
    public override bool CanBePurchase()
    {
        throw new System.NotImplementedException();
    }
}
