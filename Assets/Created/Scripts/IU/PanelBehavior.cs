using System;
using UnityEngine;

public class PanelBehavior : MonoBehaviour
{ 
    public void Awake()
    {
        Hide();
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    
}
