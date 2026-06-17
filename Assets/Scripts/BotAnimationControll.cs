using UnityEngine;

public class BotAnimationControll : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            animator.SetTrigger("RightJabTrigger"); 
        }
    }
}