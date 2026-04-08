using FishNet.Connection;

public interface IPurchasableItem
{
    public void TryBuyItem(NetworkConnection conn);
    public void BuyFailed(NetworkConnection conn, string msg);
    public void BuySucceeded(NetworkConnection conn, string msg);
}
