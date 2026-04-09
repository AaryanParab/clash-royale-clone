using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("Troop Prefabs")]
    public GameObject knightPrefab;
    public GameObject archerPrefab;

    [Header("Mana System")]
    public float currentMana = 10f;
    public float maxMana = 20f;
    public float manaRegenRate = 3f;

    [Header("Enemy Settings")]
    public float enemySpawnInterval = 8f;
    public float enemyInitialDelay = 4f;

    private float nextEnemySpawnTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating("RegenerateMana", 1f, 1f);
        nextEnemySpawnTime = Time.time + enemyInitialDelay;
    }

    private void Update()
    {
        if (Time.time >= nextEnemySpawnTime)
        {
            SpawnRandomEnemyTroop();
            nextEnemySpawnTime = Time.time + enemySpawnInterval;
        }
    }

    private void RegenerateMana()
    {
        currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
    }

    // ====================== PLAYER SPAWNING ======================
    public void SpawnKnight()
    {
        if (currentMana < 6f) return;
        currentMana -= 6f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        GameObject troop = Instantiate(knightPrefab, playerSpawnPoint.position + offset, Quaternion.identity);

        SetupTroop(troop, "PlayerTroop", GetEnemyTowerTransform());

        Debug.Log("Player spawned Knight");
    }

    public void SpawnArcher()
    {
        if (currentMana < 3f) return;
        currentMana -= 3f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        GameObject troop = Instantiate(archerPrefab, playerSpawnPoint.position + offset, Quaternion.identity);

        SetupTroop(troop, "PlayerTroop", GetEnemyTowerTransform());

        Debug.Log("Player spawned Archer");
    }

    // ====================== ENEMY SPAWNING ======================
    private void SpawnRandomEnemyTroop()
    {
        bool spawnKnight = Random.value > 0.5f;
        GameObject prefab = spawnKnight ? knightPrefab : archerPrefab;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        Quaternion enemyRotation = Quaternion.Euler(0, 180, 0);

        GameObject troop = Instantiate(prefab, enemySpawnPoint.position + offset, enemyRotation);

        SetupTroop(troop, "EnemyTroop", GetPlayerTowerTransform());

        Debug.Log($"Enemy spawned {(spawnKnight ? "Knight" : "Archer")}");
    }

    // ====================== HELPER METHOD ======================
    private void SetupTroop(GameObject troop, string tag, Transform target)
    {
        if (troop == null) return;

        // Set tag
        troop.tag = tag;

        // Set target after a tiny delay so Awake/Start of TroopAI has finished
        TroopAI ai = troop.GetComponent<TroopAI>();
        if (ai != null)
        {
            StartCoroutine(SetTargetAfterDelay(ai, target));
        }
    }

    private System.Collections.IEnumerator SetTargetAfterDelay(TroopAI ai, Transform target)
    {
        yield return new WaitForSeconds(0.05f);   // Small delay to ensure TroopAI is ready
        if (ai != null)
            ai.SetTarget(target);
    }

    private Transform GetEnemyTowerTransform()
    {
        GameObject tower = GameObject.FindGameObjectWithTag("EnemyTower");
        return tower != null ? tower.transform : null;
    }

    private Transform GetPlayerTowerTransform()
    {
        GameObject tower = GameObject.FindGameObjectWithTag("PlayerTower");
        return tower != null ? tower.transform : null;
    }
}