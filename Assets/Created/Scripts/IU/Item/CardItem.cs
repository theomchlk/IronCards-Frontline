using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : ASpawnableItem
{
    private CardsSO _data;
    
    [TargetRpc]
    public override void TargetSpawnItem(NetworkConnection conn, string itemId)
    {
        Init(DataBaseItem.Instance.GetDataItem(itemId));
        var uiManager = conn.FirstObject.GetComponent<PlayerState>().UIManager;
        uiManager.uiCardShop.BuyNewCard(this, uiManager.uiSlotShop);
    }
    
    public override void Init(ItemSO itemData) => _data = (CardsSO)itemData;
    
    public CardsSO Data => _data;
}
