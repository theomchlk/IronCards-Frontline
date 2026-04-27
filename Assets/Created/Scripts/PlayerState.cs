using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;

public class PlayerState : NetworkBehaviour
{
    public static PlayerState Local;
    //Partie
    /*public SyncVar<int> hp;*/
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
            Local = this;
            money.OnChange += OnMoneyChanged;
            UIManager.Instance.moneyUI.ChangeMoneyText(money.Value);
        }
    }

    public void OnDestroy()
    {
        if (IsOwner)
        {
            money.OnChange -= OnMoneyChanged;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        var itemSo = DataBaseItem.Instance.GetDataItem("slot");
        if (itemSo is SlotSO slotSo)
        {
            nbSlots.Value = slotSo.nbSlotByDefault;
            slotCost.Value = slotSo.cost;
        }
        itemSo = DataBaseItem.Instance.GetDataItem("mill");
        if (itemSo is MillSO millSo)
        {
            nbMills.Value = millSo.nbMillsByDefault;
            millCost.Value = millSo.cost;
        }
    }

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
        if (!IsOwner) return;
        UIManager.Instance.moneyUI.ChangeMoneyText(next);
    }


    
}
