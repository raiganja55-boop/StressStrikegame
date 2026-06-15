using System.IO;
using UnityEngine;

public class RuntimeCharacterImporter : MonoBehaviour
{
    [Header("3D Model Setup")]
    [Tooltip("Drag the object that has the Mesh Renderer or Skinned Mesh Renderer here.")]
    [SerializeField] private Renderer dummyRenderer; 

    [Tooltip("If the dummy has multiple materials (e.g., 0 for Body, 1 for Face), type the face number here.")]
    [SerializeField] private int faceMaterialIndex = 0;

    public void ImportImageToDummy(string filePath)
    {
        if (!File.Exists(filePath)) return;

        // 1. Read the image file
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        
        // 2. Load the data into the texture
        if (texture.LoadImage(fileData))
        {
            // 3. Apply the texture to the 3D material
            // We use .materials (not .sharedMaterials) so we only change THIS specific dummy
            Material targetMaterial = dummyRenderer.materials[faceMaterialIndex];
            
            // Note: "mainTexture" works for the Standard pipeline. 
            // If you are using URP (Universal Render Pipeline), replace the line below with:
            // targetMaterial.SetTexture("_BaseMap", texture);
            targetMaterial.mainTexture = texture; 
        }
    }
}