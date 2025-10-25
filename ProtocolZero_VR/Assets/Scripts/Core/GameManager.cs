using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 게임 전반을 관리하는 싱글톤 매니저 (간소화 버전)
/// 탄막 슈팅 VR 게임용
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    public int currentWave = 0;
    public int bossesDefeated = 0;
    public float gameTime = 0f;
    public bool isGameActive = false;

    [Header("웨이브 설정")]
    public int maxWaves = 5; // 최대 웨이브 수 (0 = 무한)
    public float timeBetweenWaves = 3f; // 웨이브 사이 대기 시간

    [Header("보스 스폰 설정")]
    public GameObject[] bossPrefabs; // 보스 프리팹 배열
    public Transform bossSpawnPoint; // 보스 스폰 위치
    public bool spawnBossesAutomatically = true; // 자동 스폰 여부

    [Header("이벤트")]
    public UnityEvent<int> OnWaveStart;
    public UnityEvent<int> OnBossDefeat;
    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;
    public UnityEvent OnGameWin;

    [Header("참조")]
    public PlayerHealth playerHealth;
    public GameUI gameUI;

    private GameObject currentBoss;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (isGameActive)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        currentWave = 0;
        bossesDefeated = 0;
        gameTime = 0f;

        OnGameStart?.Invoke();
        StartNextWave();
    }

    /// <summary>
    /// 다음 웨이브 시작
    /// </summary>
    public void StartNextWave()
    {
        // 최대 웨이브 도달 시 게임 클리어
        if (maxWaves > 0 && currentWave >= maxWaves)
        {
            WinGame();
            return;
        }

        currentWave++;
        OnWaveStart?.Invoke(currentWave);

        Debug.Log($"웨이브 {currentWave} 시작!");

        // UI 업데이트
        if (gameUI != null)
        {
            //gameUI.UpdateWave(currentWave);
        }

        // 자동 스폰이 활성화되어 있으면 보스 생성
        if (spawnBossesAutomatically)
        {
            StartCoroutine(SpawnBossAfterDelay());
        }
    }

    /// <summary>
    /// 딜레이 후 보스 스폰
    /// </summary>
    private IEnumerator SpawnBossAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        SpawnBoss();
    }

    /// <summary>
    /// 보스 스폰
    /// </summary>
    public void SpawnBoss()
    {
        // 이미 보스가 존재하면 무시
        if (currentBoss != null)
        {
            Debug.LogWarning("이미 보스가 존재합니다!");
            return;
        }

        // 보스 프리팹이 없으면 에러
        if (bossPrefabs == null || bossPrefabs.Length == 0)
        {
            Debug.LogError("보스 프리팹이 설정되지 않았습니다!");
            return;
        }

        // 스폰 위치 결정
        Vector3 spawnPosition = bossSpawnPoint != null
            ? bossSpawnPoint.position
            : new Vector3(0, 1, 10); // 기본 위치

        Quaternion spawnRotation = bossSpawnPoint != null
            ? bossSpawnPoint.rotation
            : Quaternion.identity;

        // 현재 웨이브에 맞는 보스 선택 (순환)
        int bossIndex = (currentWave - 1) % bossPrefabs.Length;
        GameObject bossPrefab = bossPrefabs[bossIndex];

        // 보스 생성
        currentBoss = Instantiate(bossPrefab, spawnPosition, spawnRotation);
        Debug.Log($"보스 생성: {bossPrefab.name} (웨이브 {currentWave})");
    }

    /// <summary>
    /// 보스가 처치되었을 때 호출 (ShootingBoss.Die()에서 호출)
    /// </summary>
    public void OnBossDefeated()
    {
        bossesDefeated++;
        currentBoss = null;

        OnBossDefeat?.Invoke(bossesDefeated);

        Debug.Log($"보스 처치! 총 {bossesDefeated}마리");

        // UI 업데이트
        if (gameUI != null)
        {
            gameUI.UpdateBossKills(bossesDefeated);
        }

        // 다음 웨이브 시작
        StartNextWave();
    }

    /// <summary>
    /// 게임 오버 (플레이어 사망)
    /// </summary>
    public void GameOver()
    {
        isGameActive = false;

        // 현재 보스 제거
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }

        OnGameOver?.Invoke();

        Debug.Log($"게임 오버! 웨이브: {currentWave}, 처치한 보스: {bossesDefeated}, 플레이 시간: {gameTime:F1}초");

        // UI 업데이트
        if (gameUI != null)
        {
            //gameUI.ShowGameOver();
        }
    }

    /// <summary>
    /// 게임 클리어 (모든 웨이브 완료)
    /// </summary>
    public void WinGame()
    {
        isGameActive = false;
        OnGameWin?.Invoke();

        Debug.Log($"게임 클리어! 총 {bossesDefeated}마리의 보스 처치, 플레이 시간: {gameTime:F1}초");

        // UI 업데이트
        if (gameUI != null)
        {
            gameUI.ShowGameWin();
        }
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        // 현재 보스 제거
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }

        // 플레이어 체력 회복
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // 게임 재시작
        StartGame();
    }

    /// <summary>
    /// 현재 보스가 존재하는지 확인
    /// </summary>
    public bool HasActiveBoss()
    {
        return currentBoss != null;
    }

    /// <summary>
    /// 현재 보스 가져오기
    /// </summary>
    public GameObject GetCurrentBoss()
    {
        return currentBoss;
    }
}
