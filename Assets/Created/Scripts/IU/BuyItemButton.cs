using System;
using UnityEngine;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private ShopItem shopItem;
    private IItem _item;
    
    public void SetItem(IItem item) => _item = item;
    
    public void OnClickBuyItem()
    {
        shopItem.BuyItemServerRpc(_item.GetIdentifier());
    }
    
}
