using System;
using NUnit.Framework.Internal;
using UnityEngine;

public class UIManager : MonoBehaviour, IItemVisitor
{
    public static UIManager Instance;
    public static event Action<PanelBehavior> OnActivatePanel;
    public event Action OnSetUIsItems;

    private PanelBehavior _lastPanel;

    [SerializeField] private PanelBehavior defaultPanel;
    
    [SerializeField] private UISlotShop uiSlotShop;
    [SerializeField] private UICardShop uiCardShop;
    [SerializeField] private UIMillShop uiMillShop;

    
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
    
    
    //Visitor Pattern
    public void OnBuyItemSucceeded(string id)
    {
        var item = DataBaseItem.Instance.GetDataItem(id).aItem;
        item.Accept(this);
    }

    public void Visit(SlotItem item)
    {
        uiSlotShop.BuyNewSlot(item);
    }

    public void Visit(CardItem item)
    {
        uiCardShop.BuyNewCard(item);
    }

    public void Visit(MillItem item)
    {
        uiMillShop.BuyNewMill(item);
    }
}

