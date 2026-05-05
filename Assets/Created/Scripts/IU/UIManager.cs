using System;
using NUnit.Framework.Internal;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /*public static UIManager Instance;*/
    public static event Action<PanelBehavior> OnActivatePanel;
    public event Action OnSetUIsItems;

    private PanelBehavior _lastPanel;

    [SerializeField] private PanelBehavior defaultPanel;
    public MoneyUI moneyUI;
    
    public UISlotShop uiSlotShop;
    public UICardShop uiCardShop;
    public UIMillShop uiMillShop;

    
    public void Awake()
    {
        /*Instance = this;*/
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

    public void SetUIsItems()
    {
        OnSetUIsItems?.Invoke();
    }

    public void SetUpUI()
    {
        uiCardShop.Setup(this);
    }
    
}

