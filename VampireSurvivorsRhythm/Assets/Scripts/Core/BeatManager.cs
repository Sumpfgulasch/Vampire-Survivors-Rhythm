using System;
using Audio;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central timing system that synchronizes all game actions to music beats
/// </summary>
public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }
    
    [Header("Configuration")]
    [SerializeField] private GameConfigSO gameConfig;
    
    [Header("Events")]
    public UnityEvent OnBeat {get; private set; } = new();
    public UnityEvent<int> OnBeatNumber {get; private set; } = new();
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    // Properties
    public float BPM => gameConfig != null ? gameConfig.BPM : 120f;
    public float SecondsPerBeat => 60f / BPM;
    public int CurrentBeat { get; private set; }
    public float BeatProgress { get; private set; } // 0-1 within current beat
    /// <summary>
    /// Get the time until the next beat
    /// </summary>
    public float TimeUntilNextBeat() => nextBeatTime - Time.time;

    private float nextBeatTime;
    private float gameStartTime;
    private bool isPlaying;
    private bool musicStarted = false;
    
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
        if (gameConfig == null)
        {
            Debug.LogError("BeatManager: GameConfigSO is not assigned!");
            return;
        }
        
        StartBeat();
    }
    
    /// <summary>
    /// Start the beat system
    /// </summary>
    public void StartBeat()
    {
        gameStartTime = Time.time;
        nextBeatTime = gameStartTime + SecondsPerBeat;
        CurrentBeat = 0;
        isPlaying = true;
        
        Debug.Log($"BeatManager started: BPM={BPM}, SecondsPerBeat={SecondsPerBeat}");
    }
    
    /// <summary>
    /// Stop the beat system
    /// </summary>
    public void StopBeat()
    {
        isPlaying = false;
    }
    
    /// <summary>
    /// Resume the beat system
    /// </summary>
    public void ResumeBeat()
    {
        isPlaying = true;
    }
    
    private void Update()
    {
        if (!isPlaying || gameConfig == null) return;
        
        float currentTime = Time.time;
        
        // Calculate beat progress
        float timeSinceLastBeat = currentTime - (nextBeatTime - SecondsPerBeat);
        BeatProgress = Mathf.Clamp01(timeSinceLastBeat / SecondsPerBeat);
        
        // Check if we've hit the next beat
        if (currentTime >= nextBeatTime)
        {
            CurrentBeat++;
            nextBeatTime += SecondsPerBeat;
            
            // Fire beat events
            OnBeat?.Invoke();
            OnBeatNumber?.Invoke(CurrentBeat);
            
            if (showDebugInfo)
            {
                Debug.Log($"Beat {CurrentBeat} at time {currentTime:F2}");
            }
        }
        
        
        
        if (!musicStarted && CurrentBeat == 1)
        {
            GameStateManager.Instance.OnStartMusic();
            musicStarted = true;
        }
    }
    
    /// <summary>
    /// Check if current time is within the beat tolerance window
    /// </summary>
    public bool IsOnBeat()
    {
        if (gameConfig == null) return false;
        
        float tolerance = gameConfig.BeatTolerance;
        float timeSinceLastBeat = Time.time - (nextBeatTime - SecondsPerBeat);
        
        // Check if we're near the beat (either just before or just after)
        return timeSinceLastBeat <= tolerance || 
               (SecondsPerBeat - timeSinceLastBeat) <= tolerance;
    }
    
    /// <summary>
    /// Check if this is a specific beat interval (e.g., every 2 beats, every 4 beats)
    /// </summary>
    public bool IsBeatInterval(int interval)
    {
        return CurrentBeat % interval == 0;
    }
    
    private void OnGUI()
    {
        if (!showDebugInfo || !isPlaying) return;
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        
        GUI.Label(new Rect(10, 10, 300, 30), $"Beat: {CurrentBeat}", style);
        GUI.Label(new Rect(10, 40, 300, 30), $"BPM: {BPM}", style);
        GUI.Label(new Rect(10, 70, 300, 30), $"Progress: {BeatProgress:F2}", style);
        GUI.Label(new Rect(10, 100, 300, 30), $"Next Beat: {TimeUntilNextBeat():F2}s", style);
    }
}
