using System;
using UnityEngine;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private ShopItem shopItem;
    [SerializeField] private ItemSO itemData;

    
    public void OnClickBuyItem()
    {
        shopItem.BuyItemServerRpc(itemData.Id);
    }
    
}
