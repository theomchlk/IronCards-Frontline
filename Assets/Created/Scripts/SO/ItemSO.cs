using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    public abstract string Id { get; }
    public int cost;
    public GameObject goItem;
}
