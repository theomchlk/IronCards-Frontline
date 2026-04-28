using Created.Scripts.IU;
using Created.Scripts.IU.Shop;
using UnityEngine;

public class UICardShop : MonoBehaviour
{

    [SerializeField] private Transform cardStallsLocation;
    [SerializeField] private GameObject cardStallContainer;

    public void BuyNewCard(CardItem card)
    {
        var slotFree = UIManager.Instance.uiSlotShop.GetSlotFree();
        Instantiate(card.gameObject, slotFree.transform);
        slotFree.ChangeFreeState();
    }

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
        /*ResetCardShop();*/
        
        foreach (var csID in CardStallTable.Instance.cardStallsOnTable)
        {
            var cs = CardStallDataBase.Instance.GetCardStall(csID);
            /*SetCardStall(cs);*/
        }
    }

    private void ResetCardShop()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    private void SetCardStall(CardStallSO cs)
    {
        var csUI = Instantiate(cs.cardStallPrefab, cardStallsLocation).GetComponent<CardStallUI>();
        foreach (var cSO in cs.cardsSo)
        {
            var csc = Instantiate(cardStallContainer, csUI.transform).GetComponent<CardStallContainer>();
            /*SetCardInSlot(cSO, csc.SlotLocation);*/
        }
    }
    
    private void SetCardInSlot(CardsSO cSO, Transform location)
    {
        Instantiate(cSO.goItem, location);
    }
    

}
