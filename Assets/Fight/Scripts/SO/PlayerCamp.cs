using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerCamp", menuName = "Fight/PlayerCamp", order = 1)]
public class PlayerCamp : ScriptableObject
{
    [SerializeField] public int[] campCardsId = Enumerable.Repeat(-1, 35).ToArray();
}
