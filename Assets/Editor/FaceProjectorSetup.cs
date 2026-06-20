using UnityEngine;
using UnityEditor;
#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

public class FaceProjectorSetup : EditorWindow
{
    [MenuItem("Tools/Setup Decal Projector for Dummy")]
    public static void SetupFaceProjector()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select your BoxingDummy in the scene first.", "OK");
            return;
        }

        // Check if there is already a face projector
        Transform existingProjector = selectedObj.transform.Find("FaceProjector");
        if (existingProjector != null)
        {
            EditorUtility.DisplayDialog("Already Exists", "A FaceProjector already exists on this dummy.", "OK");
            return;
        }

        // 1. Create a Projector object
        GameObject faceProjector = new GameObject("FaceProjector");
        
        // Find the Head bone
        Transform headBone = FindHeadBone(selectedObj.transform);
        
        // 2. Set world position to point at the face
        // Assuming dummy faces positive Z, we place projector slightly in front of the face, pointing back at it.
        Vector3 worldPos = selectedObj.transform.position + selectedObj.transform.up * 1.6f + selectedObj.transform.forward * 0.5f;
        Quaternion worldRot = selectedObj.transform.rotation * Quaternion.Euler(0, 180, 0); // Pointing towards the dummy (-Z)
        
        faceProjector.transform.position = worldPos;
        faceProjector.transform.rotation = worldRot;
        
        // 3. Parent it to the dummy's head bone (keeping world transform)
        faceProjector.transform.SetParent(headBone, true);
        
        // 4. Add the Decal Projector component
        UnityEngine.Rendering.Universal.DecalProjector decalProjector = faceProjector.AddComponent<UnityEngine.Rendering.Universal.DecalProjector>();
        decalProjector.size = new Vector3(0.4f, 0.4f, 0.6f); // Width, Height, Depth (Projection distance)
        
        // 5. Create and assign a Decal material
        Shader decalShader = Shader.Find("Shader Graphs/Decal");
        if (decalShader == null) decalShader = Shader.Find("Universal Render Pipeline/Decal");
        
        Material faceMat = new Material(decalShader != null ? decalShader : Shader.Find("Standard"));
        
        // Save the material to the project so it persists
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        
        string matPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Materials/FaceDecalMat.mat");
        AssetDatabase.CreateAsset(faceMat, matPath);
        decalProjector.material = faceMat;

        // 6. Hook it up to the RuntimeCharacterImporter
        RuntimeCharacterImporter importer = selectedObj.GetComponent<RuntimeCharacterImporter>();
        if (importer == null)
        {
            importer = selectedObj.AddComponent<RuntimeCharacterImporter>();
        }

        // Use SerializedObject to modify the private serialized fields
        SerializedObject serializedImporter = new SerializedObject(importer);
        serializedImporter.Update();
        
        SerializedProperty rendererProp = serializedImporter.FindProperty("faceDecalProjector");
        if (rendererProp != null) rendererProp.objectReferenceValue = decalProjector;

        serializedImporter.ApplyModifiedProperties();

        // 7. Hook it up to ImageUploaderUI if we can find one in the scene
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

        // Highlight the new projector so the user can adjust its position
        Selection.activeGameObject = faceProjector;
        
        Debug.Log("Face Projector created and hooked up successfully! Use the Scene view to position and scale the projection box exactly over the dummy's face.");
    }

    private static Transform FindHeadBone(Transform root)
    {
        Animator animator = root.GetComponentInChildren<Animator>();
        if (animator != null && animator.isHuman)
        {
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            if (head != null) return head;
        }

        Transform[] allChildren = root.GetComponentsInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            string lowerName = t.name.ToLower();
            if (lowerName.Contains("head") && !lowerName.Contains("ahead"))
            {
                return t;
            }
        }
        return root;
    }
}
