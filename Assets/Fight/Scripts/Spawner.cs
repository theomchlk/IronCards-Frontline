using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnHeight = 4f;

    public void SpawnSoldiers(CardsSO[] cards, CardUI[] cardUIs, int playerId)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null)
            {
                GameObject soldierPrefab = cards[i].soldierPrefab;
                RectTransform cardRect = cardUIs[i].GetComponent<RectTransform>();
                for (int j = 0; j < cards[i].nbSoldiers; j++)
                {                    
                    float angle = j * Mathf.PI * 2 / cards[i].nbSoldiers;
                    float radius = 2f; // Distance par rapport au centre
                    Vector3 circleOffset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                    Vector3 spawnPosition = cardRect.position + new Vector3(0, spawnHeight, 0) + circleOffset;
                    SpawnSoldier(soldierPrefab, spawnPosition, playerId);
                }
            }
        }
    }

    public void SpawnSoldier(GameObject soldierPrefab, Vector3 spawnPosition, int playerId)
    {
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        soldier.GetComponent<Soldier>().ownerId = playerId;
    }
}