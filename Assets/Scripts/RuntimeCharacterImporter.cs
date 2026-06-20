using System.IO;
using UnityEngine;

public class RuntimeCharacterImporter : MonoBehaviour
{
    [SerializeField] private Renderer faceQuadRenderer; 
    [SerializeField] private UnityEngine.Rendering.Universal.DecalProjector faceDecalProjector;

    // Material adjustment properties
    private float currentZoom = 1f;
    private float currentOffsetX = 0f;
    private float currentOffsetY = 0f;

    private Vector3 initialDecalLocalPos;
    private Vector3 initialDecalSize;
    private Vector3 initialQuadLocalPos;
    private Vector3 initialQuadLocalScale;
    private bool baseStateStored = false;

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

        // Apply any existing zoom/offset right after importing a new image
        ApplyTransformToFaceObject();
    }

    public void SetFaceZoom(float zoom)
    {
        currentZoom = Mathf.Max(0.1f, zoom); // Prevent division by zero or negative zoom
        ApplyTransformToFaceObject();
    }

    public void SetFaceOffsetX(float offsetX)
    {
        currentOffsetX = offsetX;
        ApplyTransformToFaceObject();
    }

    public void SetFaceOffsetY(float offsetY)
    {
        currentOffsetY = offsetY;
        ApplyTransformToFaceObject();
    }

    private void StoreBaseState()
    {
        if (baseStateStored) return;

        if (faceDecalProjector != null)
        {
            initialDecalLocalPos = faceDecalProjector.transform.localPosition;
            initialDecalSize = faceDecalProjector.size;
        }

        if (faceQuadRenderer != null)
        {
            initialQuadLocalPos = faceQuadRenderer.transform.localPosition;
            initialQuadLocalScale = faceQuadRenderer.transform.localScale;
        }

        baseStateStored = true;
    }

    private void ApplyTransformToFaceObject()
    {
        StoreBaseState();

        if (faceDecalProjector != null)
        {
            // Zoom changes the physical size of the projection box
            Vector3 newSize = initialDecalSize;
            newSize.x *= currentZoom;
            newSize.y *= currentZoom;
            faceDecalProjector.size = newSize;

            // Reset local position first
            faceDecalProjector.transform.localPosition = initialDecalLocalPos;
            
            // Calculate world offset based on the projector's right and up vectors
            // Negative to make slider movement intuitive (moving slider right moves image right)
            Vector3 worldOffset = faceDecalProjector.transform.right * (-currentOffsetX) + 
                                  faceDecalProjector.transform.up * (-currentOffsetY);
                                  
            // Apply world offset
            faceDecalProjector.transform.position += worldOffset;
        }

        if (faceQuadRenderer != null)
        {
            // Zoom changes the quad scale
            Vector3 newScale = initialQuadLocalScale;
            newScale.x *= currentZoom;
            newScale.y *= currentZoom;
            faceQuadRenderer.transform.localScale = newScale;

            // Reset local position first
            faceQuadRenderer.transform.localPosition = initialQuadLocalPos;

            // Calculate world offset
            Vector3 worldOffset = faceQuadRenderer.transform.right * (-currentOffsetX) + 
                                  faceQuadRenderer.transform.up * (-currentOffsetY);
                                  
            // Apply world offset
            faceQuadRenderer.transform.position += worldOffset;
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