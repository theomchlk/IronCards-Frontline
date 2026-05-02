using FishNet.Object;
using FishNet.Connection;
using UnityEditor;
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
    public void BuyItemServerRpc(string id, NetworkConnection conn = null)
    {
        var itemData = DataBaseItem.Instance.GetDataItem(id);
        
        /* Ici on recupere le script IItem des objets via la prefab goItem contenu dans ItemSO. Ce prefab va être
         instantié par l'item si il est un ASpawnableObject. Cependant certains scripts ne sont pas des ASpawnableItem
         mais doivent quand même avoir un goItem pour récuperer la référence de IItem, ce qui peut être déroutant.
         On pourrait faire un proxy pour chaque ASpawnableItem qui ne serait que des IItem, et feraient spawn les 
         ASpawnableObject, mais pas soucis de temps et de simplicité on restera comme cela
         */
        var item = itemData.goItem.GetComponent<IItem>();
        
        //Ici context est un objet temporaire rataché à aucun objet. Il permet juste de récuperer des références au 
        //contexte du joueur -> voir Context Object Pattern
        var context = PurchaseContext.FromPlayer(conn.FirstObject);
        
        

        if (item == null)
        {
            Debug.Log("This item doesn't exist in the database.");
            return;
        }

        if (!item.CanBePurchased(context, itemData)) return;

        item.Purchase(context, itemData);
        
        /*TargetBuySucceeded(conn, id);*/
    }
    

    [TargetRpc]
    private void BuyFailed(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
    }

    /*[TargetRpc]
    private void TargetBuySucceeded(NetworkConnection conn, string id)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.OnBuyItemSucceeded(id);
    }*/

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
