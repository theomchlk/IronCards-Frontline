using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerCamp", menuName = "Fight/PlayerCamp", order = 1)]
public class PlayerCamp : ScriptableObject
{
    [SerializeField] public string[] campCardsId = Enumerable.Repeat(string.Empty, 35).ToArray();
}
