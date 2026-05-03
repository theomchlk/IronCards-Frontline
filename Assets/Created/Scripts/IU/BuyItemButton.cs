using System;
using UnityEngine;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private ShopItem _shopItem;
    [SerializeField] private ItemSO _itemData;


    public void OnClickBuyItem()
    {
        _shopItem.BuyItemServerRpc(_itemData.Id);
    }

    public void SetButton(ItemSO itemData, ShopItem shopItem)
    {
        _shopItem = shopItem;
        _itemData = itemData;
    }
}