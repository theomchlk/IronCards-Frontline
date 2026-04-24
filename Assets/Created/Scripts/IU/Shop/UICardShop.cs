using UnityEngine;

public class UICardShop : MonoBehaviour
{
    
    [SerializeField] private

    public void OnEnable()
    {
        UIManager.Instance.OnSetUIsItems += SetUI;
    }

    public void OnDisable()
    {
        UIManager.Instance.OnSetUIsItems -= SetUI;
    }

    private void SetUI()
    {
        ChangeMillCostText(PlayerState.Local.millCost.Value);
        ChangeNbMillText(PlayerState.Local.nbMills.Value);
    }

    private void ResetCardShop()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
    

}
