using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;



[CreateAssetMenu(fileName = "CardsSO", menuName = "CardsSO")]
public class CardsSO : ItemSO
{
    //DATA
    public string cardName;
    public float dps;
    public float health;
    /*public float armor;*/
    public float range;
    public float movementSpeed;
    public int nbSoldiers;
    public GameObject soldierPrefab;
    public GameObject cardPrefab;
    /*public List<Skills> skills;*/
}
