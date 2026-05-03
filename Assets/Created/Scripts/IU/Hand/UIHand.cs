using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UIHand : MonoBehaviour
{
    public static UIHand Instance;
    
    public List<SlotItem> slots;

    void Awake()
    {
        Instance = this;
    }

    /*public SlotUI GetSlotFree()
    {
        return slots.FirstOrDefault(slot => slot.IsFree());
    }*/
    
    public int GetSlotCount() => slots.Count;
}
