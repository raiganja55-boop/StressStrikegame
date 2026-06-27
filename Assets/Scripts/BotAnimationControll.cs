using UnityEngine;

public class BotAnimationControll : MonoBehaviour
{
    Animator animator;
    private CombatHudController combatHud;
    
    private float actionTimer = 0f;
    [Header("Difficulty Settings")]
    [Tooltip("Time in seconds between bot attacks. Lower value = faster attacks/harder difficulty.")]
    public float actionInterval = 2f;

    private float freezeTimer = 0f;
    private bool isFrozen = false;

    [Header("Audio Settings")]
    [Tooltip("Array of sound effects to play when the bot hits the player. A random one will be chosen each time.")]
    public AudioClip[] hitSounds;
    private AudioSource audioSource;

    public void FreezeBot(float duration)
    {
        freezeTimer = duration;
        isFrozen = true;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        combatHud = FindObjectOfType<CombatHudController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds != null && hitSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
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
                        if (combatHud != null) combatHud.DrainPlayerHealth(20f);
                        PlayRandomHitSound();
                        break;
                    case 1:
                        if (combatHud != null) combatHud.DrainOpponentStamina(10f);
                        animator.SetTrigger("LeftJabTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(20f);
                        PlayRandomHitSound();
                        break;
                    case 2:
                        if (combatHud != null) combatHud.DrainOpponentStamina(15f);
                        animator.SetTrigger("RightHookTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(25f);
                        PlayRandomHitSound();
                        break;
                    case 3:
                        if (combatHud != null) combatHud.DrainOpponentStamina(15f);
                        animator.SetTrigger("LeftHookTrigger");
                        if (combatHud != null) combatHud.DrainPlayerHealth(25f);
                        PlayRandomHitSound();
                        break;
                }
            }
        }

        if (Input.GetKeyDown("i"))
        {
            animator.SetTrigger("RightJabTrigger"); 
            if (combatHud != null) combatHud.DrainPlayerHealth(20f);
            PlayRandomHitSound();
        }
        if (Input.GetKeyDown("z"))
        {
            animator.SetTrigger("LeftJabTrigger"); 
            if (combatHud != null) combatHud.DrainPlayerHealth(20f);
            PlayRandomHitSound();
        }
    }
}