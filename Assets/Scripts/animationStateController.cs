using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJabHash;
    int isLeftJabHash;
    int isLeftHookHash;
    int isHookHash;
    int isBlockHash;
    int isLeftBlockHash;

    private CombatHudController combatHud;

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
        isLeftBlockHash = Animator.StringToHash("isLeftBlock");
        
        // This confirms everything is set up before allowing Update to run
        combatHud = FindObjectOfType<CombatHudController>();
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
        bool isLeftBlock = animator.GetBool(isLeftBlockHash);
        
        bool canAct = combatHud == null || !combatHud.IsPlayerExhausted;

        bool Jpressed = Input.GetKey(KeyCode.J) && canAct;
        bool Kpressed = Input.GetKey(KeyCode.K) && canAct;
        bool Lpressed = Input.GetKey(KeyCode.M) && canAct;
        bool Hpressed = Input.GetKey(KeyCode.H) && canAct;
        
        // Blocking does not cost stamina and is not prevented by exhaustion
        bool Bpressed = Input.GetKey(KeyCode.B);
        bool Npressed = Input.GetKey(KeyCode.N);
        
        float jabStaminaCost = 1f;
        float hookStaminaCost = 2f;

        if (!isJab && Jpressed)
        {
            animator.SetBool(isJabHash, true); // Changed "isJab" to isJabHash for consistency
        }

        if (Input.GetKeyDown(KeyCode.J) && canAct)
        {
            if (combatHud != null) combatHud.DrainPlayerStamina(jabStaminaCost);
            if (combatHud != null) combatHud.DrainOpponentHealth(1.5f);
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
        
        if (Input.GetKeyDown(KeyCode.K) && canAct)
        {
            if (combatHud != null) combatHud.DrainPlayerStamina(hookStaminaCost);
            if (combatHud != null) combatHud.DrainOpponentHealth(1.5f); // Example damage
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

        if (Input.GetKeyDown(KeyCode.H) && canAct)
        {
            if (combatHud != null) combatHud.DrainPlayerStamina(hookStaminaCost);
            if (combatHud != null) combatHud.DrainOpponentHealth(1.5f);
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

        if (Input.GetKeyDown(KeyCode.M) && canAct)
        {
            if (combatHud != null) combatHud.DrainPlayerStamina(jabStaminaCost);
            if (combatHud != null) combatHud.DrainOpponentHealth(1.5f);
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

        if (!isLeftBlock && Npressed)
        {
            animator.SetBool(isLeftBlockHash, true);
        }

        if (isLeftBlock && !Npressed)
        {
            animator.SetBool(isLeftBlockHash, false);
        }
////////////////////////////////////////////////////////////////
        
        ////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.P) && canAct)
        {
            animator.SetTrigger("isSpecial");
            animator.SetTrigger("isLeftSpecial");
            
            if (combatHud != null) combatHud.DrainPlayerStamina(3f); // Special costs more
            
            BotAnimationControll botController = FindObjectOfType<BotAnimationControll>();
            if (botController != null)
            {
                botController.FreezeBot(10f);
            }
        }
    }
}
