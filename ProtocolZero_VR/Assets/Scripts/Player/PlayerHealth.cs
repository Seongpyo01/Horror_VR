using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 플레이어 체력 시스템
/// XR Origin에 부착하여 사용
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    public float maxHealth = 100f;

    [Tooltip("현재 체력")]
    public float currentHealth;

    [Tooltip("무적 시간 (연속 피격 방지)")]
    public float invincibilityDuration = 1f;

    private bool isInvincible = false;

    [Header("체력 회복")]
    [Tooltip("자동 회복 활성화")]
    public bool autoRegeneration = true;

    [Tooltip("회복 속도 (초당)")]
    public float regenerationRate = 5f;

    [Tooltip("회복 시작 대기 시간 (피격 후)")]
    public float regenerationDelay = 3f;

    private float timeSinceLastHit;

    [Header("이펙트")]
    [Tooltip("피격 시 화면 효과")]
    public GameObject damageVignette;

    [Tooltip("피격 사운드")]
    public AudioClip hurtSound;

    [Tooltip("사망 사운드")]
    public AudioClip deathSound;

    private AudioSource audioSource;

    [Header("햅틱 피드백")]
    [Tooltip("피격 시 컨트롤러 진동 강도")]
    public float hapticIntensity = 0.5f;

    [Tooltip("진동 시간")]
    public float hapticDuration = 0.3f;

    [Header("이벤트")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnPlayerDeath;
    public UnityEvent<float> OnPlayerDamaged;

    [Header("VR 컨트롤러 참조")]
    public XRBaseController leftController;
    public XRBaseController rightController;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (damageVignette != null)
        {
            damageVignette.SetActive(false);
        }

        // VR 컨트롤러 자동 찾기
        if (leftController == null || rightController == null)
        {
            FindVRControllers();
        }
    }

    private void Update()
    {
        if (isDead) return;

        // 자동 회복
        if (autoRegeneration && currentHealth < maxHealth)
        {
            timeSinceLastHit += Time.deltaTime;

            if (timeSinceLastHit >= regenerationDelay)
            {
                currentHealth += regenerationRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, maxHealth);

                OnHealthChanged?.Invoke(currentHealth / maxHealth);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        timeSinceLastHit = 0f;

        // 이벤트 호출
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnPlayerDamaged?.Invoke(damage);

        Debug.Log($"Player took {damage} damage. HP: {currentHealth}/{maxHealth}");

        // 이펙트
        PlayDamageEffects();

        // 햅틱 피드백
        TriggerHapticFeedback();

        // 무적 시간
        if (invincibilityDuration > 0)
        {
            StartInvincibility();
        }

        // 사망 체크
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void PlayDamageEffects()
    {
        // 화면 효과
        if (damageVignette != null)
        {
            damageVignette.SetActive(true);
            Invoke(nameof(HideDamageVignette), 0.5f);
        }

        // 사운드
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    private void HideDamageVignette()
    {
        if (damageVignette != null)
        {
            damageVignette.SetActive(false);
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        Invoke(nameof(EndInvincibility), invincibilityDuration);
    }

    private void EndInvincibility()
    {
        isInvincible = false;
    }

    private void TriggerHapticFeedback()
    {
        // 양쪽 컨트롤러 진동
        if (leftController != null)
        {
            leftController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }

        if (rightController != null)
        {
            rightController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player died!");

        // 사운드
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // 이벤트 호출
        OnPlayerDeath?.Invoke();

        // 게임 매니저에 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        Debug.Log($"Player healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        isInvincible = false;
        timeSinceLastHit = 0f;

        OnHealthChanged?.Invoke(1f);
    }

    private void FindVRControllers()
    {
        // XR Origin 내의 컨트롤러 찾기
        XRBaseController[] controllers = GetComponentsInChildren<XRBaseController>();

        foreach (var controller in controllers)
        {
            if (controller.name.Contains("Left"))
            {
                leftController = controller;
            }
            else if (controller.name.Contains("Right"))
            {
                rightController = controller;
            }
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}
