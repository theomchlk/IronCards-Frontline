using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Object;

public class UISlotShop : MonoBehaviour
{
    /*[SerializeField] private SlotShop slotShop;*/
    /*[SerializeField] private GameObject slotPrefab;*/
    [SerializeField] private Transform slotsHand;
    [SerializeField] private TMP_Text slotPriceText;
    private List<SlotUI> _slots = new List<SlotUI>();

    /*private void Awake()
    {
        var netObj = GetComponentInParent<NetworkObject>();

        if (netObj != null && netObj.IsOwner)
        {
            _slots = new List<SlotItem>();  
            ResetHand();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }*/
    
    /*public void OnEnable()
    {
        UIManager.Instance.OnSetUIsItems += SetUI;
    }

    public void OnDisable()
    {
        UIManager.Instance.OnSetUIsItems -= SetUI;
    }*/

    public void SetUI(int slotCost)
    {
        ChangeSlotPriceText(slotCost);
    }

    
    public SlotUI GetSlotFree()
    {
        return _slots.FirstOrDefault(slot => slot.IsFree());
    }
    
    public int GetSlotCount() => _slots.Count;
    
    public void BuyNewSlot(SlotItem slot)
    {
        AddNewSlot(slot);
    }

    private void AddNewSlot(SlotItem slot)
    {
        var slotPrefab = slot.Data.goItemUI;
        var slotUi = Instantiate(slotPrefab, slotsHand).GetComponent<SlotUI>();
        _slots.Add(slotUi);
        slotUi.Bind(slot);
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

    /*public void InitSlot(int nbSlots, int slotPrice)
    {
        ChangeSlotPriceText(slotPrice);
        for (int i = 0; i < nbSlots; i++)
        {
            AddNewSlot();
        }
    }*/

    private void ResetHand()
    {
        foreach (Transform child in slotsHand.transform)
            Destroy(child.gameObject);
    }

    public void SetSlotAsUsed(SlotUI slot)
    {
        slot.ChangeFreeState();
    }
}
