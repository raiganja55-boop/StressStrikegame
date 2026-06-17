using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class SetupCombatText : EditorWindow
{
    [MenuItem("Tools/Stress Strike/Setup Combat Text UI")]
    public static void SetupUI()
    {
        // 1. Find the CombatHudController in the scene
        CombatHudController hudController = FindObjectOfType<CombatHudController>();
        if (hudController == null)
        {
            Debug.LogError("Could not find a CombatHudController in the scene. Please make sure the UI Mainmenu scene is open.");
            return;
        }

        // Record undo so the user can easily revert if they want
        Undo.RecordObject(hudController.gameObject, "Setup Combat Text UI");

        // 2. Setup Round Text
        Transform roundsParent = hudController.transform.Find("Rounds");
        if (roundsParent == null) roundsParent = hudController.transform;

        TextMeshProUGUI roundText = CreateTextElement("Round Text", roundsParent, new Vector2(0, 1), new Vector2(0, 1), new Vector2(56.17584f, -49.4371f), new Vector2(400, 100));
        roundText.text = "--VS--";
        roundText.fontSize = 72;
        roundText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        roundText.alignment = TextAlignmentOptions.Center;
        roundText.transform.localRotation = Quaternion.Euler(0, 0, 6.91f);
        roundText.transform.localScale = new Vector3(0.1476777f, 0.1585276f, 0.4075146f);
        
        // Add a simple shadow/outline feel (this relies on the default material, but gives a starting point)
        roundText.color = Color.white;

        // 3. Setup Player Name
        TextMeshProUGUI playerNameText = CreateTextElement("Player Name Text", hudController.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(490f, -142f), new Vector2(300, 60));
        playerNameText.text = "Player";
        playerNameText.fontSize = 48;
        playerNameText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        playerNameText.alignment = TextAlignmentOptions.Left;

        // 4. Setup Opponent Name
        TextMeshProUGUI opponentNameText = CreateTextElement("Opponent Name Text", hudController.transform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-439f, -137f), new Vector2(300, 60));
        opponentNameText.text = "Damien";
        opponentNameText.fontSize = 48;
        opponentNameText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        opponentNameText.alignment = TextAlignmentOptions.Right;

        // 5. Setup Opponent Portrait Image
        GameObject portraitObj = new GameObject("Opponent Portrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(portraitObj, "Create Opponent Portrait");
        portraitObj.transform.SetParent(hudController.transform, false);
        RectTransform portraitRect = portraitObj.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(1, 1);
        portraitRect.anchorMax = new Vector2(1, 1);
        portraitRect.pivot = new Vector2(1, 1);
        portraitRect.anchoredPosition = new Vector2(-148, -116);
        portraitRect.sizeDelta = new Vector2(120, 120);
        Image portraitImage = portraitObj.GetComponent<Image>();

        // 6. Setup Player Portrait Image
        GameObject playerPortraitObj = new GameObject("Player Portrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(playerPortraitObj, "Create Player Portrait");
        playerPortraitObj.transform.SetParent(hudController.transform, false);
        RectTransform pRect = playerPortraitObj.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0, 1);
        pRect.anchorMax = new Vector2(0, 1);
        pRect.pivot = new Vector2(0, 1);
        pRect.anchoredPosition = new Vector2(194, -116);
        pRect.sizeDelta = new Vector2(120, 120);
        Image pImage = playerPortraitObj.GetComponent<Image>();

        // 7. Link to the script using SerializedObject
        SerializedObject serializedHud = new SerializedObject(hudController);
        serializedHud.Update();
        serializedHud.FindProperty("_roundText").objectReferenceValue = roundText;
        serializedHud.FindProperty("_playerNameText").objectReferenceValue = playerNameText;
        serializedHud.FindProperty("_opponentNameText").objectReferenceValue = opponentNameText;
        serializedHud.FindProperty("_opponentPortraitImage").objectReferenceValue = portraitImage;
        serializedHud.FindProperty("_playerPortraitImage").objectReferenceValue = pImage;
        serializedHud.ApplyModifiedProperties();

        Debug.Log("Successfully generated and linked Combat Text UI elements! You can now adjust their exact positions in the Scene view.");
    }

    private static TextMeshProUGUI CreateTextElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(textObj, "Create " + name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        return textObj.GetComponent<TextMeshProUGUI>();
    }
}
