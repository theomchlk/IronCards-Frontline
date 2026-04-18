using System;
using UnityEngine;

public class BuySlotButton : MonoBehaviour
{
    [SerializeField] private ShopItem shopItem;
    
    
    public void OnClickBuySlot()
    {
        shopItem.BuyItem("slot");
    }
    
}
