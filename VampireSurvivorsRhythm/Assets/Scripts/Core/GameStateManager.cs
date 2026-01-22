using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages game state transitions and pausing
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    [Header("Events")]
    public UnityEvent<GameState> OnStateChanged { get; private set; }= new();
    
    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.Gameplay;
    
    public GameState CurrentState => currentState;
    public bool IsGameplayActive => currentState == GameState.Gameplay;
    public bool IsPaused => currentState == GameState.UpgradeSelection;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        SetState(GameState.Gameplay);
    }
    
    /// <summary>
    /// Change the current game state
    /// </summary>
    public void SetState(GameState newState)
    {
        if (currentState == newState) return;
        
        GameState previousState = currentState;
        currentState = newState;
        
        // Handle state-specific logic
        switch (newState)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                if (BeatManager.Instance != null)
                {
                    BeatManager.Instance.ResumeBeat();
                }
                break;
                
            case GameState.UpgradeSelection:
                Time.timeScale = 0f;
                if (BeatManager.Instance != null)
                {
                    BeatManager.Instance.StopBeat();
                }
                break;
                
            case GameState.GameOver:
                //Time.timeScale = 0f;
                if (BeatManager.Instance != null)
                {
                    BeatManager.Instance.StopBeat();
                }
                break;
        }
        
        Debug.Log($"Game State changed: {previousState} -> {newState}");
        OnStateChanged?.Invoke(newState);
    }
    
    /// <summary>
    /// Transition to gameplay state
    /// </summary>
    public void StartGameplay()
    {
        SetState(GameState.Gameplay);
    }
    
    /// <summary>
    /// Transition to upgrade selection state
    /// </summary>
    public void ShowUpgradeScreen()
    {
        SetState(GameState.UpgradeSelection);
    }
    
    /// <summary>
    /// Transition to game over state
    /// </summary>
    public void GameOver()
    {
        SetState(GameState.GameOver);
    }
    
    /// <summary>
    /// Restart the game
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

/// <summary>
/// Possible game states
/// </summary>
public enum GameState
{
    Gameplay,           // Phase A: Active combat
    UpgradeSelection,   // Phase B: Paused, showing upgrade options
    GameOver            // Player died
}
