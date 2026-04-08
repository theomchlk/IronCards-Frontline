using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private int money;

    public bool CanAfford(int price) => money >= price ;
    
    [Server]
    public void RemoveMoney(int amount)
    {
        money -= amount;
    }

    [Server]
    public void AddMoney(int amount)
    {
        money += amount;
    }
}
