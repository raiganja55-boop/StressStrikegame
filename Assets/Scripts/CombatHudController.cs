using UnityEngine;
using UnityEngine.UI;

public class CombatHudController : MonoBehaviour
{
    [Header("Player Bars")]
    [SerializeField] private Image playerHealthFill;
    [SerializeField] private Image playerStaminaFill;

    [Header("Opponent Bars")]
    [SerializeField] private Image opponentHealthFill;
    [SerializeField] private Image opponentStaminaFill;

    [Header("Preview Values")]
    [Range(0f, 1f)] public float playerHealth = 1f;
    [Range(0f, 1f)] public float playerStamina = 1f;
    [Range(0f, 1f)] public float opponentHealth = 1f;
    [Range(0f, 1f)] public float opponentStamina = 1f;

    private void Awake()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }

    public void SetPlayerHealth(float current, float max)
    {
        playerHealth = ToPercent(current, max);
        SetFill(playerHealthFill, playerHealth);
    }

    public void SetPlayerStamina(float current, float max)
    {
        playerStamina = ToPercent(current, max);
        SetFill(playerStaminaFill, playerStamina);
    }

    public void SetOpponentHealth(float current, float max)
    {
        opponentHealth = ToPercent(current, max);
        SetFill(opponentHealthFill, opponentHealth);
    }

    public void SetOpponentStamina(float current, float max)
    {
        opponentStamina = ToPercent(current, max);
        SetFill(opponentStaminaFill, opponentStamina);
    }

    public void Refresh()
    {
        SetFill(playerHealthFill, playerHealth);
        SetFill(playerStaminaFill, playerStamina);
        SetFill(opponentHealthFill, opponentHealth);
        SetFill(opponentStaminaFill, opponentStamina);
    }

    private static float ToPercent(float current, float max)
    {
        if (max <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(current / max);
    }

    private static void SetFill(Image image, float amount)
    {
        if (image == null)
        {
            return;
        }

        image.fillAmount = Mathf.Clamp01(amount);
    }
}
