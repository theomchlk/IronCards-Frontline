using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using FishNet.Object;

public class UISlotShop : MonoBehaviour
{
    public static UISlotShop Local;
    
    [SerializeField] private SlotShop slotShop;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotsHand;
    [SerializeField] private TMP_Text slotPriceText;
    private List<SlotItem> _slots;

    void Awake()
    {
        Local = this;
    }

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
    
    public void OnEnable()
    {
        UIManager.Instance.OnSetUIsItems += SetUI;
    }

    public void OnDisable()
    {
        UIManager.Instance.OnSetUIsItems -= SetUI;
    }

    private void SetUI()
    {
        ResetHand();
        InitSlot(PlayerState.Local.nbSlots.Value, PlayerState.Local.slotCost.Value);
    }

    
    public SlotItem GetSlotFree()
    {
        return _slots.FirstOrDefault(slot => slot.IsFree());
    }
    
    public int GetSlotCount() => _slots.Count;
    
    public void BuyNewSlot(SlotItem slot)
    {
        Debug.Log("Slot purchased !");
        AddNewSlot();
        ChangeSlotPriceText(PlayerState.Local.slotCost.Value);
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
