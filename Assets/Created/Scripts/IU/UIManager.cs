using System;
using FishNet;
using NUnit.Framework.Internal;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static event Action<PanelBehavior> OnActivatePanel;
    public event Action OnSetUIsItems;

    private PanelBehavior _lastPanel;

    [SerializeField] private PanelBehavior defaultPanel;
    public MoneyUI moneyUI;
    
    public ShopItemUI shopItemUI;
    public UISlotShop uiSlotShop;
    public UICardShop uiCardShop;
    public UIMillShop uiMillShop;
    public LocalPlayerUIBinder localPlayerUIBinder;
    public PlayerState ps;

    public void Bind(PlayerState ps)
    {
        this.ps = ps;
        uiCardShop.Bind(ps);
        localPlayerUIBinder.Bind(ps);
    }
    
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

    public void SetUIsItems()
    {
        OnSetUIsItems?.Invoke();
    }

    public void SetUpUI()
    {
        uiCardShop.Setup(this);
    }
    
    
}

