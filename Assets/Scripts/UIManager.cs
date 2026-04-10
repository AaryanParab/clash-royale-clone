using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Mana UI")]
    public ManaBarUI playerManaBar;
    public ManaBarUI enemyManaBar;

    [Header("Spawn Buttons")]
    public Button knightButton;
    public Button archerButton;

    [Header("Mana Text (Optional)")]
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyManaText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;           // ← New: Timer display

    [Header("End Game Screen")]
    public GameObject endGamePanel;
    public TextMeshProUGUI resultText;
    public Button restartButton;
    public Button quitButton;

    private float gameStartTime;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        gameStartTime = Time.time;

        // Link spawn buttons
        if (knightButton != null)
            knightButton.onClick.AddListener(() => GameManager.Instance.SpawnKnight());

        if (archerButton != null)
            archerButton.onClick.AddListener(() => GameManager.Instance.SpawnArcher());

        // Link end game buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Hide end screen at start
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        UpdateManaDisplays();
        UpdateButtonInteractability();
    }

    private void Update()
    {
        if (gameEnded) return;

        // Update Timer every frame
        UpdateTimer();
    }

    // ====================== MANA UI ======================
    public void UpdateManaDisplays()
    {
        if (playerManaBar != null)
            playerManaBar.UpdateMana(GameManager.Instance.currentMana, GameManager.Instance.maxMana);

        if (enemyManaBar != null)
            enemyManaBar.UpdateMana(GameManager.Instance.currentEnemyMana, GameManager.Instance.enemyMaxMana);

        if (playerManaText != null)
            playerManaText.text = $"Mana: {Mathf.Round(GameManager.Instance.currentMana)} / {GameManager.Instance.maxMana}";

        if (enemyManaText != null)
            enemyManaText.text = $"Enemy: {Mathf.Round(GameManager.Instance.currentEnemyMana)} / {GameManager.Instance.enemyMaxMana}";
    }

    public void UpdateButtonInteractability()
    {
        if (knightButton != null)
            knightButton.interactable = GameManager.Instance.currentMana >= 6f;

        if (archerButton != null)
            archerButton.interactable = GameManager.Instance.currentMana >= 3f;
    }

    // ====================== TIMER ======================
    private void UpdateTimer()
    {
        if (timerText == null) return;

        float timeLeft = GameManager.Instance.gameDuration - (Time.time - gameStartTime);

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            gameEnded = true;
        }

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Optional: Change color when time is low
        if (timeLeft <= 30f)
            timerText.color = Color.red;
    }

    // ====================== END GAME SCREEN ======================
    public void ShowEndScreen(string resultMessage)
    {
        if (endGamePanel == null) return;

        gameEnded = true;
        endGamePanel.SetActive(true);
        resultText.text = resultMessage;

        Time.timeScale = 0f;        // Pause the game
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}