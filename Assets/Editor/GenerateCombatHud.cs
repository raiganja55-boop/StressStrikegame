using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class GenerateCombatHud
{
    private const string SourceHudPath = "Assets/UI/health, stamina bar.png";
    private const string GeneratedFolder = "Assets/UI/Generated";
    private const string PlayerFramePath = GeneratedFolder + "/PlayerHudFrame.png";
    private const string OpponentFramePath = GeneratedFolder + "/OpponentHudFrame.png";

    [MenuItem("Tools/Stress Strike/Create Combat HUD")]
    public static void CreateCombatHud()
    {
        Sprite playerFrame = BuildFrameSprite(PlayerFramePath, new RectInt(0, 414, 560, 250));
        Sprite opponentFrame = BuildFrameSprite(OpponentFramePath, new RectInt(950, 404, 560, 270));

        GameObject oldHud = GameObject.Find("Combat HUD");
        if (oldHud != null)
        {
            Undo.DestroyObjectImmediate(oldHud);
        }

        GameObject canvasObject = new GameObject("Combat HUD");
        Undo.RegisterCreatedObjectUndo(canvasObject, "Create Combat HUD");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        CombatHudController controller = canvasObject.AddComponent<CombatHudController>();

        CreateHudSide(
            canvasObject.transform,
            "Player HUD",
            playerFrame,
            new Vector2(0f, 1f),
            new Vector2(350f, -128f),
            false,
            out Image playerHealth,
            out Image playerStamina);

        CreateHudSide(
            canvasObject.transform,
            "Opponent HUD",
            opponentFrame,
            new Vector2(1f, 1f),
            new Vector2(-350f, -128f),
            true,
            out Image opponentHealth,
            out Image opponentStamina);

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("playerHealthFill").objectReferenceValue = playerHealth;
        serializedController.FindProperty("playerStaminaFill").objectReferenceValue = playerStamina;
        serializedController.FindProperty("opponentHealthFill").objectReferenceValue = opponentHealth;
        serializedController.FindProperty("opponentStaminaFill").objectReferenceValue = opponentStamina;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
        controller.Refresh();

        EnsureEventSystem();
        Selection.activeGameObject = canvasObject;
        EditorUtility.SetDirty(canvasObject);
    }

    private static void CreateHudSide(
        Transform parent,
        string name,
        Sprite frameSprite,
        Vector2 anchor,
        Vector2 anchoredPosition,
        bool drainsRightToLeft,
        out Image healthFill,
        out Image staminaFill)
    {
        GameObject root = CreateRect(name, parent, anchor, anchor, anchoredPosition, new Vector2(610f, 250f));
        root.transform.localScale = Vector3.one;

        healthFill = CreateFill(
            "Health Fill",
            root.transform,
            new Vector2(0f, 1f),
            new Vector2(72f, -74f),
            new Vector2(420f, 38f),
            new Color(0.95f, 0.06f, 0.04f, 0.95f),
            drainsRightToLeft);

        staminaFill = CreateFill(
            "Stamina Fill",
            root.transform,
            new Vector2(0f, 1f),
            new Vector2(54f, -133f),
            new Vector2(452f, 34f),
            new Color(0.08f, 0.65f, 1f, 0.92f),
            drainsRightToLeft);

        GameObject frameObject = CreateRect("Frame", root.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image frame = frameObject.AddComponent<Image>();
        frame.sprite = frameSprite;
        frame.preserveAspect = true;
        frame.raycastTarget = false;
    }

    private static Image CreateFill(
        string name,
        Transform parent,
        Vector2 anchor,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color,
        bool drainsRightToLeft)
    {
        GameObject fillObject = CreateRect(name, parent, anchor, anchor, anchoredPosition, size);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = color;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = drainsRightToLeft ? (int)Image.OriginHorizontal.Right : (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;
        return fill;
    }

    private static GameObject CreateRect(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent, false);

        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = Vector3.one;

        return gameObject;
    }

    private static Sprite BuildFrameSprite(string outputPath, RectInt crop)
    {
        Texture2D source = LoadReadableSourceTexture();
        Texture2D cropped = new Texture2D(crop.width, crop.height, TextureFormat.RGBA32, false);

        Color32[] pixels = source.GetPixels32();
        Color32[] outputPixels = new Color32[crop.width * crop.height];

        for (int y = 0; y < crop.height; y++)
        {
            for (int x = 0; x < crop.width; x++)
            {
                int sourceIndex = (crop.y + y) * source.width + crop.x + x;
                Color32 pixel = pixels[sourceIndex];
                pixel.a = ShouldKeepPixel(pixel) ? pixel.a : (byte)0;
                outputPixels[y * crop.width + x] = pixel;
            }
        }

        cropped.SetPixels32(outputPixels);
        cropped.Apply();

        if (!AssetDatabase.IsValidFolder(GeneratedFolder))
        {
            AssetDatabase.CreateFolder("Assets/UI", "Generated");
        }

        File.WriteAllBytes(outputPath, cropped.EncodeToPNG());
        AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
        ConfigureSpriteImporter(outputPath);
        return AssetDatabase.LoadAssetAtPath<Sprite>(outputPath);
    }

    private static Texture2D LoadReadableSourceTexture()
    {
        TextureImporter importer = AssetImporter.GetAtPath(SourceHudPath) as TextureImporter;
        if (importer == null)
        {
            throw new FileNotFoundException("Missing HUD source image.", SourceHudPath);
        }

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(SourceHudPath);
        if (texture == null)
        {
            throw new FileNotFoundException("Could not load HUD source texture.", SourceHudPath);
        }

        return texture;
    }

    private static void ConfigureSpriteImporter(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }

    private static bool ShouldKeepPixel(Color32 pixel)
    {
        float red = pixel.r / 255f;
        float green = pixel.g / 255f;
        float blue = pixel.b / 255f;
        float max = Mathf.Max(red, Mathf.Max(green, blue));
        float min = Mathf.Min(red, Mathf.Min(green, blue));
        float saturation = max <= 0f ? 0f : (max - min) / max;
        bool brightOutline = max > 0.82f;
        bool darkFrame = max < 0.28f;
        bool redGlow = red > green * 1.2f && red > blue * 1.2f && red > 0.22f;

        return brightOutline || darkFrame || redGlow || saturation > 0.28f;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
