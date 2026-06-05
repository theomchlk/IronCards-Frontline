using System;
using FishNet.Managing.Client;
using UnityEngine;
using FishNet;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private ShopItem _shopItem;
    [SerializeField] private ItemSO _itemData;
    
    public void OnClickBuyItem()
    {
        if (!_shopItem) _shopItem = ShopItem.Instance;
        _shopItem.BuyItemServerRpc(_itemData.Id);
    }

    public void SetButton(ItemSO itemData, ShopItem shopItem)
    {
        _shopItem = shopItem;
        _itemData = itemData;
    }
}