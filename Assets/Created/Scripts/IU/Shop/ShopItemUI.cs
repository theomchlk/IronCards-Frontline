using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IReceiver, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject shutter;
    [SerializeField] private GameObject sellingPanel;
    [SerializeField] private TMP_Text sellingText;
    

    public void OpenShuttereUI()
    {
        shutter.SetActive(false);
    }

    public void CloseShuttereUI()
    {
        shutter.SetActive(true);
    }

    public void CardBeingPut(CardDragHandler card)
    {
        if (!AllowedTransition().Contains(card.lastParent.GetComponent<IReceiver>().Type()))
        {
            card.ReturnToLastParent();
            return;
        }

        var cardItem = card.GetComponent<CardUI>().CardItem;
        card.CleanAll();
        if (cardItem) ShopItem.Instance.ServerRefundCard(cardItem);
    }

    public void DragRejected()
    {
        
    }

    public List<ReceiverType> AllowedTransition() => new()
    {
        ReceiverType.SlotHand
    };

    public ReceiverType Type() => ReceiverType.Shop;

    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard)
    {
        
    }
    

    public void OnPointerEnter(PointerEventData e)
    {
        if (e.dragging)
        {
            sellingPanel.SetActive(true);

        }
    }

    public void OnPointerExit(PointerEventData e)
    {
        sellingPanel.SetActive(false);
    }
}
