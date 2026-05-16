using UnityEngine;
using UnityEngine.UI;
public class IconBehavior : MonoBehaviour
{
    public PanelBehavior panel;
    [SerializeField] private Image image;
    [SerializeField] private Color colorWhenSelected = new Color(0x8A,0x8A,0x8A);
    [SerializeField] private Color defaultColor = Color.white;

    void Awake()
    {
        image = GetComponent<Image>();
    }
    
    public void ActivePanel()
    {
        /*UIManager.Instance.ShowPanel(panel);*/
    }

    private void OnEnable()
    {
        UIManager.OnActivatePanel += HandleOnActivatePanel;
    }

    private void OnDisable()
    {
        UIManager.OnActivatePanel -= HandleOnActivatePanel;
    }

    private void HandleOnActivatePanel(PanelBehavior shown)
    {
        bool isSelected = (shown == panel);
        SetSelectedStateButton(isSelected);
    }

    private void SetSelectedStateButton(bool isSelected)
    {
        image.color = isSelected ? colorWhenSelected : defaultColor;
    }
}
