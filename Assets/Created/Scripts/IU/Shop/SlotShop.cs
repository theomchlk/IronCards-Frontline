using FishNet.Connection;
using FishNet.Object;
using UnityEngine;


public class SlotShop : NetworkBehaviour, IPurchasableItem
{
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private int nbSlotByDefault;
    [SerializeField] private int nbSlotMax;
    private int _nbSlot;
    [SerializeField] private UISlotShop uiSlotShop;
    [SerializeField] private int slotPrice;
    [SerializeField] private int slotPriceMultiplier;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _nbSlot = nbSlotByDefault;
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
            ServerInitSlotShop();
    }

    [ServerRpc]
    private void ServerInitSlotShop()
    {
        TargetInitSlotShop(Owner, _nbSlot, slotPrice);
    }
    
    [Server]
    public void TryBuyItem(NetworkConnection conn)
    {
        if (_nbSlot >= nbSlotMax)
        {
            BuyFailed(conn,"You have reach the maximum number of slots");  
            return;
        }
        if (!playerWallet.CanAfford(slotPrice))
        {
            BuyFailed(conn, "You do not have enough money to buy this item.");
            return;
        }
        
        /*int currentSlotPrice = GetCurrentSlotPrice();*/
        //On enleve au joueur le prix d'un slot
        playerWallet.RemoveMoney(slotPrice);
        //On ajoute un slot apres l'achat
        _nbSlot++;
        //On augmente le prix du slot (a voir comment on fait pour l'instant)
        slotPrice += slotPrice * slotPriceMultiplier;
        //On effectue les changements en local
        TargetBuySucceeded(conn, slotPrice);
    }

    [TargetRpc]
    private void BuyFailed(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
    }

    [TargetRpc]
    private void TargetBuySucceeded(NetworkConnection conn, int newSlotPrice)
    {
        uiSlotShop.OnBuySlotSucceeded("Item purchased !", newSlotPrice);
    }

   
    [TargetRpc]
    private void TargetInitSlotShop(NetworkConnection target, int nbSlots, int slotsPrice)
    {
        uiSlotShop.InitSlot(nbSlots, slotsPrice);
    }
    
    private int GetCurrentSlotPrice() => slotPrice;

}
