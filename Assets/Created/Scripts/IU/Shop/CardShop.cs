using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using System.Collections.Generic;

public class CardShop : NetworkBehaviour, IPurchasableItem
{
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private List<CardsSO> cards;

    public void TryBuyItem(NetworkConnection conn)
    {
        throw new System.NotImplementedException();
    }
}
