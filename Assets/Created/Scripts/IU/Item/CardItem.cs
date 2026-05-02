using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class CardItem : ASpawnableItem, IItem
{
    private CardsSO _data;
    
    
    public int Cost(PurchaseContext context, ItemSO itemData) => itemData.cost;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            _data = (CardsSO)DataBaseItem.Instance.GetDataItem(ItemId);
        }
    }
    
    [Server]
    public bool CanBePurchased(PurchaseContext context, ItemSO itemData)
    {
        if (itemData is not CardsSO data)
        {
            Debug.LogWarning("WARNING: itemData is not CardsSO");
            return false;
        }
        if (!context.playerState.CanAfford(Cost(null, data))) return false;
        if (!context.playerState.HaveFreeSlot()) return false;
        return true;
    }

    [Server]
    public void Purchase(PurchaseContext context, ItemSO itemData)
    {
        if (itemData is not CardsSO data) return;
        context.playerState.RemoveMoney(Cost(null, data));
        CardCollection.AddCard(context.playerState.cardsOwned,data.Id);
        
        SpawnItem(Owner, data);
    }

    public void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public string GetIdentifier()
    {
        return _data.Id;
    }


    public override void SpawnItem(NetworkConnection conn, ItemSO itemData)
    {
        var nob = Instantiate(itemData.goItem).GetComponent<CardItem>();
        nob.SetItemId(itemData.Id);
        InstanceFinder.ServerManager.Spawn(nob.gameObject, conn);
        TargetSpawnItem(conn,nob);
    }
    
    [TargetRpc]
    public void TargetSpawnItem(NetworkConnection conn, CardItem nob)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.uiCardShop.BuyNewCard(nob);
    }
    
    public CardsSO GetData() => _data;
}
