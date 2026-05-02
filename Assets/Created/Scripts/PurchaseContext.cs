using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

//Context Object Pattern
public class PurchaseContext 
{
    public PlayerState playerState;
    public ShopItem shopItem;
    public NetworkConnection connection;

    public static PurchaseContext FromPlayer(NetworkObject player)
    {
        var ps = player.GetComponent<PlayerState>();
        var shop = player.GetComponent<ShopItem>();

        return new PurchaseContext
        {
            playerState = ps,
            shopItem = shop,
            connection = player.Owner
        };
    }

}
