using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class ShopItem : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            ServerInitUIsItems();
        }
    }
    
    [ServerRpc] 
    public void BuyItemServerRpc(string id, NetworkConnection conn)
    {
        var itemData = DataBaseItem.Instance.GetDataItem(id);
        var item = itemData.aItem;
        
        //Ici context est un objet temporaire rataché à aucun objet. Il permet juste de récuperer des références au 
        //contexte du joueur -> voir Context Object Pattern
        var context = PurchaseContext.FromPlayer(conn.FirstObject);
        
        

        if (item == null)
        {
            Debug.Log("This item doesn't exist in the database.");
            return;
        }

        if (!item.CanBePurchased(context)) return;

        item.Purchase(context);
        
        TargetBuySucceeded(conn, id);
    }
    

    [TargetRpc]
    private void BuyFailed(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
    }

    [TargetRpc]
    private void TargetBuySucceeded(NetworkConnection conn, string id)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.OnBuyItemSucceeded(id);
    }

    [ServerRpc]
    private void ServerInitUIsItems()
    {
        TargetInitUIsItems(Owner);
    }

    [TargetRpc]
    private void TargetInitUIsItems(NetworkConnection conn)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.SetUIsItems();
    }

   
    /*[TargetRpc]
    private void TargetInitSlotShop(NetworkConnection target, int nbSlots, int slotsPrice)
    {
        uiSlotShop.InitSlot(nbSlots, slotsPrice);
    }
    */
}
