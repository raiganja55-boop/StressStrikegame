using System.IO;
using UnityEngine;

public class RuntimeCharacterImporter : MonoBehaviour
{
    [SerializeField] private Renderer faceQuadRenderer; 

    public void ImportImageToDummy(string filePath)
    {
        // 1. Fallback: Find the FaceScreen if it wasn't assigned in the Inspector
        if (faceQuadRenderer == null)
        {
            // Use our custom deep-search function below
            Transform faceScreen = FindChildRecursive(transform, "FaceScreen");
            
            if (faceScreen != null)
            {
                faceQuadRenderer = faceScreen.GetComponent<Renderer>();
            }
            
            if (faceQuadRenderer == null)
            {
                Debug.LogError("faceQuadRenderer is not assigned and no FaceScreen object was found! Please assign it in the Inspector.");
                return;
            }
        }

        // 2. Read the image file
        if (!File.Exists(filePath)) return;

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        
        // 3. Apply the texture to the material
        if (texture.LoadImage(fileData))
        {
            // The specific command for URP materials
            faceQuadRenderer.material.SetTexture("_BaseMap", texture);
            
            // Standard fallback just in case
            faceQuadRenderer.material.mainTexture = texture; 
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