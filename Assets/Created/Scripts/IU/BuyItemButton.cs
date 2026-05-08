using System;
using FishNet.Managing.Client;
using UnityEngine;
using FishNet;

public class BuyItemButton : MonoBehaviour
{
    [SerializeField] private ShopItem _shopItem;
    [SerializeField] private ItemSO _itemData;

    public void Start()
    {
        _shopItem = PlayerRegistry.GetPlayerState(InstanceFinder.ClientManager.Connection).GetComponent<ShopItem>();
    }
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