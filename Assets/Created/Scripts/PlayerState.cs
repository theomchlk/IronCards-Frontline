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
    
    //Slot
    public readonly SyncVar<int> nbSlots;
    public readonly SyncVar<int> slotCost;
    public readonly SyncVar<int> nbFreeSlots;
    //Cards
    public readonly SyncDictionary<string, int> cardsOwned = new();
    //Mill
    public readonly SyncVar<int> nbMills;
    public readonly SyncVar<int> millCost;


    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            Local = this;
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


    
}
