using UnityEngine;
using System.Collections;

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

    [Header("Enemy Mana")]
    public float currentEnemyMana = 10f;
    public float enemyMaxMana = 20f;

    [Header("Enemy AI Settings")]
    public float enemySpawnInterval = 6f;
    public float enemyInitialDelay = 2f;

    [Header("Game Timer")]
    public float gameDuration = 300f;

    private float nextEnemySpawnTime;
    private float gameStartTime;
    private bool gameEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameStartTime = Time.time;
        InvokeRepeating("RegenerateMana", 1f, 1f);
        nextEnemySpawnTime = Time.time + enemyInitialDelay;
        
        StartCoroutine(GameTimer());
    }

    private void Update()
    {
        if (gameEnded) return;
        
        if (Time.time >= nextEnemySpawnTime)
        {
            TryEnemyGreedySpawn();
            nextEnemySpawnTime = Time.time + enemySpawnInterval;
        }
    }

    private void RegenerateMana()
    {
        currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
        currentEnemyMana = Mathf.Min(currentEnemyMana + manaRegenRate, enemyMaxMana);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateManaDisplays();
            UIManager.Instance.UpdateButtonInteractability();
        }
    }

    // ====================== PLAYER SPAWNING ======================
    public void SpawnKnight()
    {
        if (gameEnded || currentMana < 6f) return;
        currentMana -= 6f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        GameObject troop = Instantiate(knightPrefab, playerSpawnPoint.position + offset, Quaternion.identity);

        SetupTroop(troop, "PlayerTroop", GetEnemyTowerTransform());

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateManaDisplays();
            UIManager.Instance.UpdateButtonInteractability();
        }
    }

    public void SpawnArcher()
    {
        if (gameEnded || currentMana < 3f) return;
        currentMana -= 3f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        GameObject troop = Instantiate(archerPrefab, playerSpawnPoint.position + offset, Quaternion.identity);

        SetupTroop(troop, "PlayerTroop", GetEnemyTowerTransform());

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateManaDisplays();
            UIManager.Instance.UpdateButtonInteractability();
        }
    }
    
    private void TryEnemyGreedySpawn()
    {
        if (gameEnded) return;
        
        float randomRoll = Random.value; 

        if (randomRoll < 0.8f)   // 80% chance
        {
            if (currentEnemyMana >= 3f)
            {
                SpawnEnemyArcher();
            }
            else if (currentEnemyMana >= 6f)
            {
                SpawnEnemyKnight();
            }
        }
        else   // 20% chance
        {
            if (currentEnemyMana >= 6f)
            {
                SpawnEnemyKnight();
            }
            else if (currentEnemyMana >= 3f)
            {
                SpawnEnemyArcher();
            }
        }
    }

    private void SpawnEnemyArcher()
    {
        currentEnemyMana -= 3f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        Quaternion enemyRotation = Quaternion.Euler(0, 180, 0);

        GameObject troop = Instantiate(archerPrefab, enemySpawnPoint.position + offset, enemyRotation);

        SetupTroop(troop, "EnemyTroop", GetPlayerTowerTransform());

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateManaDisplays();

        Debug.Log("Enemy spawned Archer");
    }

    private void SpawnEnemyKnight()
    {
        currentEnemyMana -= 6f;

        Vector3 offset = new Vector3(Random.Range(-1.2f, 1.2f), 0, Random.Range(-0.8f, 0.8f));
        Quaternion enemyRotation = Quaternion.Euler(0, 180, 0);

        GameObject troop = Instantiate(knightPrefab, enemySpawnPoint.position + offset, enemyRotation);

        SetupTroop(troop, "EnemyTroop", GetPlayerTowerTransform());

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateManaDisplays();

        Debug.Log("Enemy spawned Knight");
    }
    
    // ====================== GAME TIMER & END CONDITIONS ======================
    private IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(gameDuration);
        if (!gameEnded)
            EndGame("Time's Up! It's a Draw");
    }
    
    /*public void OnTowerDestroyed(string towerName)
    {
        if (gameEnded) return;

        if (towerName.Contains("Player"))
            EndGame("Enemy Wins!");
        else
            EndGame("Player Wins!");
    }*/

    private void EndGame(string resultMessage)
    {
        gameEnded = true;
        Debug.Log("Game Over: " + resultMessage);
        
        StopAllCoroutines();
        CancelInvoke();
    }

    // ====================== HELPERS ======================
    private void SetupTroop(GameObject troop, string tag, Transform target)
    {
        if (troop == null) return;
        troop.tag = tag;

        TroopAI ai = troop.GetComponent<TroopAI>();
        if (ai != null)
        {
            StartCoroutine(SetTargetAfterDelay(ai, target));
        }
    }

    private System.Collections.IEnumerator SetTargetAfterDelay(TroopAI ai, Transform target)
    {
        yield return new WaitForSeconds(0.05f);
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