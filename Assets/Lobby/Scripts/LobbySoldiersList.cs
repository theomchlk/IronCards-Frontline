using UnityEngine;

[CreateAssetMenu(fileName = "LobbySoldiersList", menuName = "Lobby/LobbySoldiersList")]
public class LobbySoldiersList : ScriptableObject
{
    public CardsSO[] soldiers; 

    public CardsSO GetRandomCardSO()
    {
        if (soldiers.Length == 0)
        {
            Debug.LogWarning("No soldiers available in the list.");
            return null;
        }
        int randomIndex = Random.Range(0, soldiers.Length);
        return soldiers[randomIndex];
    }
}
