using System;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class SlotItem : AItem
{
    private bool _isFree;
    [SerializeField] private SlotSO data;
    private int _slotCost;
    private int _nbSlots;
    public override string Id => "slot";
    public override int Cost => _slotCost;

    private void Awake()
    {
        _slotCost = data.cost;
        _nbSlots = data.nbSlotByDefault
    }
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;

    [Server]
    public override bool CanBePurchased()
    {
        if (!playerWallet.CanAfford(Cost))
        {
            Debug.Log("Not enough money to afford");
            return false;
        }
        if (data.nbSlotMax <= _nbSlots)
        {
            Debug.Log("Max slots reached");
            return false;
        }
        return true;


    }

    [Server]
    public override void Purchase()
    {
        UISlotShop ui = UISlotShop.Instance;
        _slotCost = NewSlotPrice(_slotCost, data.costMultiplier);
        _nbSlots++;
        ui.OnBuySlotSucceeded("Slot Purchased !", _slotCost);
    }
    
    private int NewSlotPrice(int price, float multiplier)
    {
        return (int) Math.Ceiling(price * multiplier);
    }
}
  