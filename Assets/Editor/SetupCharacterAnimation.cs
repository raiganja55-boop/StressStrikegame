using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SetupCharacterAnimation
{
    [MenuItem("StressStrike/Setup Character Animation")]
    public static void Setup()
    {
        // Find Character by partial name in case it's named "characterMedium (1)" or similar
        GameObject character = null;
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            if (go.name.ToLower().Contains("charactermedium"))
            {
                character = go;
                break;
            }
        }

        if (character == null)
        {
            Debug.LogError("Could not find CharacterMedium in the scene. Please ensure it is in the scene.");
            return;
        }

        // Add Animator if needed
        Animator animator = character.GetComponent<Animator>();
        if (animator == null)
        {
            animator = character.AddComponent<Animator>();
        }

        // Create or load Animator Controller
        string controllerDir = "Assets/Animations";
        if (!AssetDatabase.IsValidFolder(controllerDir))
        {
            AssetDatabase.CreateFolder("Assets", "Animations");
        }

        string controllerPath = "Assets/Animations/CharacterController.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        }

        // Load Idle Animation
        string idlePath = "Assets/Animations/idle.fbx";
        AnimationClip idleClip = null;
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(idlePath);
        foreach (Object asset in assets)
        {
            if (asset is AnimationClip && !asset.name.StartsWith("__preview__") && !asset.name.Contains("Take 001"))
            {
                idleClip = asset as AnimationClip;
                break;
            }
            // fallback
            if (asset is AnimationClip && idleClip == null && !asset.name.StartsWith("__preview__"))
            {
                idleClip = asset as AnimationClip;
            }
        }

        if (idleClip == null)
        {
            Debug.LogError("Could not find AnimationClip in " + idlePath);
            return;
        }

        // Add state
        if (controller.layers.Length > 0)
        {
            var stateMachine = controller.layers[0].stateMachine;
            if (stateMachine.states.Length == 0)
            {
                AnimatorState state = stateMachine.AddState("Idle");
                state.motion = idleClip;
                stateMachine.defaultState = state;
            }
            else
            {
                stateMachine.states[0].state.motion = idleClip;
            }
        }

        // Assign controller
        animator.runtimeAnimatorController = controller;
        
        // Ensure Avatar is set
        if (animator.avatar == null)
        {
            string modelPath = "Assets/Model/characterMedium.fbx";
            GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (modelPrefab != null)
            {
                Animator prefabAnimator = modelPrefab.GetComponent<Animator>();
                if (prefabAnimator != null)
                {
                    animator.avatar = prefabAnimator.avatar;
                }
            }
        }

        // Mark scene as dirty so the user can save it
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(character.scene);

        Debug.Log("Successfully set up idle animation for " + character.name + "!", character);
    }
}
