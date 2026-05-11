using UnityEngine;

[CreateAssetMenu(fileName = "TmpPlayerCamp", menuName = "Fight/TmpPlayerCamp", order = 1)]
public class TmpPlayerCamp : ScriptableObject
{
    [SerializeField] public CardsSO[] camp = new CardsSO[35];
}
