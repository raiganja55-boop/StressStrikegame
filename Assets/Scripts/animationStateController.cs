using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJabHash;
    int isLeftJabHash;
    int isLeftHookHash;
    int isHookHash;
    int isBlockHash;
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
        isLeftJabHash = Animator.StringToHash("isLeftJab");
        isLeftHookHash = Animator.StringToHash("isLeftHook");
        isHookHash = Animator.StringToHash("isHook");
        isBlockHash = Animator.StringToHash("isBlock");
        // This confirms everything is set up before allowing Update to run
        isValidSetup = true; 
    }

    void Update()
    {
        // Don't run animation logic if the animator isn't set up properly
        if (!isValidSetup) return;

        bool isJab = animator.GetBool(isJabHash);
        bool isLeftJab = animator.GetBool(isLeftJabHash);
        bool isLeftHook = animator.GetBool(isLeftHookHash);
        bool isHook = animator.GetBool(isHookHash);
        bool isBlock = animator.GetBool(isBlockHash);
        bool Jpressed = Input.GetKey("j");
        bool Kpressed = Input.GetKey("k");
        bool Lpressed = Input.GetKey("m");
        bool Hpressed = Input.GetKey("h");
        bool Bpressed = Input.GetKey("b");
        
        if (!isJab && Jpressed)
        {
            animator.SetBool(isJabHash, true); // Changed "isJab" to isJabHash for consistency
        }

        if (isJab && !Jpressed)
        {
            animator.SetBool(isJabHash, false); // Changed "isJab" to isJabHash for consistency
        }
//////////////////////////////////////////////////////////////////
        if (!isLeftHook && Kpressed)
        {
            animator.SetBool(isLeftHookHash, true);
        }
        if (isLeftHook && !Kpressed)
        {
            animator.SetBool(isLeftHookHash, false);
        }
///////////////////////////////////////////////////////////////////
        if (!isHook && Hpressed)
        {
            animator.SetBool(isHookHash, true);
        }

        if (isHook && !Hpressed)
        {
            animator.SetBool(isHookHash, false);
        }
////////////////////////////////////////////////////////////////////
        if (!isLeftJab && Lpressed)
        {
            animator.SetBool(isLeftJabHash, true);
        }

        if (isLeftJab && !Lpressed)
        {
            animator.SetBool(isLeftJabHash, false);
        }
/////////////////////////////////////////////////////////////////////
        if (!isBlock && Bpressed)
        {
            animator.SetBool(isBlockHash, true);
        }

        if (isBlock && !Bpressed)
        {
            animator.SetBool(isBlockHash, false);
        }

    }
}
