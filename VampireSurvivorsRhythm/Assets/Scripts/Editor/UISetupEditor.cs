using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Editor script to automatically create UI elements for the rhythm game
/// </summary>
public class UISetupEditor : EditorWindow
{
    [MenuItem("Tools/Setup Rhythm Game UI")]
    public static void ShowWindow()
    {
        GetWindow<UISetupEditor>("UI Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Rhythm Game UI Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create All UI Elements", GUILayout.Height(40)))
        {
            CreateAllUI();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create HUD Only", GUILayout.Height(30)))
        {
            CreateHUD();
        }
        
        if (GUILayout.Button("Create Upgrade Screen Only", GUILayout.Height(30)))
        {
            CreateUpgradeScreen();
        }
        
        if (GUILayout.Button("Create Game Over Screen Only", GUILayout.Height(30)))
        {
            CreateGameOverScreen();
        }
        
        if (GUILayout.Button("Create Beat Feedback Only", GUILayout.Height(30)))
        {
            CreateBeatFeedback();
        }
    }
    
    private static void CreateAllUI()
    {
        CreateHUD();
        CreateUpgradeScreen();
        CreateGameOverScreen();
        CreateBeatFeedback();
        
        Debug.Log("All UI elements created successfully!");
    }
    
    #region HUD Creation
    
    private static void CreateHUD()
    {
        // Find or create HUD Canvas
        Canvas hudCanvas = FindOrCreateCanvas("HUD Canvas", RenderMode.ScreenSpaceOverlay, 0);
        
        // Add HUDManager component
        HUDManager hudManager = hudCanvas.GetComponent<HUDManager>();
        if (hudManager == null)
        {
            hudManager = hudCanvas.gameObject.AddComponent<HUDManager>();
        }
        
        // Create HUD container
        GameObject hudContainer = CreateUIObject("HUD Container", hudCanvas.transform);
        RectTransform hudRect = hudContainer.GetComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.offsetMin = new Vector2(20, 20);
        hudRect.offsetMax = new Vector2(-20, -20);
        
        // Create Health Bar
        GameObject healthBar = CreateSlider("Health Bar", hudContainer.transform, new Vector2(20, -20), new Vector2(320, -70), Color.red);
        Slider healthSlider = healthBar.GetComponent<Slider>();
        
        // Create Health Text
        GameObject healthText = CreateText("Health Text", hudContainer.transform, new Vector2(20, -20), new Vector2(320, -70), "3/3", 24);
        
        // Create Experience Bar
        GameObject expBar = CreateSlider("Experience Bar", hudContainer.transform, new Vector2(20, -90), new Vector2(320, -140), Color.cyan);
        Slider expSlider = expBar.GetComponent<Slider>();
        
        // Create Experience Text
        GameObject expText = CreateText("Experience Text", hudContainer.transform, new Vector2(20, -90), new Vector2(320, -140), "0/10", 20);
        
        // Create Level Text
        GameObject levelText = CreateText("Level Text", hudContainer.transform, new Vector2(20, -160), new Vector2(200, -200), "Level 1", 32);
        
        // Create Enemy Count Text
        GameObject enemyCountText = CreateText("Enemy Count", hudContainer.transform, new Vector2(-320, -20), new Vector2(-20, -50), "Enemies: 0", 18);
        RectTransform enemyRect = enemyCountText.GetComponent<RectTransform>();
        enemyRect.anchorMin = new Vector2(1, 1);
        enemyRect.anchorMax = new Vector2(1, 1);
        
        // Create Attack Count Text
        GameObject attackCountText = CreateText("Attack Count", hudContainer.transform, new Vector2(-320, -60), new Vector2(-20, -90), "Attacks: 0", 18);
        RectTransform attackRect = attackCountText.GetComponent<RectTransform>();
        attackRect.anchorMin = new Vector2(1, 1);
        attackRect.anchorMax = new Vector2(1, 1);
        
        // Assign references to HUDManager
        SerializedObject so = new SerializedObject(hudManager);
        so.FindProperty("healthSlider").objectReferenceValue = healthSlider;
        so.FindProperty("healthText").objectReferenceValue = healthText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("experienceSlider").objectReferenceValue = expSlider;
        so.FindProperty("experienceText").objectReferenceValue = expText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("enemyCountText").objectReferenceValue = enemyCountText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("attackCountText").objectReferenceValue = attackCountText.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();
        
        Debug.Log("HUD created successfully!");
    }
    
    #endregion
    
    #region Upgrade Screen Creation
    
