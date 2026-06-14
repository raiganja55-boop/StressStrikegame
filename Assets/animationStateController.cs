using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJabHash;
    int isHookHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        isJabHash = Animator.StringToHash("isJab");
        isHookHash = Animator.StringToHash("isHook");
    }

    void Update()
    {
        bool isJab = animator.GetBool(isJabHash);
        bool isHook = animator.GetBool(isHookHash);
        bool Jpressed = Input.GetKey("j");
        bool Hpressed = Input.GetKey("h");
        if (!isJab && Jpressed)
        {
            animator.SetBool("isJab", true);
        }

        if (isJab && !Jpressed)
        {
            animator.SetBool("isJab", false);
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
