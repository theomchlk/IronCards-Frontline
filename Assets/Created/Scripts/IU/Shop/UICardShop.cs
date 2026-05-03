using Created.Scripts.IU;
using Created.Scripts.IU.Shop;
using UnityEngine;

public class UICardShop : MonoBehaviour
{

    [SerializeField] private Transform cardStallsLocation;
    /*[SerializeField] private GameObject cardStallUI;
    [SerializeField] private GameObject cardContainerUI;*/

    public void BuyNewCard(CardItem card)
    {
        var slotFree = UIManager.Instance.uiSlotShop.GetSlotFree();
        var cardPrefab = card.Data.goItemUI;
        var cardUI= Instantiate(cardPrefab, slotFree.transform).GetComponent<CardUI>();
        cardUI.Bind(card);
        slotFree.ChangeFreeState();
    }

    /*public void OnEnable()
    {
        UIManager.Instance.OnSetUIsItems += SetUI;
    }

    public void OnDisable()
    {
        UIManager.Instance.OnSetUIsItems -= SetUI;
    }*/

    /*private void SetUI()
    {
        /*ResetCardShop();#1#
        
        foreach (var csID in CardStallTable.Instance.cardStallsOnTable)
        {
            var cs = CardStallDataBase.Instance.GetCardStall(csID);
            SetCardStall(cs);
        }
    }*/

    private void ResetCardShop()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    /*private void SetCardStall(CardStallSO cs)
    {
        var csUI = Instantiate(cs.cardStallPrefab, cardStallsLocation).GetComponent<CardStallUI>();
        foreach (var cSO in cs.cardsSo)
        {
            var csc = Instantiate(cardStallContainer, csUI.transform).GetComponent<CardStallContainer>();
            SetCardInSlot(cSO, csc.SlotLocation);
        }
    }*/
    
    private void SetCardInSlot(CardsSO cSO, Transform location)
    {
        /*Instantiate(cSO.goItemUI, location);*/
    }


    public void AddNewStall(CardStallSO csSO)
    {
        var csGO = Instantiate(csSO.cardStallPrefab, cardStallsLocation);
        var csUI = csGO.GetComponent<CardStallUI>();
        csUI.SetCardStall(csSO);
        foreach (var cSO in csSO.cardsSo)
        {
            csUI.SetNewCardContainer(csSO.cardContainer, cSO);
        }
    }
    

}
