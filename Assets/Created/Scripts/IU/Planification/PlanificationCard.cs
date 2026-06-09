using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanificationCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Image du contour (rouge = sans cible, vert = avec cible). Cartes alliées uniquement.")]
    [SerializeField] private Image outline;

    private PlanificationManager _manager;
    private Localisation _loc;
    private int _ownerPlayerId;
    private bool _isAlly;

    public Localisation Loc => _loc;
    public int OwnerPlayerId => _ownerPlayerId;
    public bool IsAlly => _isAlly;
    public RectTransform Rect => (RectTransform)transform;

    public void Init(PlanificationManager manager, Localisation loc, int ownerPlayerId, bool isAlly)
    {
        _manager = manager;
        _loc = loc;
        _ownerPlayerId = ownerPlayerId;
        _isAlly = isAlly;
    }

    public void SetOutline(bool hasTarget, Color noTargetColor, Color hasTargetColor)
    {
        if (outline != null) outline.color = hasTarget ? hasTargetColor : noTargetColor;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!_isAlly) return;
        _manager.BeginTargeting(this, e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        if (!_isAlly) return;
        _manager.UpdateTargeting(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!_isAlly) return;
        _manager.EndTargeting(this, e);
    }
}
