using UnityEngine;

public class BotAnimationControll : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("i"))
        {
            animator.SetBool("isBotRightJab", true);
        }
        if (!Input.GetKey("i"))
        {
            animator.SetBool("isBotRightJab", false);
        }
        
    }
}
