using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet;

public class PlayerState : NetworkBehaviour
{
    /*public static PlayerState Local;*/
    [SerializeField] private PlayerSO playerConfig;
    //Partie
    public readonly SyncVar<int> hp = new SyncVar<int>();
    public readonly SyncVar<int> money = new SyncVar<int>();
    
    //Slot
    public readonly SyncVar<int> nbSlots = new SyncVar<int>();
    public readonly SyncVar<int> slotCost = new SyncVar<int>();
    public readonly SyncVar<int> nbFreeSlots = new SyncVar<int>();
    //Cards
    public readonly SyncDictionary<string, int> cardsOwned = new();
    //Mill
    public readonly SyncVar<int> nbMills = new SyncVar<int>();
    public readonly SyncVar<int> millCost = new SyncVar<int>();


    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            SetPlayerConfig();
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
        UIManager.Instance.moneyUI.ChangeMoneyText(money.Value);
    }

    private void SetUiChangeMill()
    {
        nbMills.OnChange += OnMillCostOrNbChanged;
        millCost.OnChange += OnMillCostOrNbChanged;
        UIManager.Instance.uiMillShop.SetUI(nbMills.Value, millCost.Value);
    }

    private void SetUiChangeSlot()
    {
        nbSlots.OnChange += OnSlotCostChanged;
        UIManager.Instance.uiSlotShop.SetUI(slotCost.Value);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        var itemSo = DataBaseItem.Instance.GetDataItem("slot");
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
        }
    }
    
    [Server]
    private void InitDefaultSlots(SlotSO data, int defaultValue)
    {
        for (int i = 0; i < defaultValue; i++)
        {
            var slot = Instantiate(data.goItem).GetComponent<SlotItem>();
            slot.Init(data);
            InstanceFinder.ServerManager.Spawn(slot.gameObject, Owner);
            slot.TargetSpawnItem(Owner);
        }

        nbFreeSlots.Value = defaultValue;
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
        UIManager.Instance.moneyUI.ChangeMoneyText(next);
    }
    
    private void OnMillCostOrNbChanged(int previous, int next, bool asServer)
    {
        Debug.Log("Mill changed");
        if (!IsOwner) return;
        UIManager.Instance.uiMillShop.SetUI(nbMills.Value, millCost.Value);
    }

    private void OnSlotCostChanged(int previous, int next, bool asServer)
    {
        Debug.Log("Slot changed");
        if (!IsOwner) return;
        UIManager.Instance.uiSlotShop.SetUI(next);
    }
    

    
}
