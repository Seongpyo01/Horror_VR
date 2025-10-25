using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 스폰 시스템
/// 랜덤하게 또는 순차적으로 보스를 생성
/// </summary>
public class BossSpawner : MonoBehaviour
{
    [Header("보스 프리팹")]
    [Tooltip("보스 프리팹 배열 (MeleeBoss, RangedBoss, HybridBoss)")]
    public GameObject[] bossPrefabs;

    [Header("스폰 위치")]
    [Tooltip("보스 스폰 위치 배열")]
    public Transform[] spawnPoints;

    [Tooltip("스폰 위치가 없으면 이 위치에서 스폰")]
    public Vector3 defaultSpawnOffset = new Vector3(0, 0, 20f);

    [Header("스폰 설정")]
    [Tooltip("랜덤 보스 선택 (false면 순차적)")]
    public bool randomBossSelection = true;

    [Tooltip("랜덤 스폰 위치 (false면 순차적)")]
    public bool randomSpawnPoint = true;

    [Tooltip("스폰 이펙트")]
    public GameObject spawnEffectPrefab;

    [Tooltip("스폰 사운드")]
    public AudioClip spawnSound;

    private int currentBossIndex = 0;
    private int currentSpawnIndex = 0;
    private Transform player;
    private AudioSource audioSource;
    private GameObject currentBoss;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
    }

    private void Start()
    {
        // 플레이어 찾기
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            player = mainCam.transform;
        }

        // 스폰 포인트 유효성 검사
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("BossSpawner: Spawn points가 설정되지 않았습니다. 기본 위치를 사용합니다.");
        }

        // 보스 프리팹 유효성 검사
        if (bossPrefabs == null || bossPrefabs.Length == 0)
        {
            Debug.LogError("BossSpawner: Boss prefabs가 설정되지 않았습니다!");
        }
    }

    public void SpawnRandomBoss()
    {
        if (bossPrefabs == null || bossPrefabs.Length == 0)
        {
            Debug.LogError("BossSpawner: Cannot spawn boss - no prefabs available!");
            return;
        }

        GameObject bossPrefab = GetNextBossPrefab();
        Vector3 spawnPosition = GetNextSpawnPosition();

        SpawnBoss(bossPrefab, spawnPosition);
    }

    public void SpawnSpecificBoss(int bossIndex)
    {
        if (bossPrefabs == null || bossIndex < 0 || bossIndex >= bossPrefabs.Length)
        {
            Debug.LogError($"BossSpawner: Invalid boss index {bossIndex}");
            return;
        }

        Vector3 spawnPosition = GetNextSpawnPosition();
        SpawnBoss(bossPrefabs[bossIndex], spawnPosition);
    }

    private void SpawnBoss(GameObject bossPrefab, Vector3 position)
    {
        if (bossPrefab == null)
        {
            Debug.LogError("BossSpawner: Boss prefab is null!");
            return;
        }

        // 이전 보스가 아직 있으면 제거
        if (currentBoss != null)
        {
            Destroy(currentBoss);
        }

        // 스폰 이펙트
        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 3f);
        }

        // 사운드
        if (audioSource != null && spawnSound != null)
        {
            audioSource.transform.position = position;
            audioSource.PlayOneShot(spawnSound);
        }

        // 보스 생성
        currentBoss = Instantiate(bossPrefab, position, Quaternion.identity);

        Debug.Log($"Spawned {bossPrefab.name} at {position}");
    }

    private GameObject GetNextBossPrefab()
    {
        if (randomBossSelection)
        {
            return bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        }
        else
        {
            GameObject boss = bossPrefabs[currentBossIndex];
            currentBossIndex = (currentBossIndex + 1) % bossPrefabs.Length;
            return boss;
        }
    }

    private Vector3 GetNextSpawnPosition()
    {
        // 스폰 포인트가 있으면 사용
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint;

            if (randomSpawnPoint)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            }
            else
            {
                spawnPoint = spawnPoints[currentSpawnIndex];
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
            }

            return spawnPoint.position;
        }

        // 스폰 포인트가 없으면 플레이어 앞에 생성
        if (player != null)
        {
            return player.position + player.forward * defaultSpawnOffset.z + defaultSpawnOffset;
        }

        // 플레이어도 없으면 기본 위치
        return transform.position + defaultSpawnOffset;
    }

    // 외부에서 현재 보스 가져오기
    public GameObject GetCurrentBoss()
    {
        return currentBoss;
    }

    // 디버그용: 즉시 특정 보스 스폰
    [ContextMenu("Spawn Melee Boss")]
    public void DebugSpawnMeleeBoss()
    {
        SpawnSpecificBoss(0);
    }

    [ContextMenu("Spawn Ranged Boss")]
    public void DebugSpawnRangedBoss()
    {
        SpawnSpecificBoss(1);
    }

    [ContextMenu("Spawn Hybrid Boss")]
    public void DebugSpawnHybridBoss()
    {
        SpawnSpecificBoss(2);
    }

    private void OnDrawGizmos()
    {
        // 스폰 포인트 시각화
        if (spawnPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 1f);
                    Gizmos.DrawLine(point.position, point.position + Vector3.up * 2f);
                }
            }
        }
    }
}
