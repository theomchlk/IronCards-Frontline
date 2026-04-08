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

    public void OnConnectedToServer()
    {
        playerShop = FindFirstObjectByType<PlayerShop>();
        slotShop = FindFirstObjectByType<SlotShop>();
        
    }
}
