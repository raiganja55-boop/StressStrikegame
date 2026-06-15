using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJabHash;
    int isHookHash;
    bool isValidSetup = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError($"[animationStateController] No Animator component found on GameObject: '{gameObject.name}'", gameObject);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"[animationStateController] The Animator on '{gameObject.name}' is missing an Animator Controller! Please assign one in the Inspector.", gameObject);
            return;
        }

        isJabHash = Animator.StringToHash("isJab");
        isHookHash = Animator.StringToHash("isHook");
        
        // This confirms everything is set up before allowing Update to run
        isValidSetup = true; 
    }

    void Update()
    {
        // Don't run animation logic if the animator isn't set up properly
        if (!isValidSetup) return;

        bool isJab = animator.GetBool(isJabHash);
        bool isHook = animator.GetBool(isHookHash);
        bool Jpressed = Input.GetKey("j");
        bool Hpressed = Input.GetKey("h");
        
        if (!isJab && Jpressed)
        {
            animator.SetBool(isJabHash, true); // Changed "isJab" to isJabHash for consistency
        }

        if (isJab && !Jpressed)
        {
            animator.SetBool(isJabHash, false); // Changed "isJab" to isJabHash for consistency
        }

        if (!isHook && Hpressed)
        {
            animator.SetBool(isHookHash, true);
        }

        if (isHook && !Hpressed)
        {
            animator.SetBool(isHookHash, false);
        }
    }
}
