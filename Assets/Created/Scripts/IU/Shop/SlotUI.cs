using UnityEngine;

public class SlotUI : MonoBehaviour
{
    private SlotItem _slotItem;

    public void Bind(SlotItem slotItem)
    {
        _slotItem = slotItem;
    }
}