    private static void CreateUpgradeScreen()
    {
        // Find or create Upgrade Canvas
        Canvas upgradeCanvas = FindOrCreateCanvas("Upgrade Canvas", RenderMode.ScreenSpaceOverlay, 10);
        
        // Add UpgradeScreenUI component
        UpgradeScreenUI upgradeUI = upgradeCanvas.GetComponent<UpgradeScreenUI>();
        if (upgradeUI == null)
        {
            upgradeUI = upgradeCanvas.gameObject.AddComponent<UpgradeScreenUI>();
        }
        
        // Create upgrade panel (background)
        GameObject upgradePanel = CreateUIObject("Upgrade Panel", upgradeCanvas.transform);
        RectTransform panelRect = upgradePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = upgradePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        // Create title
        GameObject title = CreateText("Title", upgradePanel.transform, new Vector2(-200, -50), new Vector2(200, -100), "LEVEL UP!", 48);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        
        // Create 3 upgrade cards
        Button[] upgradeButtons = new Button[3];
        Image[] upgradeIcons = new Image[3];
        TextMeshProUGUI[] upgradeNames = new TextMeshProUGUI[3];
        TextMeshProUGUI[] upgradeDescriptions = new TextMeshProUGUI[3];
        
        for (int i = 0; i < 3; i++)
        {
            float xPos = (i - 1) * 350f; // -350, 0, 350
            
            GameObject card = CreateUpgradeCard($"Upgrade Card {i + 1}", upgradePanel.transform, xPos);
            upgradeButtons[i] = card.GetComponent<Button>();
            
            // Find child elements
            upgradeIcons[i] = card.transform.Find("Icon").GetComponent<Image>();
            upgradeNames[i] = card.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            upgradeDescriptions[i] = card.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        }
        
        // Assign references to UpgradeScreenUI
        SerializedObject so = new SerializedObject(upgradeUI);
        so.FindProperty("upgradePanel").objectReferenceValue = upgradePanel;
        
        SerializedProperty buttonsArray = so.FindProperty("upgradeButtons");
        buttonsArray.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            buttonsArray.GetArrayElementAtIndex(i).objectReferenceValue = upgradeButtons[i];
        }
        
