using FishNet.Object;
using UnityEngine;

//Context Object Pattern
public class PurchaseContext 
{
    public PlayerState playerState;

    public static PurchaseContext FromPlayer(NetworkObject player)
    {
        return new PurchaseContext()
        {
            playerState = player.gameObject.GetComponent<PlayerState>()
        };
    }
}
