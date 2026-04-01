using Unity.VisualScripting;
using UnityEngine;

public class IconBehavior : MonoBehaviour
{
    public PanelBehavior panel;

    public void OnButtonClick()
    {
        panel.ActivePanel(panel.GetSide());
    }
    
    
}
