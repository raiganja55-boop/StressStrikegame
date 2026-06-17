using UnityEngine;
using UnityEditor;

public class FaceScreenSetup : EditorWindow
{
    [MenuItem("Tools/Setup Face Screen for Dummy")]
    public static void SetupFaceScreen()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select your BoxingDummy in the scene first.", "OK");
            return;
        }

        // Check if there is already a face screen
        Transform existingFace = selectedObj.transform.Find("FaceScreen");
        if (existingFace != null)
        {
            EditorUtility.DisplayDialog("Already Exists", "A FaceScreen already exists on this dummy.", "OK");
            return;
        }

        // 1. Create a Quad to act as the face screen
        GameObject faceScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        faceScreen.name = "FaceScreen";
        
        // 2. Parent it to the dummy
        faceScreen.transform.SetParent(selectedObj.transform);
        
        // 3. Set a default approximate position for a face (can be tweaked manually by user)
        // Adjust these values based on the typical height of your dummy
        faceScreen.transform.localPosition = new Vector3(0, 1.5f, 0.3f); 
        faceScreen.transform.localRotation = Quaternion.identity;
        // Make it roughly face-sized (e.g., 0.3m x 0.3m)
        faceScreen.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        // Remove the mesh collider so it doesn't interfere with physics/boxing
        DestroyImmediate(faceScreen.GetComponent<Collider>());

        // 4. Create and assign a new material for the face screen
        Renderer renderer = faceScreen.GetComponent<Renderer>();
        Material faceMat = new Material(Shader.Find("Standard"));
        
        // Optional: Make it slightly emissive or adjust properties so the image shows well
        faceMat.SetFloat("_Glossiness", 0f); // Make it matte so the image is clear
        
        // Save the material to the project so it persists
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        
        string matPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Materials/FaceScreenMat.mat");
        AssetDatabase.CreateAsset(faceMat, matPath);
        renderer.sharedMaterial = faceMat;

        // 5. Hook it up to the RuntimeCharacterImporter
        RuntimeCharacterImporter importer = selectedObj.GetComponent<RuntimeCharacterImporter>();
        if (importer == null)
        {
            importer = selectedObj.AddComponent<RuntimeCharacterImporter>();
        }

        // Use SerializedObject to modify the private serialized fields
        SerializedObject serializedImporter = new SerializedObject(importer);
        serializedImporter.Update();
        
        SerializedProperty rendererProp = serializedImporter.FindProperty("faceQuadRenderer");
        if (rendererProp != null) rendererProp.objectReferenceValue = renderer;
        
        SerializedProperty indexProp = serializedImporter.FindProperty("faceMaterialIndex");
        if (indexProp != null) indexProp.intValue = 0; // The quad only has 1 material, so index 0

        serializedImporter.ApplyModifiedProperties();

        // 6. Hook it up to ImageUploaderUI if we can find one in the scene
        ImageUploaderUI uploader = FindObjectOfType<ImageUploaderUI>();
        if (uploader != null)
        {
            SerializedObject serializedUploader = new SerializedObject(uploader);
            serializedUploader.Update();
            SerializedProperty importerProp = serializedUploader.FindProperty("characterImporter");
            if (importerProp != null && importerProp.objectReferenceValue == null)
            {
                importerProp.objectReferenceValue = importer;
            }
            serializedUploader.ApplyModifiedProperties();
        }

        // Highlight the new face screen so the user can adjust its position
        Selection.activeGameObject = faceScreen;
        
        Debug.Log("Face Screen created and hooked up successfully! You may need to manually adjust its position/scale to fit your dummy's face exactly.");
    }
}
