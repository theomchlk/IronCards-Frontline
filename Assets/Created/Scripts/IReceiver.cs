using System.Collections.Generic;
using UnityEngine;
public interface IReceiver
{
    public void CardBeingPut(CardDragHandler card);
    public void DragRejected();
    public List<ReceiverType> AllowedTransition();
    public ReceiverType Type();
    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard);
}
