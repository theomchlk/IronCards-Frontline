using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class UISlotShop : MonoBehaviour
{
    public static UISlotShop Instance;
    [SerializeField] private SlotShop slotShop;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotsHand;
    [SerializeField] private TMP_Text slotPriceText;
    private List<SlotItem> _slots;

    private void Awake()
    {
        Instance = this;
        ResetHand();
    }
    
    public SlotItem GetSlotFree()
    {
        return _slots.FirstOrDefault(slot => slot.IsFree());
    }
    
    public int GetSlotCount() => _slots.Count;
    
    public void OnBuySlotSucceeded(string msg, int newSlotPrice)
    {
        Debug.Log(msg);
        AddNewSlot();
        ChangeSlotPriceText(newSlotPrice);
    }

    private void AddNewSlot()
    {
        SlotItem slot = Instantiate(slotPrefab, slotsHand.transform).GetComponent<SlotItem>();
        _slots.Add(slot);
        AnimationNewSlot();
    }

    private void AnimationNewSlot()
    {
        //Mettre ici une animation d'instantiation pour le feel good du genre grandit et fait un bruit 

        return;
    }

    private void ChangeSlotPriceText(int amount)
    {
        slotPriceText.text = amount.ToString();
    }

    public void InitSlot(int nbSlots, int slotPrice)
    {
        ChangeSlotPriceText(slotPrice);
        for (int i = 0; i < nbSlots; i++)
        {
            AddNewSlot();
        }
    }

    private void ResetHand()
    {
        foreach (Transform child in slotsHand.transform)
            Destroy(child.gameObject);
    }

    public void SetSlotAsUsed(SlotItem slot)
    {
        slot.ChangeFreeState();
    }
}
