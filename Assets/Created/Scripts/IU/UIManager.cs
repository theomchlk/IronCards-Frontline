using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private PanelBehavior _lastLeftPanel;
    private PanelBehavior _lastRightPanel;

    [SerializeField] private PanelBehavior defaultLeftPanel;
    [SerializeField] private PanelBehavior defaultRightPanel;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (defaultLeftPanel != null) defaultLeftPanel.gameObject.SetActive(true);
        if (defaultRightPanel != null) defaultRightPanel.gameObject.SetActive(true);
    }

    public void ShowPanel(PanelBehavior panel)
    {
        if (panel.GetSide() == PanelBehavior.Side.Left)
        {
            if (_lastLeftPanel != null) _lastLeftPanel.Hide();
            _lastLeftPanel = panel;
        }
        else if (panel.GetSide() == PanelBehavior.Side.Right)
        {
            if (_lastRightPanel != null) _lastRightPanel.Hide();
            _lastRightPanel = panel;
        }
        panel.Show();
    }
}

