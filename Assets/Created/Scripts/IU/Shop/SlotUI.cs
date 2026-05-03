using UnityEngine;

public class SlotUI : MonoBehaviour
{
    private bool _isFree = true;
    private SlotItem _slotItem;
    
    public bool IsFree() => _isFree;
    public void ChangeFreeState() => _isFree = !_isFree;

    public void Bind(SlotItem slotItem)
    {
        _slotItem = slotItem;
    }
}
