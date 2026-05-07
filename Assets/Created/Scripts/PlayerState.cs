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
    private readonly SyncVar<bool> isLobbyLeader = new ();
    private readonly SyncVar<int> hp = new ();
    private readonly SyncVar<int> money = new ();
    private int _moneyPerMills;
    
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
    


    private void Awake()
    {
        hp.Value = playerConfig.hpByDefault;
        money.Value = playerConfig.moneyByDefault;
        _moneyPerMills = playerConfig.moneyPerMills;
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



    [Server]
    public void InitItemsFromDatabase()
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
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerRegistry.Register(Owner, this);
        GameStateController.Instance.CurrentState.OnPlayerEnter(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameStateController.Instance.CurrentState.OnPlayerExit(this);
        /*foreach (var slot in _slotItems)
        {
            if (slot != null && slot.IsSpawned /*&& slot.IsOwnerPlayerState(this)#1#)
                InstanceFinder.ServerManager.Despawn(slot.gameObject);
        }

        _slotItems.Clear();*/

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

    [Server]
    public void SetNewMoney()
    {
        var newMoney = nbMills.Value * _moneyPerMills; 
        AddMoney(newMoney);
    }
    
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
    private void AddMoney(int amount)
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

    [Server]
    public void SetLobbyLeader()
    {
        isLobbyLeader.Value = true;
    }

    [Server]
    public bool IsLobbyLeader() => isLobbyLeader.Value;


}
