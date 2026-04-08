using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class SlotShop : NetworkBehaviour, IPurchasableItem
{
    private int _cost;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private int nbSlotByDefault;
    [SerializeField] private int nbSlotMax;
    private int _nbSlot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotHand;

    [Server]
    public void TryBuyItem(NetworkConnection conn)
    {
        if (nbSlotMax <= nbSlotByDefault)
        {
            BuyFailed(conn,"You have reach the maximum number of slots");  
            return;
        }
        if (!playerWallet.CanAfford(_cost))
        {
            BuyFailed(conn, "You do not have enough money to buy this item.");
            return;
        }
        playerWallet.RemoveMoney(_cost);
        _nbSlot++;
        BuySucceeded(conn, "Item purchased !");
    }

    [TargetRpc]
    public void BuyFailed(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
    }

    [TargetRpc]
    public void BuySucceeded(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
        AddNewSlot();
    }

    private void AddNewSlot()
    {
        slotHand = Instantiate(slotPrefab, slotHand.transform);
        //Mettre ici une animation d'instantiation pour le feel good du genre grandit et fait un bruit 
    }
        
}
