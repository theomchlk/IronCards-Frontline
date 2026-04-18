using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class ShopItem : NetworkBehaviour
{
    [SerializeField] private PlayerWallet playerWallet;

    
    public void BuyItem(string id)
    {
        BuyItemServerRpc(id);
    }
    
    [ServerRpc] 
    private void BuyItemServerRpc(string id, NetworkConnection conn = null)
    {
        var itemData = DataBaseItem.Instance.GetItem(id);
        var item = itemData.aItem;
        
        if (item == null) return;
        if (item.CanBePurchased())
        {
            /*int currentSlotPrice = GetCurrentSlotPrice();*/
            //On enleve au joueur le prix de l'item
            playerWallet.RemoveMoney(itemData.cost);
            //On ajoute un slot apres l'achat
            _nbSlot++;
            //On augmente le prix du slot (a voir comment on fait pour l'instant)
            slotPrice = NewSlotPrice(slotPrice, slotPriceMultiplier);
            //On effectue les changements en local
            TargetBuySucceeded(conn, slotPrice);
        }
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

    private int NewSlotPrice(int price, float multiplier)
    {
        return (int) Math.Ceiling(price * multiplier);
    }
}
