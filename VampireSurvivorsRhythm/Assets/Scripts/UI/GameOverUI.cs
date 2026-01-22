using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the game over screen
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalLevelText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    private void Start()
    {
        // Subscribe to game state changes
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStateChanged.AddListener(OnStateChanged);
        }
        
        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        // Hide panel initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Called when game state changes
    /// </summary>
    private void OnStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver)
        {
            ShowGameOver();
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Show the game over screen
    /// </summary>
    private void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        
        // Update final stats
        if (LevelManager.Instance != null)
        {
            if (finalLevelText != null)
            {
                finalLevelText.text = $"Level Reached: {LevelManager.Instance.CurrentLevel}";
            }
            
            if (finalScoreText != null)
            {
                // Calculate score (level * 100 + experience)
                int score = LevelManager.Instance.CurrentLevel * 100 + LevelManager.Instance.CurrentExperience;
                finalScoreText.text = $"Score: {score}";
            }
        }
        
        // Show panel
        gameOverPanel.SetActive(true);
    }
    
    /// <summary>
    /// Restart the game
    /// </summary>
    private void OnRestartClicked()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RestartGame();
        }
    }
    
    /// <summary>
    /// Quit the game
    /// </summary>
    private void OnQuitClicked()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.QuitGame();
        }
    }
}
