using System;
using UnityEngine;

public class BuySlotButton : MonoBehaviour
{
    [SerializeField] PlayerShop playerShop;
    [SerializeField] SlotShop slotShop;
    
    public void OnClickBuySlot()
    {
        playerShop.BuyItem(slotShop);
    }
    
}
