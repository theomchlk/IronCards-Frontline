using UnityEngine;
using TMPro;

public class UISlotShop : MonoBehaviour
{
    [SerializeField] private SlotShop slotShop;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotsHand;
    [SerializeField] private TMP_Text slotPriceText;

    private void Awake()
    {
        ResetHand();
    }
    
    public void OnBuySlotSucceeded(string msg, int newSlotPrice)
    {
        Debug.Log(msg);
        AddNewSlot();
        ChangeSlotPriceText(newSlotPrice);
    }

    private void AddNewSlot()
    {
        Instantiate(slotPrefab, slotsHand.transform);
        AnimationNewSlot();
    }

    private void AnimationNewSlot()
    {
        //Mettre ici une animation d'instantiation pour le feel good du genre grandit et fait un bruit 

        return;
    }

    public void ChangeSlotPriceText(int amount)
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
}
