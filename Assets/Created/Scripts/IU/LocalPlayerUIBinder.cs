using System;
using System.Linq;
using UnityEngine;
using FishNet;

public class LocalPlayerUIBinder : MonoBehaviour
{
    private PlayerState _ps;

    private void Start()
    {
        _ps = PlayerRegistry.GetPlayerState(InstanceFinder.ClientManager.Connection);
        Debug.Log(InstanceFinder.ClientManager.Connection);

        
        BindMoney();
        BindMills();
        BindSlots();
    }

    private void BindMoney()
    {
        Debug.Log($"Bind money ps = {_ps}");
        _ps.money.OnChange += OnMoneyChanged;
        OnMoneyChanged(_ps.money.Value, _ps.money.Value, false);
    }

    private void OnMoneyChanged(int previous, int next, bool asServer)
    {
        UIManager.Instance.moneyUI.ChangeMoneyText(next);
    }

    private void BindMills()
    {
        _ps.nbMills.OnChange += OnMillChanged;
        _ps.millCost.OnChange += OnMillChanged;

        OnMillChanged(0, 0, false);
    }

    private void OnMillChanged(int previous, int next, bool asServer)
    {
        UIManager.Instance.uiMillShop.SetUI(
            _ps.nbMills.Value,
            _ps.millCost.Value
        );
    }

    private void BindSlots()
    {
        _ps.slotCost.OnChange += OnSlotChanged;
        OnSlotChanged(0, 0, false);
    }

    private void OnSlotChanged(int previous, int next, bool asServer)
    {
        UIManager.Instance.uiSlotShop.SetUI(next);
    }
    
    private void OnDestroy()
    {
        DestroyMoney();
        DestroyMill();
        DestroySlot();
    }
    
    private void DestroyMoney()
    {
        _ps.money.OnChange -= OnMoneyChanged;
    }

    private void DestroyMill()
    {
        _ps.nbMills.OnChange -= OnMillChanged;
        _ps.millCost.OnChange -= OnMillChanged;
    }

    private void DestroySlot()
    {
        _ps.slotCost.OnChange -= OnSlotChanged;
    }
}
