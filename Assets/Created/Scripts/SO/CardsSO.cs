using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;



[CreateAssetMenu(fileName = "CardsSO", menuName = "CardsSO")]
public class CardsSO : ItemSO
{
    public string id;
    //DATA
    public string cardName;
    public float damage;
    public float attackSpeed;
    public float health;
    /*public float armor;*/
    public float range;
    public float movementSpeed;
    public int nbSoldiers;
    /*public GameObject soldierPrefab;*/
    /*public List<Skills> skills;*/
    public Sprite sprite;
    public AudioClip sound;
    public CombatActionSO combatAction;
    public GameObject soldierPrefab;
    [SerializeField, Range(0f, 1f)] public float soundVolume = 0.05f;
    [SerializeField, Range(0f, 1f)] public float armorProtection = 0f;
    public AudioClip protectionSound;

    public GameObject LobbySoldierPrefab;

    public override string Id => id;

    public override IItem CreateItemInstance()
    {
        return new CardItemLogic(this);
    }
}
