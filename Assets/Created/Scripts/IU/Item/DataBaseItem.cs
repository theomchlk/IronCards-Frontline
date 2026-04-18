using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DataBaseItem : MonoBehaviour
{
    public static DataBaseItem Instance;
    
    [SerializeField] private List<ItemSO> itemSO;

    private Dictionary<string, ItemSO> _dict;

    void Awake()
    {
        Instance = this;
        _dict = itemSO.ToDictionary(i => i.aItem.Id, i => i);
    }

    public ItemSO GetItem(string id) => _dict[id];
}
