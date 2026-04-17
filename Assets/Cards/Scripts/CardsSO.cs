using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;


[CreateAssetMenu(fileName = "CardsSO", menuName = "CardsSO")]
public class CardsSO : ScriptableObject
{
    public string cardID;
    //DATA
    public string cardName;
    public int cost;
    public float dps;
    public float health;
    /*public float armor;*/
    public float range;
    public float movementSpeed;
    public int nbSoldiers;
    public GameObject soldierPrefab;
    /*public List<Skills> skills;*/
    //UI
    public Sprite cardSprite;

}
