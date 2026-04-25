using Created.Scripts.IU.Shop;
using UnityEngine;

public class UICardShop : MonoBehaviour
{
    public static UICardShop Local;

    [SerializeField] private Transform cardStallsLocation;
    [SerializeField] private Transform cardsLocation;

    public void Awake()
    {
        Local = this;
    }

    public void BuyNewCard(CardItem card)
    {
        var slotFree = UISlotShop.Local.GetSlotFree();
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
        ResetCardShop();
        foreach (var cs in CardStallTable.Instance.cardStallsOnTable)
        {
            SetCardStall(cs);
        }
    }

    private void ResetCardShop()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    private void SetCardStall(CardStallSO cs)
    {
        Instantiate(cs.cardStallPrefab, cardStallsLocation);
        foreach (var cSO in cs.cardsSo)
        {
            Instantiate(cSO.cardPrefab, cardsLocation);
        }
    }
    

}
