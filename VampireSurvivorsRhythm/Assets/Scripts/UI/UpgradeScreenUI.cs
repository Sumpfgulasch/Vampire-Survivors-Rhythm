using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Audio;
using FMOD.Studio;

/// <summary>
/// Manages the upgrade selection screen
/// </summary>
public class UpgradeScreenUI : MonoBehaviour {
    [Header("UI Elements")] [SerializeField]
    private GameObject upgradePanel;

    [SerializeField] private Button[] upgradeButtons = new Button[3];
    [SerializeField] private Image[] upgradeIcons = new Image[3];
    [SerializeField] private TextMeshProUGUI[] upgradeNames = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] upgradeDescriptions = new TextMeshProUGUI[3];

    private List<object> currentOptions = new List<object>(); // Can be AttackTypeSO or AttackUpgradeSO

    private void Start() {
        // Subscribe to level up event
        if (LevelManager.Instance != null) {
            LevelManager.Instance.OnLevelUp.AddListener(OnLevelUp);
        }

        // Hide panel initially
        if (upgradePanel != null) {
            upgradePanel.SetActive(false);
        }

        // Setup button listeners
        for (int i = 0; i < upgradeButtons.Length; i++) {
            int index = i; // Capture for closure
            if (upgradeButtons[i] != null) {
                upgradeButtons[i].onClick.AddListener(() => StartCoroutine(OnUpgradeSelected(index)));
            }
        }
    }

    /// <summary>
    /// Called when player levels up
    /// </summary>
    private void OnLevelUp(int level) {
        FeedbackManager.Instance.TriggerLevelUpFeedback();
        ShowUpgradeOptions();
    }

    /// <summary>
    /// Show the upgrade screen with random options
    /// </summary>
    private void ShowUpgradeOptions() {
        if (upgradePanel == null) return;

        // Generate 3 random options
        currentOptions.Clear();
        currentOptions = GenerateUpgradeOptions(3);

        // Update UI for each option
        for (int i = 0; i < upgradeButtons.Length && i < currentOptions.Count; i++) {
            UpdateUpgradeButton(i, currentOptions[i]);
        }

        // Show panel
        upgradePanel.SetActive(true);
    }

    /// <summary>
    /// Generate random upgrade options
    /// </summary>
    private List<object> GenerateUpgradeOptions(int count) {
        List<object> options = new List<object>();

        if (LevelManager.Instance == null || LevelManager.Instance.CurrentStageConfig == null) {
            return options;
        }

        StageConfigSO stage = LevelManager.Instance.CurrentStageConfig;

        // Pool of available options
        List<object> availableOptions = new List<object>();

        // Add available upgrades
        if (stage.AvailableUpgrades != null) {
            availableOptions.AddRange(stage.AvailableUpgrades);
        }

        // Add available new attacks
        if (stage.AvailableNewAttacks != null) {
            availableOptions.AddRange(stage.AvailableNewAttacks);
        }

        // Randomly select options
        for (int i = 0; i < count && availableOptions.Count > 0; i++) {
            int randomIndex = Random.Range(0, availableOptions.Count);
            options.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex); // Don't show same option twice
        }

        return options;
    }

    /// <summary>
    /// Update a single upgrade button
    /// </summary>
    private void UpdateUpgradeButton(int index, object option) {
        if (index >= upgradeButtons.Length) return;

        // Check if it's an upgrade or new attack
        if (option is AttackUpgradeSO upgrade) {
            if (upgradeIcons[index] != null)
                upgradeIcons[index].sprite = upgrade.Icon;
            if (upgradeNames[index] != null)
                upgradeNames[index].text = upgrade.UpgradeName;
            if (upgradeDescriptions[index] != null)
                upgradeDescriptions[index].text = upgrade.Description;
        }
        else if (option is AttackTypeSO attack) {
            if (upgradeIcons[index] != null)
                upgradeIcons[index].sprite = attack.Icon;
            if (upgradeNames[index] != null)
                upgradeNames[index].text = attack.AttackName;
            if (upgradeDescriptions[index] != null)
                upgradeDescriptions[index].text = $"New Attack: {attack.AttackName}";
        }

        // Enable button
        if (upgradeButtons[index] != null) {
            upgradeButtons[index].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when an upgrade is selected
    /// </summary>
    private IEnumerator OnUpgradeSelected(int index) {
        if (index >= currentOptions.Count) 
            yield return null;
        else {
            var selectedOption = currentOptions[index];

            // Apply the upgrade or add the attack
            if (selectedOption is AttackUpgradeSO upgrade) {
                PlayerAttackManager.Instance.ApplyUpgrade(upgrade);
            }
            else if (selectedOption is AttackTypeSO attack) {
                PlayerAttackManager.Instance.AddAttack(attack);
            }
            
            GameStateManager.Instance.StartGameplay();
            yield return new WaitForEndOfFrame();
            
            yield return new WaitForSeconds(BeatManager.Instance.TimeUntilNextBeat());
            
            // Hide panel and resume game
            if (upgradePanel != null) {
                upgradePanel.SetActive(false);
            }
        }
    }
}