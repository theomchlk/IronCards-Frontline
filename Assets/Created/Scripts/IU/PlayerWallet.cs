using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private int money;
    [SerializeField] private TMP_Text moneyText;    //L'argent du joueur

    void Awake()
    {
        moneyText.text = money.ToString();
    }

    public bool CanAfford(int price) => money >= price ;
    
    [Server]
    public void RemoveMoney(int amount)
    {
        money -= amount;
        TargetUpdateMoney(Owner, money);
    }

    [Server]
    public void AddMoney(int amount)
    {
        money += amount;
        TargetUpdateMoney(Owner, money);
    }

    [TargetRpc]
    private void TargetUpdateMoney(NetworkConnection target, int newAmount)
    {
        moneyText.text = newAmount.ToString();
        Debug.Log("new money: " + money);
    }
}
