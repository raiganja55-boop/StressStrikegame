using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;

public class CombatHudController : MonoBehaviour
{
    [Header("UI Text & Portraits")]
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _opponentNameText;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private Image _playerPortraitImage;
    [SerializeField] private Image _opponentPortraitImage;

    [Header("Opponent Icons (Assign in Inspector)")]
    [SerializeField] private Sprite _damienIcon;
    [SerializeField] private Sprite _brockIcon;
    [SerializeField] private Sprite _silasIcon;

    [Header("Player Health")]
    [SerializeField] private Image _playerHealthFillImage;
    [SerializeField] private Image _playerHealthTrailingFillImage;

    [Header("Player Stamina")]
    [SerializeField] private Image _playerStaminaFillImage;
    [SerializeField] private Image _playerStaminaTrailingFillImage;

    [Header("Opponent Health")]
    [SerializeField] private Image _opponentHealthFillImage;
    [SerializeField] private Image _opponentHealthTrailingFillImage;

    [Header("Opponent Stamina")]
    [SerializeField] private Image _opponentStaminaFillImage;
    [SerializeField] private Image _opponentStaminaTrailingFillImage;

    [Header("Tween Settings")]
    [SerializeField] private float _trailDelay = 0.4f;
    [SerializeField] private float _fillDuration = 0.25f;
    [SerializeField] private float _trailDuration = 0.3f;

    [Header("Player Stats")]
    [SerializeField] private float _playerMaxHealth = 100f;
    [SerializeField] private float _playerMaxStamina = 100f;

    [Header("Opponent Stats")]
    [SerializeField] private float _opponentMaxHealth = 100f;
    [SerializeField] private float _opponentMaxStamina = 100f;

    private float _playerCurrentHealth;
    private float _playerCurrentStamina;
    private float _opponentCurrentHealth;
    private float _opponentCurrentStamina;

    private int _currentStage = 1;
    private bool _isCleared = false;

    // Track active tweens so we can kill them before starting new ones
    private Sequence _playerHealthSequence;
    private Sequence _playerStaminaSequence;
    private Sequence _opponentHealthSequence;
    private Sequence _opponentStaminaSequence;

    private void Awake()
    {
        // Force max health to 100 to prevent unbalanced inspector settings
        _playerMaxHealth = 100f;
        _playerMaxStamina = 100f;
        _opponentMaxHealth = 100f;
        _opponentMaxStamina = 100f;

        _playerCurrentHealth = _playerMaxHealth;
        _playerCurrentStamina = _playerMaxStamina;
        _opponentCurrentHealth = _opponentMaxHealth;
        _opponentCurrentStamina = _opponentMaxStamina;

        SetFillImmediate(_playerHealthFillImage, 1f);
        SetFillImmediate(_playerHealthTrailingFillImage, 1f);
        SetFillImmediate(_playerStaminaFillImage, 1f);
        SetFillImmediate(_playerStaminaTrailingFillImage, 1f);
        SetFillImmediate(_opponentHealthFillImage, 1f);
        SetFillImmediate(_opponentHealthTrailingFillImage, 1f);
        SetFillImmediate(_opponentStaminaFillImage, 1f);
        SetFillImmediate(_opponentStaminaTrailingFillImage, 1f);

        UpdateRoundText();
        UpdateOpponentProfile();
    }

    private void Update()
    {
        // Debug controls — remove these when hooking up real gameplay
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DrainPlayerHealth(10f);
        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            DrainPlayerStamina(15f);
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            DrainOpponentHealth(10f);
        }

