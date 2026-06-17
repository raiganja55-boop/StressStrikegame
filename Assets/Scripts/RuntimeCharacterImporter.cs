using System.IO;
using UnityEngine;

public class RuntimeCharacterImporter : MonoBehaviour
{
    [SerializeField] private Renderer faceQuadRenderer; 
    [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector faceDecalProjector;

    public void ImportImageToDummy(string filePath)
    {
        Debug.Log("ImportImageToDummy called with path: " + filePath);
        
        // 1. Fallback: Find the FaceProjector or FaceScreen if not assigned
        if (faceQuadRenderer == null && faceDecalProjector == null)
        {
            Debug.Log("faceQuadRenderer and faceDecalProjector are null. Searching for FaceProjector...");
            // First try to find a FaceProjector
            Transform faceProjector = FindChildRecursive(transform, "FaceProjector");
            if (faceProjector != null)
            {
                Debug.Log("Found FaceProjector object.");
                faceDecalProjector = faceProjector.GetComponent<UnityEngine.Rendering.Universal.DecalProjector>();
                if (faceDecalProjector == null) 
                {
                    Debug.Log("FaceProjector does not have a DecalProjector. Checking for Renderer...");
                    faceQuadRenderer = faceProjector.GetComponent<Renderer>();
                    if (faceQuadRenderer != null) Debug.Log("Found Renderer on FaceProjector.");
                }
                else
                {
                    Debug.Log("Found DecalProjector on FaceProjector.");
                }
            }
            
            // If still not found, try FaceScreen
            if (faceQuadRenderer == null && faceDecalProjector == null)
            {
                Debug.Log("Searching for FaceScreen object...");
                Transform faceScreen = FindChildRecursive(transform, "FaceScreen");
                if (faceScreen != null)
                {
                    faceQuadRenderer = faceScreen.GetComponent<Renderer>();
                    if (faceQuadRenderer != null) Debug.Log("Found Renderer on FaceScreen.");
                }
            }
            
            if (faceQuadRenderer == null && faceDecalProjector == null)
            {
                Debug.LogError("Neither FaceProjector nor FaceScreen object was found! Please assign it in the Inspector.");
                return;
            }
        }

        // 2. Read the image file
        if (!File.Exists(filePath))
        {
            Debug.LogError("File does not exist: " + filePath);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        
        // 3. Apply the texture to the material
        if (texture.LoadImage(fileData))
        {
            Debug.Log("Successfully loaded image data into texture.");
            
            if (faceDecalProjector != null)
            {
                Debug.Log("Applying texture to DecalProjector...");
                if (faceDecalProjector.material != null)
                {
                    // Instantiate a new material so we don't overwrite the project asset
                    Material instancedMat = new Material(faceDecalProjector.material);
                    // Decal Shader Graphs use Base_Map, standard URP Lit uses _BaseMap
                    instancedMat.SetTexture("Base_Map", texture); 
                    instancedMat.SetTexture("_BaseMap", texture); 
                    
                    faceDecalProjector.material = instancedMat;
                    Debug.Log("Texture applied to DecalProjector material.");
                }
                else
                {
                    Debug.LogError("DecalProjector has no material assigned!");
                }
            }

            if (faceQuadRenderer != null)
            {
                Debug.Log("Applying texture to Renderer...");
                if (faceQuadRenderer.material != null)
                {
                    // The specific command for URP materials
                    faceQuadRenderer.material.SetTexture("_BaseMap", texture);
                    faceQuadRenderer.material.SetTexture("Base_Map", texture);
                    
                    // Standard fallback just in case
                    faceQuadRenderer.material.mainTexture = texture; 
                    Debug.Log("Texture applied to Renderer material.");
                }
                else
                {
                    Debug.LogError("Renderer has no material assigned!");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load image data into texture.");
        }
    }

    /// <summary>
    /// Searches deep into the skeleton to find a child by name.
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child; // We found it!
            }
            
            // If it's not this child, dig deeper into THIS child's children
            Transform result = FindChildRecursive(child, childName);
            if (result != null)
            {
                return result;
            }
        }
        return null; // Not found in this branch
    }
}