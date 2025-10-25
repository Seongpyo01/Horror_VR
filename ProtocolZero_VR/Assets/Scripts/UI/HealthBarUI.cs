using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 체력바 UI - 보스나 플레이어의 체력을 표시
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("UI 요소")]
    [Tooltip("체력바 Image (Fill Type: Filled)")]
    public Image healthBarFill;

    [Tooltip("체력 텍스트 (선택사항)")]
    public Text healthText;

    [Header("설정")]
    [Tooltip("카메라를 항상 바라보기")]
    public bool billboardToCamera = true;

    [Tooltip("부드러운 전환")]
    public bool smoothTransition = true;

    [Tooltip("전환 속도")]
    public float transitionSpeed = 5f;

    [Header("색상 설정")]
    public bool useColorGradient = true;
    public Gradient healthGradient;

    private Transform mainCamera;
    private float targetFillAmount = 1f;

    private void Start()
    {
        if (healthBarFill == null)
        {
            healthBarFill = GetComponentInChildren<Image>();
        }

        mainCamera = Camera.main?.transform;

        // 기본 그라데이션 설정
        if (healthGradient == null || healthGradient.colorKeys.Length == 0)
        {
            healthGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0f);
            colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
            colorKeys[2] = new GradientColorKey(Color.green, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);

            healthGradient.SetKeys(colorKeys, alphaKeys);
        }
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
                healthBarFill.color = healthGradient.Evaluate(healthBarFill.fillAmount);
            }
        }
    }

    public void UpdateHealth(float healthPercentage)
    {
        healthPercentage = Mathf.Clamp01(healthPercentage);
        targetFillAmount = healthPercentage;

        if (!smoothTransition && healthBarFill != null)
        {
            healthBarFill.fillAmount = healthPercentage;

            if (useColorGradient)
            {
                healthBarFill.color = healthGradient.Evaluate(healthPercentage);
            }
        }
    }

    public void UpdateHealthWithText(float current, float max)
    {
        float percentage = current / max;
        UpdateHealth(percentage);

        if (healthText != null)
        {
            healthText.text = $"{current:F0} / {max:F0}";
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