        if (Keyboard.current.rightShiftKey.wasPressedThisFrame)
        {
            DrainOpponentStamina(15f);
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            AdvanceRound();
        }
    }

    // ─────────────────────────────────────────────
    // Public API — Call these from gameplay scripts
    // ─────────────────────────────────────────────

    /// <summary>
    /// Advance the progression: Stage 1 -> Clear -> Stage 2 -> Clear -> Final Stage -> All Clear
    /// </summary>
    public void AdvanceRound()
    {
        if (!_isCleared)
        {
            _isCleared = true; // Beat the current stage
        }
        else
        {
            if (_currentStage < 3)
            {
                _isCleared = false; // Move to next stage
                _currentStage++;
            }
        }
        
        UpdateRoundText();
        UpdateOpponentProfile();
        
        // Optional: Animate the text when it changes
        if (_roundText != null)
        {
            _roundText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1f);
        }
    }

    private void UpdateRoundText()
    {
        if (_roundText == null) return;

        if (_isCleared)
        {
            if (_currentStage >= 3)
            {
                _roundText.text = "ALL CLEAR!";
                _roundText.fontSize = 60; // Make it fit better
            }
            else
            {
                _roundText.text = "CLEAR!";
                _roundText.fontSize = 72; 
            }
        }
        else
        {
            switch (_currentStage)
            {
                case 1:
                    _roundText.text = "--VS--";
                    _roundText.fontSize = 72;
                    break;
                case 2:
                    _roundText.text = "--VS--";
                    _roundText.fontSize = 72;
                    break;
                case 3:
                    _roundText.text = "--VS--";
                    _roundText.fontSize = 72;
                    break;
            }
        }
    }

    private void UpdateOpponentProfile()
    {
        if (_opponentNameText == null) return;
        
        switch (_currentStage)
        {
            case 1:
                _opponentNameText.text = "Damien";
                if (_opponentPortraitImage != null) _opponentPortraitImage.sprite = _damienIcon;
                break;
            case 2:
                _opponentNameText.text = "Brock";
                if (_opponentPortraitImage != null) _opponentPortraitImage.sprite = _brockIcon;
                break;
            case 3:
                _opponentNameText.text = "Silas";
                if (_opponentPortraitImage != null) _opponentPortraitImage.sprite = _silasIcon;
                break;
        }
    }

    /// <summary>
    /// Drain the player's health bar by the given amount.
    /// </summary>
    public void DrainPlayerHealth(float amount)
    {
        _playerCurrentHealth = Mathf.Max(_playerCurrentHealth - amount, 0f);
        float ratio = _playerCurrentHealth / _playerMaxHealth;
        AnimateBar(ref _playerHealthSequence, _playerHealthFillImage, _playerHealthTrailingFillImage, ratio);
    }

    /// <summary>
    /// Drain the player's stamina bar by the given amount.
    /// </summary>
    public void DrainPlayerStamina(float amount)
    {
        _playerCurrentStamina = Mathf.Max(_playerCurrentStamina - amount, 0f);
        float ratio = _playerCurrentStamina / _playerMaxStamina;
        AnimateBar(ref _playerStaminaSequence, _playerStaminaFillImage, _playerStaminaTrailingFillImage, ratio);
    }

    /// <summary>
    /// Drain the opponent's health bar by the given amount.
    /// </summary>
    public void DrainOpponentHealth(float amount)
    {
        _opponentCurrentHealth = Mathf.Max(_opponentCurrentHealth - amount, 0f);
        float ratio = _opponentCurrentHealth / _opponentMaxHealth;
        AnimateBar(ref _opponentHealthSequence, _opponentHealthFillImage, _opponentHealthTrailingFillImage, ratio);
    }

    /// <summary>
    /// Drain the opponent's stamina bar by the given amount.
    /// </summary>
    public void DrainOpponentStamina(float amount)
    {
        _opponentCurrentStamina = Mathf.Max(_opponentCurrentStamina - amount, 0f);
        float ratio = _opponentCurrentStamina / _opponentMaxStamina;
        AnimateBar(ref _opponentStaminaSequence, _opponentStaminaFillImage, _opponentStaminaTrailingFillImage, ratio);
    }

    /// <summary>
    /// Restore the player's health bar by the given amount.
    /// </summary>
    public void RestorePlayerHealth(float amount)
    {
        _playerCurrentHealth = Mathf.Min(_playerCurrentHealth + amount, _playerMaxHealth);
        float ratio = _playerCurrentHealth / _playerMaxHealth;
        AnimateBar(ref _playerHealthSequence, _playerHealthFillImage, _playerHealthTrailingFillImage, ratio);
    }

    /// <summary>
    /// Restore the player's stamina bar by the given amount.
    /// </summary>
    public void RestorePlayerStamina(float amount)
    {
        _playerCurrentStamina = Mathf.Min(_playerCurrentStamina + amount, _playerMaxStamina);
        float ratio = _playerCurrentStamina / _playerMaxStamina;
        AnimateBar(ref _playerStaminaSequence, _playerStaminaFillImage, _playerStaminaTrailingFillImage, ratio);
    }

    /// <summary>
    /// Restore the opponent's health bar by the given amount.
    /// </summary>
    public void RestoreOpponentHealth(float amount)
    {
        _opponentCurrentHealth = Mathf.Min(_opponentCurrentHealth + amount, _opponentMaxHealth);
        float ratio = _opponentCurrentHealth / _opponentMaxHealth;
        AnimateBar(ref _opponentHealthSequence, _opponentHealthFillImage, _opponentHealthTrailingFillImage, ratio);
    }

    /// <summary>
    /// Restore the opponent's stamina bar by the given amount.
    /// </summary>
    public void RestoreOpponentStamina(float amount)
    {
        _opponentCurrentStamina = Mathf.Min(_opponentCurrentStamina + amount, _opponentMaxStamina);
        float ratio = _opponentCurrentStamina / _opponentMaxStamina;
        AnimateBar(ref _opponentStaminaSequence, _opponentStaminaFillImage, _opponentStaminaTrailingFillImage, ratio);
    }

    // ─────────────────────────────────────────────
    // Internal tweening
    // ─────────────────────────────────────────────

    private void AnimateBar(ref Sequence activeSequence, Image fillImage, Image trailingFillImage, float targetRatio)
    {
        // Kill any running tween on this bar so rapid hits don't stack weirdly
        activeSequence?.Kill();

        Sequence sequence = DOTween.Sequence();

        // Main fill snaps down quickly
        if (fillImage != null)
        {
            sequence.Append(fillImage.DOFillAmount(targetRatio, _fillDuration)
                .SetEase(Ease.InOutSine));
        }

        // Trailing fill follows after a delay
        if (trailingFillImage != null)
        {
            sequence.AppendInterval(_trailDelay);
            sequence.Append(trailingFillImage.DOFillAmount(targetRatio, _trailDuration)
                .SetEase(Ease.InOutSine));
        }

        sequence.Play();
        activeSequence = sequence;
    }

    private static void SetFillImmediate(Image image, float amount)
    {
        if (image != null)
        {
            image.fillAmount = Mathf.Clamp01(amount);
        }
    }

    private void OnDestroy()
    {
        // Clean up all tweens when this object is destroyed
        _playerHealthSequence?.Kill();
        _playerStaminaSequence?.Kill();
        _opponentHealthSequence?.Kill();
        _opponentStaminaSequence?.Kill();
    }
}
