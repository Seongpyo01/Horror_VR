using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 보스 전용 체력바 UI
/// </summary>
public class BossHealthBar : MonoBehaviour
{
    [Header("UI 요소")]
    [Tooltip("체력바 Image (Fill Type: Filled)")]
    public Image healthBarFill;

    [Tooltip("보스 이름 텍스트")]
    public Text bossNameText;
    public TextMeshProUGUI bossNameTextTMP;

    [Tooltip("체력 텍스트")]
    public Text healthText;
    public TextMeshProUGUI healthTextTMP;

    [Header("설정")]
    [Tooltip("카메라를 항상 바라보기")]
    public bool billboardToCamera = true;

    [Tooltip("부드러운 전환")]
    public bool smoothTransition = true;

    [Tooltip("전환 속도")]
    public float transitionSpeed = 5f;

    [Header("색상 설정")]
    public bool useColorGradient = true;
    public Color fullHealthColor = Color.green;
    public Color halfHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    private Transform mainCamera;
    private float targetFillAmount = 1f;
    private float maxHealth;
    private float currentHealth;

    private void Start()
    {
        if (healthBarFill == null)
        {
            healthBarFill = GetComponentInChildren<Image>();
        }

        mainCamera = Camera.main?.transform;
    }

    private void Update()
    {
        // 빌보드 (카메라를 향하도록)
        if (billboardToCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.forward);
        }

        // 부드러운 체력바 전환
        if (smoothTransition && healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);

            // 색상 업데이트
            if (useColorGradient)
            {
                healthBarFill.color = GetHealthColor(healthBarFill.fillAmount);
            }
        }
    }

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = max;
        SetHealth(max);
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        float percentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        percentage = Mathf.Clamp01(percentage);

        targetFillAmount = percentage;

        if (!smoothTransition && healthBarFill != null)
        {
            healthBarFill.fillAmount = percentage;

            if (useColorGradient)
            {
                healthBarFill.color = GetHealthColor(percentage);
            }
        }

        // 체력 텍스트 업데이트
        UpdateHealthText();
    }

    public void SetBossName(string name)
    {
        if (bossNameText != null)
        {
            bossNameText.text = name;
        }

        if (bossNameTextTMP != null)
        {
            bossNameTextTMP.text = name;
        }
    }

    private void UpdateHealthText()
    {
        string text = $"{currentHealth:F0} / {maxHealth:F0}";

        if (healthText != null)
        {
            healthText.text = text;
        }

        if (healthTextTMP != null)
        {
            healthTextTMP.text = text;
        }
    }

    private Color GetHealthColor(float percentage)
    {
        if (percentage > 0.5f)
        {
            // 초록 -> 노랑
            return Color.Lerp(halfHealthColor, fullHealthColor, (percentage - 0.5f) * 2f);
        }
        else
        {
            // 빨강 -> 노랑
            return Color.Lerp(lowHealthColor, halfHealthColor, percentage * 2f);
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void Show()
    {
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }
}
