using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 UI 관리 - 웨이브, 체력, 탄약 등 표시
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("UI 텍스트")]
    public Text waveText;
    public Text scoreText;
    public Text ammoText;
    public Text healthText;
    public Text timerText;

    [Header("헬스바")]
    public Image playerHealthBar;

    [Header("게임 오버 UI")]
    public GameObject gameOverPanel;
    public Text gameOverScoreText;
    public Button restartButton;

    [Header("승리 UI")]
    public GameObject victoryPanel;
    public Text victoryScoreText;

    [Header("참조")]
    public GameManager gameManager;
    public PlayerHealth playerHealth;
    public VRFixedGun[] guns; // 양손 권총 (VRFixedGun으로 변경)

    private void Start()
    {
        // 참조 자동 찾기
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (guns == null || guns.Length == 0)
        {
            guns = FindObjectsOfType<VRFixedGun>();
        }

        // 이벤트 구독
        if (gameManager != null)
        {
            gameManager.OnWaveStart.AddListener(UpdateWave);
            gameManager.OnBossDefeat.AddListener(UpdateBossKills);
            gameManager.OnGameOver.AddListener(ShowGameOver);
            gameManager.OnGameWin.AddListener(ShowGameWin);
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdatePlayerHealth);
        }

        // 초기 UI 숨김
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // 재시작 버튼 설정
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void Update()
    {
        // 탄약 업데이트
        UpdateAmmoDisplay();

        // 타이머 업데이트
        if (gameManager != null && timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameManager.gameTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {wave}";
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void UpdateBossKills(int kills)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Kills: {kills}";
        }
    }

    private void UpdatePlayerHealth(float healthPercentage)
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.fillAmount = healthPercentage;

            // 체력에 따라 색상 변경
            if (healthPercentage > 0.6f)
                playerHealthBar.color = Color.green;
            else if (healthPercentage > 0.3f)
                playerHealthBar.color = Color.yellow;
            else
                playerHealthBar.color = Color.red;
        }

        if (healthText != null && playerHealth != null)
        {
            healthText.text = $"HP: {playerHealth.currentHealth:F0}/{playerHealth.maxHealth:F0}";
        }
    }

    private void UpdateAmmoDisplay()
    {
        if (ammoText == null || guns == null || guns.Length == 0)
            return;

        // 양손 총의 탄약 표시
        if (guns.Length >= 2)
        {
            ammoText.text = $"L: {guns[0].GetAmmoDisplay()} | R: {guns[1].GetAmmoDisplay()}";
        }
        else if (guns.Length == 1)
        {
            ammoText.text = guns[0].GetAmmoDisplay();
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverScoreText != null && gameManager != null)
            {
                gameOverScoreText.text = $"Wave: {gameManager.currentWave}\nBosses Defeated: {gameManager.bossesDefeated}\nTime: {FormatTime(gameManager.gameTime)}";
            }
        }
    }

    private void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            if (victoryScoreText != null && gameManager != null)
            {
                victoryScoreText.text = $"Victory!\nBosses Defeated: {gameManager.bossesDefeated}\nTime: {FormatTime(gameManager.gameTime)}";
            }
        }
    }

    public void ShowGameWin()
    {
        ShowVictory();
    }

    private void RestartGame()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (gameManager != null)
        {
            gameManager.OnWaveStart.RemoveListener(UpdateWave);
            gameManager.OnBossDefeat.RemoveListener(UpdateBossKills);
            gameManager.OnGameOver.RemoveListener(ShowGameOver);
            gameManager.OnGameWin.RemoveListener(ShowGameWin);
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdatePlayerHealth);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }
}
