using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public bool IsGameOver { get; private set; }
    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        IsGameOver = false;
        score = 0;
        UpdateScoreUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "⭐ Score: " + score;
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        FindObjectOfType<PlayerController>()?.SetDead(true);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Final Score: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}