using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorPersonnalisation : MonoBehaviour
{
    [SerializeField] private Slider sliderR;
    [SerializeField] private Image sliderRColor;
    
    [SerializeField] private Slider sliderG;
    [SerializeField] private Image sliderGColor;
    
    [SerializeField] private Slider sliderB;
    [SerializeField] private Image sliderBColor;
    
    [SerializeField] private Image colorImage;
    [SerializeField] private TMP_Text squareInMenu;

    [SerializeField] private GameObject panel;

    public float minVal = 0.2f, maxVal = 0.8f;

    public void Awake()
    {
        LocalPlayerPrefs.MinColorValue = minVal;
        LocalPlayerPrefs.MaxColorValue = maxVal;
       
        var color = LocalPlayerPrefs.PlayerColor;
        SetSliderColor(color);
        SetImageColor(color);
        panel.SetActive(false);
    }
    
    public void ChangerSliderRColor()
    {
        sliderRColor.color = new Color(sliderR.value, 0, 0);
    }
    
    public void ChangerSliderGColor()
    {
        sliderGColor.color = new Color(0, sliderG.value, 0);
    }
    
    public void ChangerSliderBColor()
    {
        sliderBColor.color = new Color(0, 0, sliderB.value);
    }

    public void ChangeColor()
    {
        SetImageColor(sliderR.value, sliderG.value, sliderB.value);
    }

    public void ChangeColorByRandom()
    {
        var color = LocalPlayerPrefs.GetRandomColor();
        SetSliderColor(color);
        SetImageColor(color);
    }

    private void SetSliderColor(Color color)
    {
        sliderR.value = color.r;
        sliderG.value = color.g;
        sliderB.value = color.b;
    }
    
    private void SetImageColor(Color color)
    {
        colorImage.color = color;
        squareInMenu.color = color;
        LocalPlayerPrefs.PlayerColor = color;
    }
    
    private void SetImageColor(float r, float g, float b)
    {
        var color = new Color(r, g, b);
        SetImageColor(color);
    }
    
        
}
