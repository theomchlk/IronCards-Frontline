using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private GameObject shutter;
    

    public void OpenShuttereUI()
    {
        shutter.SetActive(false);
    }

    public void CloseShuttereUI()
    {
        shutter.SetActive(true);
    }
}
