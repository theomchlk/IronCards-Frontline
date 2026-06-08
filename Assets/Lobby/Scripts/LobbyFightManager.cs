using UnityEngine;

public class LobbyFightManager : MonoBehaviour
{

    public static LobbyFightManager Instance { get; private set; }
    [SerializeField] private LobbySoldiersList lobbySoldiersList;
    [SerializeField] private MeshRenderer fightGroundMeshRenderer;
    [SerializeField] private GameObject camera;
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private float spawnFrequency = 1f;
    private float spawnTimer = 0f;
    private Vector2 leftFightGroundCoordsX;
    private Vector2 leftFightGroundCoordsZ;
    private Vector2 rightFightGroundCoordsX;
    private Vector2 rightFightGroundCoordsZ;
    private int soldierIdCounter = 0;
    private bool isActive = true;
    private int lastId = 0;

    
    void Awake()
    {
        leftFightGroundCoordsX = new Vector2(fightGroundMeshRenderer.bounds.min.x, fightGroundMeshRenderer.bounds.center.x);
        leftFightGroundCoordsZ = new Vector2(fightGroundMeshRenderer.bounds.min.z, fightGroundMeshRenderer.bounds.max.z);
        rightFightGroundCoordsX = new Vector2(fightGroundMeshRenderer.bounds.center.x, fightGroundMeshRenderer.bounds.max.x);
        rightFightGroundCoordsZ = new Vector2(fightGroundMeshRenderer.bounds.min.z, fightGroundMeshRenderer.bounds.max.z);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (!isActive) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnFrequency)
        {
            LobbySoldier soldier0 = SpawnSoldier(leftFightGroundCoordsX, leftFightGroundCoordsZ, soldierIdCounter++);
            LobbySoldier soldier1 = SpawnSoldier(rightFightGroundCoordsX, rightFightGroundCoordsZ, soldierIdCounter++);
            soldier0.SetTarget(soldier1);
            soldier1.SetTarget(soldier0);
            spawnTimer = 0f;
        }
    }


    private LobbySoldier SpawnSoldier(Vector2 spawnAreaX, Vector2 spawnAreaZ, int id)
    {
        float randomX = Random.Range(spawnAreaX.x, spawnAreaX.y);
        float randomZ = Random.Range(spawnAreaZ.x, spawnAreaZ.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);
        CardsSO card = lobbySoldiersList.GetRandomCardSO();
        GameObject newSoldier = Instantiate(card.LobbySoldierPrefab, spawnPosition, Quaternion.identity);
        LobbySoldier soldierComponent = newSoldier.GetComponent<LobbySoldier>();
        soldierComponent.bind(card);
        soldierComponent.SetOwnerId(lastId++);
        soldierComponent.SetPlayerColor(new Color(Random.value, Random.value, Random.value));
        newSoldier.transform.SetParent(transform);
        return soldierComponent;
    }

    public static void SetActive(bool active)
    {
        Instance.isActive = active;
        Instance.fightGroundMeshRenderer.gameObject.SetActive(active);
        Instance.camera.SetActive(active);
        if (!active)
        {
            while (LobbySoldier.allSoldiers.Count > 0)
            {
                LobbySoldier soldier = LobbySoldier.allSoldiers[0];
                LobbySoldier.allSoldiers.RemoveAt(0);
                if (soldier != null)
                    Destroy(soldier.gameObject);
            }
        }
    }

    public void DesactiveCamera()
    {
        camera.SetActive(false);
    }

}
