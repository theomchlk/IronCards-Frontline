using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerShop : NetworkBehaviour
{
    public void BuyItem(IPurchasableItem purchasableItem)
    {
        BuyItemServerRpc((NetworkBehaviour) purchasableItem);
    }

    [ServerRpc] 
    private void BuyItemServerRpc(NetworkBehaviour item, NetworkConnection conn = null)
    {
        if (item is IPurchasableItem purchasableItem)
        {
            purchasableItem.TryBuyItem(conn);
        }
    }
}
