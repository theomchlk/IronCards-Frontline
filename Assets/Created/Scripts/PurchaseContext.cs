using FishNet.Object;
using UnityEngine;

//Context Object Pattern
public class PurchaseContext 
{
    public PlayerWallet playerWallet;
    public PlayerState playerState;

    public static PurchaseContext FromPlayer(NetworkObject player)
    {
        return new PurchaseContext()
        {
            playerWallet = player.gameObject.GetComponent<PlayerWallet>(),
            playerState = player.gameObject.GetComponent<PlayerState>()
        };
    }
}
