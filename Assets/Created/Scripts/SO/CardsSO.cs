using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;



[CreateAssetMenu(fileName = "CardsSO", menuName = "CardsSO")]
public class CardsSO : ItemSO
{
    public string id;
    //DATA
    public string cardName;
    public float dps;
    public float health;
    /*public float armor;*/
    public float range;
    public float movementSpeed;
    public int nbSoldiers;
    /*public GameObject soldierPrefab;*/
    /*public List<Skills> skills;*/
    public Sprite sprite;

    public override string Id => id;

    public override IItem CreateItemInstance()
    {
        return new CardItemLogic(this);
    }
}
