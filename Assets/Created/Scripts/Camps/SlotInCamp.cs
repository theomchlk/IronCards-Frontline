using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInCamp : MonoBehaviour, IReceiver
{
    private Localisation _loc;
    public Localisation Loc => _loc;
    private PlayerCamp camp;
    public bool canReceiveCard;

    public void SetupAlly(int row, int col, PlayerCamp camp)
    {
        Debug.Log($"SetUpAlly({row}, {col})");
        _loc = new Localisation(row, col);
        this.camp = camp;
        canReceiveCard = true;
    }

    public void SetupEnnemy(int row, int col)
    {
        _loc = new Localisation(row, col);
        canReceiveCard = false;
    }

    public void CardBeingPut(CardDragHandler card)
    {
        if (!AllowedTransition().Contains(card.lastParent.GetComponent<IReceiver>().Type()))
        {
            card.ReturnToLastParent();
            return;
        }
        if (canReceiveCard)
        {
            card.CurrentSlotInCamp = this;
            Debug.Log($"Card is put and LastSlotInCamp = {card.LastSlotInCamp}");
            Debug.Log($"{card.CardId} is set in {Loc}");
            camp.ServerPutCardOnCamp(card.CardId, Loc, card.LastSlotInCamp?.Loc);
            SetSlotInCamp(card,this);
        }
        else
        {
            DragRejected();
        }
    }

    public void DragRejected()
    {
        if (transform.childCount <= 0)
        {
            Debug.LogWarning("There is no card");
            return;
        }
        
        var dragHandler = transform.GetChild(transform.childCount - 1).GetComponent<CardDragHandler>();
        dragHandler.ReturnToLastParent();
    
    }

    public List<ReceiverType> AllowedTransition() => new()
    {
        ReceiverType.SlotCamp, ReceiverType.SlotHand
    };

    public ReceiverType Type() => ReceiverType.SlotCamp;

    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard)
    {
        Debug.Log($"CardBeingSwap({newCard}, {oldCard})");
        var newCardType = newCard.lastParent.GetComponent<IReceiver>().Type();
        if (!AllowedTransition().Contains(newCardType))
        {
            newCard.ReturnToLastParent();
            return;
        }
        newCard.CurrentSlotInCamp = this;
        if (newCardType == ReceiverType.SlotCamp)
        {
            /*
            var oldSlotInCamp = (oldCard.LastSlotInCamp != null) ? oldCard.LastSlotInCamp : oldCard.transform.parent.GetComponent<SlotInCamp>();
            */
            var oldSlotInCamp = oldCard.CurrentSlotInCamp;

            Debug.Log($"oldSlotCamp = {oldSlotInCamp.Loc.Col},{oldSlotInCamp.Loc.Row} and newSlotCamp = {newCard.LastSlotInCamp.Loc.Col},{newCard.LastSlotInCamp.Loc.Row}");
            camp.ServerPutCardOnCamp(newCard.CardId, oldSlotInCamp.Loc, newCard.LastSlotInCamp?.Loc);
            SetSlotInCamp(oldCard, newCard.LastSlotInCamp);
            SetSlotInCamp(newCard, oldSlotInCamp);
            
            return;
        }

        if (newCardType == ReceiverType.SlotHand)
        {
            camp.ServerRemoveCardFromCampToHand(oldCard.CurrentSlotInCamp.Loc, oldCard.CardId);
            SetSlotInCamp(newCard, oldCard.CurrentSlotInCamp);
            oldCard.LastSlotInCamp = null;
            oldCard.SetNewParent(oldCard.SlotInHand);
            camp.ServerPutCardOnCamp(newCard.CardId, Loc, null);
        }
        
    }

    public void OnClickDebug()
    {
        Debug.Log($"OnClickSlot: Loc(row,col) = ({_loc.Row}, {_loc.Col})");
    }

    private void SetSlotInCamp(CardDragHandler card, SlotInCamp newSlotInCamp)
    {
        card.LastSlotInCamp = newSlotInCamp;
        card.CurrentSlotInCamp = newSlotInCamp;
        card.SetNewParent(card.LastSlotInCamp.transform);
    }
    
    
}
