using UnityEngine;
using System.Collections.Generic;

public class SlotUI : MonoBehaviour, IReceiver
{
    public ReceiverType Type() => ReceiverType.SlotHand;
    private bool _isFree = true;
    private SlotItem _slotItem;

    public CardDragHandler _cardOnSlot;
    
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
        if (card.LastSlotInCamp) _slotItem.PlayerState.Camp.ServerRemoveCardFromCampToHand(card.LastSlotInCamp.Loc, card.CardId);
        if (_cardOnSlot)
        {
            _cardOnSlot.SlotInHand = card.SlotInHand;
            card.SlotInHand.GetComponent<SlotUI>()._cardOnSlot = _cardOnSlot;
            card.SlotInHand = this.transform;
            _cardOnSlot = card;
            card.SetNewParent(transform);
            return;
        }
        SetSlotInHand(card,transform);
        card.LastSlotInCamp = null;
        _cardOnSlot = card;
    }

    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard)
    {
        Debug.Log("CardBeingSwap");
        var newCardType = newCard.lastParent.GetComponent<IReceiver>().Type();
        if (!AllowedTransition().Contains(newCardType))
        {
            Debug.Log("Swap not permit");
            newCard.ReturnToLastParent();
            return;
        }

        if (newCardType == ReceiverType.SlotCamp)
        {
            _slotItem.PlayerState.Camp.ServerRemoveCardFromCampToHand(newCard.LastSlotInCamp.Loc, newCard.CardId);
            SetSlotInHand(newCard, newCard.SlotInHand);
            SetSlotInHand(oldCard, oldCard.SlotInHand);

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
        var oldSlotUI = card.SlotInHand.GetComponent<SlotUI>();
        oldSlotUI?.ChangeFreeState();
        if (oldSlotUI) oldSlotUI._cardOnSlot = null;
        card.SlotInHand = newSlotInHand;
        var newSlotUI = newSlotInHand.GetComponent<SlotUI>();
        newSlotUI?.ChangeFreeState();
        if (newSlotUI) newSlotUI._cardOnSlot = card;
        card.SetNewParent(newSlotInHand);
    }
    
}
