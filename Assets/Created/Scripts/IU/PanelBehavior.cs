using System;
using UnityEngine;

public class PanelBehavior : MonoBehaviour
{
    public enum Side { Left, Right }
    [SerializeField] private Side side;

    public Side GetSide() => side;
    public void SetSide(Side side) => this.side = side;
    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
