using UnityEngine;

public class IconBehavior : MonoBehaviour
{
    public PanelBehavior panel;

    public void ActivePanel()
    {
        UIManager.Instance.ShowPanel(panel);
    }
    
    
}
