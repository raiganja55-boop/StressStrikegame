using UnityEngine;
using SFB; 

public class ImageUploaderUI : MonoBehaviour
{
    [Header("Drag your BoxingDummy here")]
    [SerializeField] private RuntimeCharacterImporter characterImporter; 

    // Notice this now says "public"! 
    public void OpenImageBrowser()
    {
        ExtensionFilter[] extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select a Face", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            characterImporter.ImportImageToDummy(paths[0]);
        }
    }
}