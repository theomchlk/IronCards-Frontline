using UnityEngine;

public class PanelBehavior : MonoBehaviour
{
    public static PanelBehavior LastLeftPanel;
    public static PanelBehavior LastRightPanel;
    
    public enum Side { Left, Right }
    [SerializeField] private Side side;

    public Side GetSide()
    {
        return side;
    }

    public void ActivePanel(Side side)
    {
        if (side == Side.Left)
        {
            if (LastLeftPanel != null) LastLeftPanel.gameObject.SetActive(false);
            LastLeftPanel = this;
        }
        else if (side == Side.Right)
        {
            if (LastRightPanel != null) LastRightPanel.gameObject.SetActive(false);
            LastRightPanel = this;
        }
        this.gameObject.SetActive(true);
    }
    
}
