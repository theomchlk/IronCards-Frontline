using System.Collections.Generic;
using UnityEngine;

public class ShopItemUI : MonoBehaviour, IReceiver
{
    [SerializeField] private GameObject shutter;
    

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
        throw new System.NotImplementedException();
    }

    public void DragRejected()
    {
        throw new System.NotImplementedException();
    }

    public List<ReceiverType> AllowedTransition()
    {
        throw new System.NotImplementedException();
    }

    public ReceiverType Type()
    {
        throw new System.NotImplementedException();
    }

    public void CardBeingSwap(CardDragHandler newCard, CardDragHandler oldCard)
    {
        throw new System.NotImplementedException();
    }
}
