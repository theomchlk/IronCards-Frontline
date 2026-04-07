using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static event Action<PanelBehavior> OnActivatePanel;

    private PanelBehavior _lastPanel;

    [SerializeField] private PanelBehavior defaultPanel;


    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (defaultPanel == null) return;
        ShowPanel(defaultPanel);
    }

    public void ShowPanel(PanelBehavior panel)
    {
        if (panel == null || panel == _lastPanel) return;
        if (_lastPanel != null) _lastPanel.Hide();
        _lastPanel = panel;
        panel.Show();
        OnActivatePanel?.Invoke(panel);
    }
}

