using UnityEngine;
using System.Collections.Generic;

public class SlotUI : MonoBehaviour, IReceiver
{
    public ReceiverType Type() => ReceiverType.SlotHand;
    private bool _isFree = true;
    private SlotItem _slotItem;

    private CardDragHandler _cardOnSlot;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;

    public List<ReceiverType> AllowedTransition() => new()
    {
        ReceiverType.SlotHand, ReceiverType.SlotCamp
    };

    public void Bind(SlotItem slotItem)
    {
        _slotItem = slotItem;
    }

    public void CardBeingPut(CardDragHandler card)
    {
        if (!AllowedTransition().Contains(card.lastParent.GetComponent<IReceiver>().Type()))
        {
            card.ReturnToLastParent();
            return;
        }
        if (card.LastSlotInCamp) _slotItem.ServerRemoveCardFromCampToHand(card.LastSlotInCamp.Loc, card.CardId);
        SetSlotInHand(card,transform);
        card.LastSlotInCamp = null;
        _cardOnSlot = card;
    }

    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard)
    {
        if (!AllowedTransition().Contains(newCard.lastParent.GetComponent<IReceiver>().Type()))
        {
            newCard.ReturnToLastParent();
            return;
        }
        
        var oldSlot = oldCard.SlotInHand;
        SetSlotInHand( oldCard, newCard.SlotInHand);
        SetSlotInHand(newCard, oldSlot);
        
    }

    public void DragRejected()
    {
        
    }
    
    private void SetSlotInHand(CardDragHandler card,Transform newSlotInHand)
    {
        card.SlotInHand = newSlotInHand;
        card.SetNewParent(card.SlotInHand);
    }
}
