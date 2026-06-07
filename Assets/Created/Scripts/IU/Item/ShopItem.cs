using FishNet;
using FishNet.Object;
using FishNet.Connection;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class ShopItem : NetworkBehaviour
{
    private bool _isOpen;
    [Range(0,1)]
    [SerializeField] private float priceLoseWhenRefund = 0.7f;

    public float PriceLoseWhenRefund => priceLoseWhenRefund;

    public bool IsOpen { get => _isOpen; set => _isOpen = value; }
    
    public static ShopItem Instance;
    /*public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            ServerInitUIsItems();
        }
    }*/

    void Awake()
    {
        Instance = this;
    }
    
    [ServerRpc(RequireOwnership = false)] 
    public void BuyItemServerRpc(string id, NetworkConnection conn = null)
    {
        if (!IsOpen) return;
        var itemData = DataBaseItem.Instance.GetDataItem(id);
        var item = itemData.CreateItemInstance();
        
        //Ici context est un objet temporaire rataché à aucun objet. Il permet juste de récuperer des références au 
        //contexte du joueur -> voir Context Object Pattern
        var context = PurchaseContext.FromPlayer(conn.FirstObject);
        
        

        if (item == null)
        {
            Debug.Log("This item doesn't exist in the database.");
            return;
        }

        if (!item.CanBePurchased(context)) return;

        item.Purchase(context);
        
        /*TargetBuySucceeded(conn, id);*/
    }
    

    [TargetRpc]
    private void BuyFailed(NetworkConnection conn, string msg)
    {
        Debug.Log(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerRefundCard(NetworkObject card, NetworkConnection conn = null)
    {
        if (!IsOpen) return;
        var cardItem = card.GetComponent<CardItem>();
        if (cardItem == null) return;
        var ps = PlayerRegistry.GetPlayerState(conn.ClientId);
        if (ps.cardsInHand[cardItem.Data.Id] < 1)
        {
            Debug.Log($"Card {cardItem.Data.Id} isn't in hand.");
            return;
        }
        Destroy(cardItem.CardUI.gameObject);
        RefundCard(cardItem.Data.Id, card, cardItem.isBoughtThisRound, ps);
        
    }

    [Server]
    private void RefundCard(string cardId, NetworkObject card, bool isBoughtThisRound, PlayerState ps)
    {
        var cardData = DataBaseItem.Instance.GetDataItem(cardId);
        var moneyBack = (isBoughtThisRound) ? cardData.cost : Mathf.CeilToInt(cardData.cost * PriceLoseWhenRefund);
        ps.AddMoney(moneyBack);
        ps.IncrementFreeSlot();
        card.Despawn();
    }

    /*[TargetRpc]
    private void TargetBuySucceeded(NetworkConnection conn, string id)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.OnBuyItemSucceeded(id);
    }*/

    /*
    [ServerRpc]
    private void ServerInitUIsItems()
    {
        TargetInitUIsItems(Owner);
    }

    [TargetRpc]
    private void TargetInitUIsItems(NetworkConnection conn)
    {
        var uiManager = conn.FirstObject.GetComponent<UIManager>();
        uiManager.SetUIsItems();
    }
    */

   
    /*[TargetRpc]
    private void TargetInitSlotShop(NetworkConnection target, int nbSlots, int slotsPrice)
    {
        uiSlotShop.InitSlot(nbSlots, slotsPrice);
    }
    */

    [Server]
        public void SpawnItemForPlayer(NetworkConnection conn, ItemSO itemData)
        {
            var go = Instantiate(itemData.goItem);
            var nob = go.GetComponent<ASpawnableItem>();
            nob.Init(itemData);
            
            InstanceFinder.ServerManager.Spawn(go/*, conn*/);
            nob.TargetSpawnItem(conn, itemData.Id);
        }
        
        
}
