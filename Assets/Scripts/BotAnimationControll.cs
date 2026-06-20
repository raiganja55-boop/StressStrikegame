using UnityEngine;

public class BotAnimationControll : MonoBehaviour
{
    Animator animator;
    private CombatHudController combatHud;
    
    private float actionTimer = 0f;
    private float actionInterval = 2f;

    private float freezeTimer = 0f;
    private bool isFrozen = false;

    public void FreezeBot(float duration)
    {
        freezeTimer = duration;
        isFrozen = true;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        combatHud = FindObjectOfType<CombatHudController>();
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
            }
            return; // Skip attack logic while frozen
        }

        // Random action every 5 seconds
        actionTimer += Time.deltaTime;
        if (actionTimer >= actionInterval)
        {
            // If the opponent is exhausted, wait until stamina is fully refilled.
            if (combatHud != null && combatHud.IsOpponentExhausted)
            {
                // Cap the timer so it triggers immediately when exhaustion ends.
                actionTimer = actionInterval; 
            }
            else
            {
                actionTimer = 0f;
                int randomAction = Random.Range(0, 4);
                
                switch (randomAction)
                {
                    case 0:
                        if (combatHud != null) combatHud.DrainOpponentStamina(10f);
                        animator.SetTrigger("RightJabTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(10f);
                        break;
                    case 1:
                        if (combatHud != null) combatHud.DrainOpponentStamina(10f);
                        animator.SetTrigger("LeftJabTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(10f);
                        break;
                    case 2:
                        if (combatHud != null) combatHud.DrainOpponentStamina(15f);
                        animator.SetTrigger("RightHookTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(15f);
                        break;
                    case 3:
                        if (combatHud != null) combatHud.DrainOpponentStamina(15f);
                        animator.SetTrigger("LeftHookTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(15f);
                        break;
                }
            }
        }

        if (Input.GetKeyDown("i"))
        {
            animator.SetTrigger("RightJabTrigger"); 
            if (combatHud != null) combatHud.DrainPlayerHealth(10f);
        }
        if (Input.GetKeyDown("z"))
        {
            animator.SetTrigger("LeftJabTrigger"); 
            if (combatHud != null) combatHud.DrainPlayerHealth(10f);
        }
    }
}