using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private string _cardId;
    public void SetCardId(string cardId) => _cardId = cardId;
    public string CardId => _cardId;
    public Transform lastParent;
    private SlotInCamp _lastSlotInCamp = null;
    public SlotInCamp LastSlotInCamp { get => _lastSlotInCamp; set => _lastSlotInCamp = value; }
    private Transform slotInHand;
    
    public Transform SlotInHand { get => slotInHand; set => slotInHand = value; }
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (transform.parent.parent.CompareTag("Hand")) slotInHand = transform.parent;
    }

    
    
    
    public void OnBeginDrag(PointerEventData e)
    {
        Debug.Log($"Start drag {CardId}");
        if (transform.parent.parent.CompareTag("Hand")) slotInHand = transform.parent;
        _lastSlotInCamp = transform.parent.GetComponent<SlotInCamp>();
        canvasGroup.blocksRaycasts = false;
        lastParent = transform.parent;
        
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        
    }

    public void OnDrag(PointerEventData e)
    {
         /*rectTransform.anchoredPosition += e.delta / canvas.scaleFactor;*/
         Vector3 pos = e.position;
         transform.position = pos;
    }

    public void OnEndDrag(PointerEventData e)
    {
        canvasGroup.blocksRaycasts = true;
        var receiver = e.pointerEnter;
        if (receiver == null)
        {
            ReturnToLastParent();
            return;
        }
        //Si c'est un slot globabelement
        if (receiver.CompareTag("CardReceiver") )
        {
            SetNewParent(receiver.transform);
            receiver.GetComponent<IReceiver>().CardBeingPut(this);
            return;
        }

        if (receiver.CompareTag("Card"))
        {
            var oldCard = receiver.GetComponent<CardDragHandler>();
            receiver.transform.parent.GetComponent<IReceiver>().CardBeingSwap(this, oldCard);


        }
        SetNewParent(lastParent);
    }
    
    
    public void ReturnToLastParent()
    {
        SetNewParent(lastParent);
    }

    public void SetNewParent(Transform newParent)
    {
        transform.SetParent(newParent);
        transform.position = newParent.position;
        FitToParentSize();
    }

    private void FitToParentSize()
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
    
}
