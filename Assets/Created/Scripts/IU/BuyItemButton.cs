using System;
using FishNet.Object;
using UnityEngine;

public class BuyItemButton : NetworkBehaviour
{
    [SerializeField] private ShopItem shopItem;
    
    
    public void OnClickBuySlot(IItem aItem)
    {
        shopItem.BuyItemServerRpc(aItem.GetIdentifier(), Owner);
    }
    
}
