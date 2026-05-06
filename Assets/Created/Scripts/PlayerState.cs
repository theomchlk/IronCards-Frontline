using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet;
using Unity.VisualScripting;

public class PlayerState : NetworkBehaviour
{
    /*public static PlayerState Local;*/
    [SerializeField] private PlayerSO playerConfig;
    [SerializeField] private UIManager uiManager;
    
    public UIManager UIManager => uiManager;
        //Partie
    public readonly SyncVar<int> hp = new ();
    public readonly SyncVar<int> money = new ();
    
    //Slot
    public readonly SyncVar<int> nbSlots = new ();
    public readonly SyncVar<int> slotCost = new();
    public readonly SyncVar<int> nbFreeSlots = new();
    //Cards
    public readonly SyncDictionary<string, int> cardsOwned = new();
    //Mill
    public readonly SyncVar<int> nbMills = new();
    public readonly SyncVar<int> millCost = new();
    
    private List<SlotItem> _slotItems = new();
    private int _slotsReadyCount = 0;




    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            SetUiChangeMoney();
            SetUiChangeMill();
            SetUiChangeSlot();
        }
    }

    private void SetPlayerConfig()
    {
        hp.Value = playerConfig.hpByDefault;
        money.Value = playerConfig.moneyByDefault;
    }
    
    private void OnDestroy()
    {
        if (IsOwner)
        {
            DestroyMoney();
            DestroyMill();
            DestroySlot();
        }
    }

    private void DestroyMoney()
    {
        money.OnChange -= OnMoneyChanged;
    }

    private void DestroyMill()
    {
        nbMills.OnChange -= OnMillCostOrNbChanged;
        millCost.OnChange -= OnMillCostOrNbChanged;
    }

    private void DestroySlot()
    {
        slotCost.OnChange -= OnSlotCostChanged;
    }

    private void SetUiChangeMoney()
    {
        money.OnChange += OnMoneyChanged;   
        uiManager.moneyUI.ChangeMoneyText(money.Value);
    }

    private void SetUiChangeMill()
    {
        nbMills.OnChange += OnMillCostOrNbChanged;
        millCost.OnChange += OnMillCostOrNbChanged;
        uiManager.uiMillShop.SetUI(nbMills.Value, millCost.Value);
    }

    private void SetUiChangeSlot()
    {
        nbSlots.OnChange += OnSlotCostChanged;
        uiManager.uiSlotShop.SetUI(slotCost.Value);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerRegistry.Register(Owner, this);
        SetPlayerConfig();
        InitItemsFromDatabase();   // ← tout l'init est ici, côté serveur
    
        /*var itemSo = DataBaseItem.Instance.GetDataItem("slot");
        if (itemSo is SlotSO slotSo)
        {
            nbSlots.Value = slotSo.nbSlotByDefault;
            slotCost.Value = slotSo.cost;
            InitDefaultSlots(slotSo ,slotSo.nbSlotByDefault);
        }
        itemSo = DataBaseItem.Instance.GetDataItem("mill");
        if (itemSo is MillSO millSo)
        {
            nbMills.Value = millSo.nbMillsByDefault;
            millCost.Value = millSo.cost;
        }*/
    }

    [Server]
    private void InitItemsFromDatabase()
    {
        var slotSo = DataBaseItem.Instance.GetDataItem("slot") as SlotSO;
        if (slotSo != null)
        {
            nbSlots.Value  = slotSo.nbSlotByDefault;
            slotCost.Value = slotSo.cost;
            InitDefaultSlots(slotSo, slotSo.nbSlotByDefault);
        }

        var millSo = DataBaseItem.Instance.GetDataItem("mill") as MillSO;
        if (millSo != null)
        {
            nbMills.Value  = millSo.nbMillsByDefault;
            millCost.Value = millSo.cost;
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        foreach (var slot in _slotItems)
        {
            if (slot != null && slot.IsSpawned /*&& slot.IsOwnerPlayerState(this)*/)
                InstanceFinder.ServerManager.Despawn(slot.gameObject);
        }

        _slotItems.Clear();

        PlayerRegistry.Unregister(Owner);
    }

    
    [Server]
    private void InitDefaultSlots(SlotSO data, int count)
    {
        _slotsReadyCount = 0;

        for (int i = 0; i < count; i++)
        {
            var slot = Instantiate(data.goItem).GetComponent<SlotItem>();
            InstanceFinder.ServerManager.Spawn(slot.gameObject);
            slot.Init(data);
            slot.SetOwnerPlayerState(this);   // ← lie le slot à ce PlayerState
            _slotItems.Add(slot);
        }

        nbFreeSlots.Value = count;
    }
    
    /*private void AddSlots(NetworkConnection conn)
    {
        Debug.Log("Conn " + conn);
        Debug.Log(slotItems.Count);
        var ui = UIManager.uiSlotShop;
        foreach (SlotItem slot in slotItems)
        {
            Debug.Log($"AddSlots + OwnerFirstObejct {Owner.FirstObject}");
            slot.TargetSpawnItem(Owner,slot.Data.Id);
            /*ui.BuyNewSlot(slot);#1#
        }
        
        
    }*/
    
    [Server]
    public void OnSlotClientReady(SlotItem slot, NetworkConnection conn)
    {
        _slotsReadyCount++;

        Debug.Log($"[Server] Slot prêt {_slotsReadyCount}/{_slotItems.Count}");

        if (_slotsReadyCount < _slotItems.Count) return;

        // Tous les slots sont confirmés côté client → on envoie les TargetRpc
        Debug.Log("[Server] Tous les slots prêts, envoi des TargetRpc");
        foreach (var s in _slotItems)
            s.TargetSpawnItem(Owner, s.Data.Id);
    }

    [Server]
    public void IncreaseNbItem(SyncVar<int> item) => item.Value++;

    
    [Server]
    public void NewCostItemByMultiplier(SyncVar<int> itemCost ,float multiplier)
    {
        itemCost.Value = (int) Math.Ceiling(itemCost.Value * multiplier);
    }

    [Server]
    public bool HaveFreeSlot()
    {
        return nbFreeSlots.Value > 0;
    }

    [Server]
    public void DecrementFreeSlot()
    {
        nbFreeSlots.Value--;
    }
    
    [Server]
    public bool CanAfford(int price) => money.Value >= price ;
    
    [Server]
    public void RemoveMoney(int amount)
    {
        money.Value -= amount;
    }

    [Server]
    public void AddMoney(int amount)
    {
        money.Value += amount;
    }
    
    private void OnMoneyChanged(int previous, int next, bool asServer)
    {
        Debug.Log("Money changed");
        if (!IsOwner) return;
        uiManager.moneyUI.ChangeMoneyText(next);
    }
    
    private void OnMillCostOrNbChanged(int previous, int next, bool asServer)
    {
        Debug.Log("Mill changed");
        if (!IsOwner) return;
        uiManager.uiMillShop.SetUI(nbMills.Value, millCost.Value);
    }

    private void OnSlotCostChanged(int previous, int next, bool asServer)
    {
        Debug.Log("Slot changed");
        if (!IsOwner) return;
        uiManager.uiSlotShop.SetUI(next);
    }
    

    
}
