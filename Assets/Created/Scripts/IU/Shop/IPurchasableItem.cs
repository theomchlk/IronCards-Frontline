using FishNet.Connection;

public interface IPurchasableItem
{
    //A supprimer
    public void TryBuyItem(NetworkConnection conn);

}
