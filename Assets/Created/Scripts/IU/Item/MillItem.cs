using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class MillItem : AItem
{
    [Server]
    public override bool CanBePurchased()
    {
        return true;
    }
}
