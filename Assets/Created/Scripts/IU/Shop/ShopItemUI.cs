using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private GameObject shutter;
    
    public void SetPreparationStateUI()
    {
        OpenShuttereUI();
    }

    private void OpenShuttereUI()
    {
        shutter.SetActive(false);
    }
}
