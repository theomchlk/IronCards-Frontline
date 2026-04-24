using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerShop : NetworkBehaviour
{
    //Pas sûr que je l'utilise encore...
    
    /*public void BuyItem(AItem item)
    {
        BuyItemServerRpc((NetworkBehaviour) item);
    }

    [ServerRpc] 
    private void BuyItemServerRpc(NetworkBehaviour item, NetworkConnection conn = null)
    {
        if (item is AItem)
        {
            item.TryBuyItem(conn);
        }
    }*/
}
