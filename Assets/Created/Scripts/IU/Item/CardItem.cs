using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : ASpawnableItem
{
    private CardsSO _data;
    
    [TargetRpc]
    public override void TargetSpawnItem(NetworkConnection conn)
    {
       
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiCardShop.BuyNewCard(this);
    }
    
    public override void Init(ItemSO itemData) => _data = (CardsSO)itemData;
    
    public CardsSO Data => _data;
}
