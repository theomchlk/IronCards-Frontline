using System;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private readonly SyncVar<int> _money = new SyncVar<int>(OnChangeMoney);

    public bool CanAfford(int price) => _money.Value >= price ;
    
    [Server]
    public void RemoveMoney(int amount)
    {
        _money.Value -= amount;
    }

    [Server]
    public void AddMoney(int amount)
    {
        _money.Value += amount;
    }


    
}