        SerializedProperty iconsArray = so.FindProperty("upgradeIcons");
        iconsArray.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            iconsArray.GetArrayElementAtIndex(i).objectReferenceValue = upgradeIcons[i];
        }
        
        SerializedProperty namesArray = so.FindProperty("upgradeNames");
        namesArray.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            namesArray.GetArrayElementAtIndex(i).objectReferenceValue = upgradeNames[i];
        }
        
        SerializedProperty descriptionsArray = so.FindProperty("upgradeDescriptions");
        descriptionsArray.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            descriptionsArray.GetArrayElementAtIndex(i).objectReferenceValue = upgradeDescriptions[i];
        }
        
        so.ApplyModifiedProperties();
        
        // Disable panel initially
        upgradePanel.SetActive(false);
        
        Debug.Log("Upgrade Screen created successfully!");
    }
    
    private static GameObject CreateUpgradeCard(string name, Transform parent, float xPos)
    {
        GameObject card = CreateUIObject(name, parent);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(xPos, 0);
        cardRect.sizeDelta = new Vector2(300, 400);
        
        // Add button component
        Button button = card.AddComponent<Button>();
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Create icon
        GameObject icon = CreateUIObject("Icon", card.transform);
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 1f);
        iconRect.anchorMax = new Vector2(0.5f, 1f);
        iconRect.anchoredPosition = new Vector2(0, -100);
        iconRect.sizeDelta = new Vector2(150, 150);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        
        // Create name text
        GameObject nameObj = CreateText("Name", card.transform, new Vector2(-140, -180), new Vector2(140, -220), "Upgrade Name", 24);
        
        // Create description text
        GameObject descObj = CreateText("Description", card.transform, new Vector2(-140, -240), new Vector2(140, -360), "Upgrade description goes here", 18);
        TextMeshProUGUI descText = descObj.GetComponent<TextMeshProUGUI>();
        descText.alignment = TextAlignmentOptions.Top;
        
        return card;
    }
    
    #endregion
    
    #region Game Over Screen Creation
    
    private static void CreateGameOverScreen()
    {
        // Find or create Game Over Canvas
        Canvas gameOverCanvas = FindOrCreateCanvas("GameOver Canvas", RenderMode.ScreenSpaceOverlay, 20);
        
        // Add GameOverUI component
        GameOverUI gameOverUI = gameOverCanvas.GetComponent<GameOverUI>();
        if (gameOverUI == null)
        {
            gameOverUI = gameOverCanvas.gameObject.AddComponent<GameOverUI>();
        }
        
        // Create game over panel (background)
        GameObject gameOverPanel = CreateUIObject("GameOver Panel", gameOverCanvas.transform);
        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        // Create "GAME OVER" text
        GameObject gameOverText = CreateText("Game Over Text", gameOverPanel.transform, new Vector2(-300, -100), new Vector2(300, -200), "GAME OVER", 72);
        RectTransform gameOverRect = gameOverText.GetComponent<RectTransform>();
        gameOverRect.anchorMin = new Vector2(0.5f, 0.5f);
        gameOverRect.anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI gameOverTMP = gameOverText.GetComponent<TextMeshProUGUI>();
        gameOverTMP.color = Color.red;
        
        // Create final level text
        GameObject finalLevelText = CreateText("Final Level Text", gameOverPanel.transform, new Vector2(-200, -220), new Vector2(200, -270), "Level Reached: 1", 32);
        RectTransform levelRect = finalLevelText.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0.5f, 0.5f);
        levelRect.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Create final score text
        GameObject finalScoreText = CreateText("Final Score Text", gameOverPanel.transform, new Vector2(-200, -290), new Vector2(200, -340), "Score: 0", 28);
        RectTransform scoreRect = finalScoreText.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        
        // Create Restart button
        GameObject restartButton = CreateButton("Restart Button", gameOverPanel.transform, new Vector2(-150, 100), new Vector2(150, 50), "RESTART");
        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0f);
        restartRect.anchorMax = new Vector2(0.5f, 0f);
        
        // Create Quit button
        GameObject quitButton = CreateButton("Quit Button", gameOverPanel.transform, new Vector2(-150, 30), new Vector2(150, -20), "QUIT");
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchorMin = new Vector2(0.5f, 0f);
        quitRect.anchorMax = new Vector2(0.5f, 0f);
        
        // Assign references to GameOverUI
        SerializedObject so = new SerializedObject(gameOverUI);
        so.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        so.FindProperty("finalLevelText").objectReferenceValue = finalLevelText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("finalScoreText").objectReferenceValue = finalScoreText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("restartButton").objectReferenceValue = restartButton.GetComponent<Button>();
        so.FindProperty("quitButton").objectReferenceValue = quitButton.GetComponent<Button>();
        so.ApplyModifiedProperties();
        
        // Disable panel initially
        gameOverPanel.SetActive(false);
        
        Debug.Log("Game Over Screen created successfully!");
    }
    
    #endregion
    
    #region Beat Feedback Creation
    
    private static void CreateBeatFeedback()
    {
        // Find or create Beat Feedback Canvas
        Canvas beatCanvas = FindOrCreateCanvas("Beat Feedback Canvas", RenderMode.ScreenSpaceOverlay, 5);
        
        // Add BeatFeedbackUI component
        BeatFeedbackUI beatFeedbackUI = beatCanvas.GetComponent<BeatFeedbackUI>();
        if (beatFeedbackUI == null)
        {
            beatFeedbackUI = beatCanvas.gameObject.AddComponent<BeatFeedbackUI>();
        }
        
        // Create full-screen overlay
        GameObject overlay = CreateUIObject("Beat Overlay", beatCanvas.transform);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(1, 1, 1, 0);
        overlayImage.raycastTarget = false;
        
        // Assign reference to BeatFeedbackUI
        SerializedObject so = new SerializedObject(beatFeedbackUI);
        so.FindProperty("beatOverlay").objectReferenceValue = overlayImage;
        so.ApplyModifiedProperties();
        
        Debug.Log("Beat Feedback created successfully!");
    }
    
    #endregion
    
    #region Helper Methods
    
    private static Canvas FindOrCreateCanvas(string name, RenderMode renderMode, int sortOrder)
    {
        // Try to find existing canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvases)
        {
            if (c.name == name)
            {
                return c;
            }
        }
        
        // Create new canvas
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = sortOrder;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        return canvas;
    }
    
    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        return obj;
    }
    
    private static GameObject CreateText(string name, Transform parent, Vector2 min, Vector2 max, string text, int fontSize)
    {
        GameObject textObj = CreateUIObject(name, parent);
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = (min + max) / 2f;
        rect.sizeDelta = max - min;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return textObj;
    }
    
    private static GameObject CreateSlider(string name, Transform parent, Vector2 min, Vector2 max, Color fillColor)
    {
        GameObject sliderObj = CreateUIObject(name, parent);
        RectTransform rect = sliderObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = (min + max) / 2f;
        rect.sizeDelta = max - min;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        
        // Create background
        GameObject background = CreateUIObject("Background", sliderObj.transform);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create fill area
        GameObject fillArea = CreateUIObject("Fill Area", sliderObj.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        // Create fill
        GameObject fill = CreateUIObject("Fill", fillArea.transform);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = fillColor;
        
        // Configure slider
        slider.fillRect = fillRect;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        slider.interactable = false;
        
        return sliderObj;
    }
    
    private static GameObject CreateButton(string name, Transform parent, Vector2 min, Vector2 max, string text)
    {
        GameObject buttonObj = CreateUIObject(name, parent);
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = (min + max) / 2f;
        rect.sizeDelta = max - min;
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create text child
        GameObject textObj = CreateUIObject("Text", buttonObj.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return buttonObj;
    }
    
    #endregion
}
