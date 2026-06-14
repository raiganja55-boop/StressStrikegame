using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GenerateLevelUI
{
    [MenuItem("Tools/Generate Level UI")]
    public static void GenerateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("level ui");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create EventSystem if not exists
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Background
        GameObject bg = CreateUIElement("Background", canvasGO.transform, new Vector2(0,0), new Vector2(1,1), Vector2.zero, Vector2.zero);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.05f, 0.05f, 1f); // Dark red/brown background

        // Title Banner "STRESS STRIKE"
        GameObject titleBanner = CreateUIElement("TitleBanner", canvasGO.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(250, -50), new Vector2(400, 80));
        Image titleImg = titleBanner.AddComponent<Image>();
        titleImg.color = Color.red;
        // Slight rotation to match the angled box in Figma
        titleBanner.transform.localRotation = Quaternion.Euler(0, 0, 5);
        
        GameObject titleText = CreateUIElement("TitleText", titleBanner.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text tText = titleText.AddComponent<Text>();
        tText.text = "STRESS STRIKE";
        tText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tText.fontStyle = FontStyle.BoldAndItalic;
        tText.fontSize = 40;
        tText.color = Color.white;
        tText.alignment = TextAnchor.MiddleCenter;

        // Arena Mode Header
        GameObject headerPanel = CreateUIElement("HeaderPanel", canvasGO.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -100), new Vector2(600, 120));
        Image headerImg = headerPanel.AddComponent<Image>();
        headerImg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // Header Outline
        Outline headerOutline = headerPanel.AddComponent<Outline>();
        headerOutline.effectColor = Color.red;
        headerOutline.effectDistance = new Vector2(4, -4);

        GameObject headerText = CreateUIElement("HeaderText", headerPanel.transform, Vector2.zero, Vector2.one, new Vector2(0, 20), Vector2.zero);
        Text hText = headerText.AddComponent<Text>();
        hText.text = "ARENA MODE";
        hText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hText.fontStyle = FontStyle.BoldAndItalic;
        hText.fontSize = 60;
        hText.color = Color.white;
        hText.alignment = TextAnchor.MiddleCenter;

        GameObject subText = CreateUIElement("SubText", headerPanel.transform, Vector2.zero, Vector2.one, new Vector2(0, -30), Vector2.zero);
        Text sText = subText.AddComponent<Text>();
        sText.text = "SELECT YOUR TARGET";
        sText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        sText.fontStyle = FontStyle.Italic;
        sText.fontSize = 20;
        sText.color = Color.gray;
        sText.alignment = TextAnchor.MiddleCenter;

        // Cards Container
        GameObject cardsContainer = CreateUIElement("CardsContainer", canvasGO.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(1400, 700));
        HorizontalLayoutGroup hlg = cardsContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 50;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlHeight = false;
        hlg.childControlWidth = false;

        // Create 3 Cards
        CreateCard(cardsContainer.transform, "Level01", "LEVEL 01", "THE HEARTBREAKER", new Color(0.05f, 0.3f, 0.1f), "LOW", Color.green);
        CreateCard(cardsContainer.transform, "Level02", "LEVEL 02", "THE BONE CRUSHER", new Color(0.4f, 0.2f, 0.05f), "MEDIUM", Color.yellow);
        CreateCard(cardsContainer.transform, "Level03", "LEVEL 03", "CYBER THREAT", new Color(0.4f, 0.05f, 0.05f), "CRITICAL", Color.red);

        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Level UI");
        Selection.activeGameObject = canvasGO;
    }

    private static void CreateCard(Transform parent, string name, string levelName, string charName, Color bgColor, string healthText, Color healthColor)
    {
        // Each card needs its own size since HorizontalLayoutGroup childControl is false
        GameObject card = CreateUIElement(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 650));
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = bgColor;
        
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(4, -4);

        // Level Title
        GameObject titleText = CreateUIElement("LevelTitle", card.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -50), new Vector2(0, 50));
        Text tText = titleText.AddComponent<Text>();
        tText.text = levelName;
        tText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tText.fontStyle = FontStyle.BoldAndItalic;
        tText.fontSize = 45;
        tText.color = Color.white;
        tText.alignment = TextAnchor.MiddleCenter;

        // Red separator line
        GameObject separator = CreateUIElement("Separator", card.transform, new Vector2(0.1f, 1), new Vector2(0.9f, 1), new Vector2(0, -90), new Vector2(0, 4));
        Image sepImg = separator.AddComponent<Image>();
        sepImg.color = Color.red;

        // Character Name
        GameObject charText = CreateUIElement("CharName", card.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -120), new Vector2(0, 30));
        Text cText = charText.AddComponent<Text>();
        cText.text = charName;
        cText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        cText.fontStyle = FontStyle.Italic;
        cText.fontSize = 20;
        cText.color = Color.white;
        cText.alignment = TextAnchor.MiddleCenter;

        // Character Image Placeholder
        GameObject charImgGO = CreateUIElement("CharacterImage", card.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(250, 350));
        Image charImg = charImgGO.AddComponent<Image>();
        charImg.color = new Color(1, 1, 1, 0.05f); // Semi transparent placeholder

        // Bottom section (dark panel)
        GameObject bottomPanel = CreateUIElement("BottomPanel", card.transform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 80), new Vector2(0, 150));
        Image bottomImg = bottomPanel.AddComponent<Image>();
        bottomImg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        // Health Text
        GameObject hTextGO = CreateUIElement("HealthText", bottomPanel.transform, new Vector2(0.05f, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(0, 20));
        Text hTextComponent = hTextGO.AddComponent<Text>();
        hTextComponent.text = healthText;
        hTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hTextComponent.fontSize = 16;
        hTextComponent.color = healthColor;
        hTextComponent.alignment = TextAnchor.MiddleLeft;

        // Health Bar BG
        GameObject hbBg = CreateUIElement("HealthBarBG", bottomPanel.transform, new Vector2(0.05f, 1), new Vector2(0.95f, 1), new Vector2(0, -40), new Vector2(0, 10));
        Image hbBgImg = hbBg.AddComponent<Image>();
        hbBgImg.color = new Color(0.3f, 0.3f, 0.3f);

        // Health Bar Fill
        GameObject hbFill = CreateUIElement("HealthBarFill", hbBg.transform, new Vector2(0, 0), new Vector2(healthColor == Color.green ? 0.3f : healthColor == Color.yellow ? 0.6f : 0.9f, 1), Vector2.zero, Vector2.zero);
        Image hbFillImg = hbFill.AddComponent<Image>();
        hbFillImg.color = healthColor;

        // Fight Button
        GameObject buttonGO = CreateUIElement("FightButton", bottomPanel.transform, new Vector2(0.05f, 0), new Vector2(0.95f, 0), new Vector2(0, 45), new Vector2(0, 60));
        Image btnImg = buttonGO.AddComponent<Image>();
        btnImg.color = Color.red;
        Button btn = buttonGO.AddComponent<Button>();

        GameObject btnTextGO = CreateUIElement("Text", buttonGO.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text btnText = btnTextGO.AddComponent<Text>();
        btnText.text = "FIGHT";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontStyle = FontStyle.BoldAndItalic;
        btnText.fontSize = 30;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
    }

    private static GameObject CreateUIElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = sizeDelta;
        rt.localScale = Vector3.one;
        return go;
    }
}
