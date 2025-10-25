using System.Collections;
using UnityEngine;

/// <summary>
/// 정면에서 탄막을 발사하는 간단한 보스
/// 탄막 슈팅 VR 게임용
/// </summary>
public class ShootingBoss : MonoBehaviour
{
    [Header("보스 설정")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool showHealthBar = true;

    [Header("발사 설정")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform[] firePoints; // 여러 발사 지점
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float bulletLifetime = 5f;

    [Header("패턴 설정")]
    [SerializeField] private BulletPattern currentPattern = BulletPattern.Single;
    [SerializeField] private float fireRate = 1f; // 초당 발사 횟수
    [SerializeField] private int burstCount = 3; // 연발 개수
    [SerializeField] private float burstDelay = 0.1f; // 연발 간격

    [Header("회전 발사 설정")]
    [SerializeField] private int spreadCount = 5; // 부채꼴 발사 개수
    [SerializeField] private float spreadAngle = 30f; // 부채꼴 각도

    private float currentHealth;
    private float nextFireTime;
    private bool isDead = false;

    public enum BulletPattern
    {
        Single,      // 단발
        Burst,       // 연발
        Spread,      // 부채꼴
        Circle       // 원형 (360도)
    }

    private void Awake()
    {
        currentHealth = maxHealth;

        // firePoints가 없으면 자기 자신을 발사 지점으로 사용
        if (firePoints == null || firePoints.Length == 0)
        {
            firePoints = new Transform[] { transform };
        }
    }

    private void Start()
    {
        StartCoroutine(ShootingRoutine());
    }

    private void Update()
    {
        if (isDead) return;

        // 플레이어를 바라보도록 회전 (Y축만)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // 수평 방향만
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
            }
        }
    }

    /// <summary>
    /// 발사 루틴
    /// </summary>
    private IEnumerator ShootingRoutine()
    {
        yield return new WaitForSeconds(1f); // 시작 딜레이

        while (!isDead)
        {
            // 현재 패턴에 따라 발사
            switch (currentPattern)
            {
                case BulletPattern.Single:
                    FireSingle();
                    break;

                case BulletPattern.Burst:
                    yield return StartCoroutine(FireBurst());
                    break;

                case BulletPattern.Spread:
                    FireSpread();
                    break;

                case BulletPattern.Circle:
                    FireCircle();
                    break;
            }

            // 다음 발사까지 대기
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    /// <summary>
    /// 단발 발사
    /// </summary>
    private void FireSingle()
    {
        foreach (Transform firePoint in firePoints)
        {
            CreateBullet(firePoint.position, firePoint.forward);
        }
    }

    /// <summary>
    /// 연발 발사
    /// </summary>
    private IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            FireSingle();
            if (i < burstCount - 1) // 마지막 발사 후엔 대기 안 함
            {
                yield return new WaitForSeconds(burstDelay);
            }
        }
    }

    /// <summary>
    /// 부채꼴 발사
    /// </summary>
    private void FireSpread()
    {
        foreach (Transform firePoint in firePoints)
        {
            float startAngle = -spreadAngle / 2f;
            float angleStep = spreadAngle / (spreadCount - 1);

            for (int i = 0; i < spreadCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
                Vector3 direction = rotation * firePoint.forward;

                CreateBullet(firePoint.position, direction);
            }
        }
    }

    /// <summary>
    /// 원형 발사 (360도)
    /// </summary>
    private void FireCircle()
    {
        foreach (Transform firePoint in firePoints)
        {
            int bulletCount = 12; // 360도를 12등분
            float angleStep = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = angleStep * i;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 direction = rotation * Vector3.forward;

                CreateBullet(firePoint.position, direction);
            }
        }
    }

    /// <summary>
    /// 총알 생성
    /// </summary>
    private void CreateBullet(Vector3 position, Vector3 direction)
    {
        if (enemyBulletPrefab == null)
        {
            Debug.LogWarning("EnemyBullet Prefab이 설정되지 않았습니다!");
            return;
        }

        GameObject bulletObj = Instantiate(enemyBulletPrefab, position, Quaternion.LookRotation(direction));
        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();

        if (bullet != null)
        {
            bullet.Initialize(direction, bulletDamage, bulletSpeed, bulletLifetime);
        }
        else
        {
            Debug.LogError("EnemyBullet 컴포넌트를 찾을 수 없습니다!");
            Destroy(bulletObj);
        }
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // 체력바 업데이트
        if (showHealthBar)
        {
            UpdateHealthBar();
        }

        // 사망 체크
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력바 업데이트
    /// </summary>
    private void UpdateHealthBar()
    {
        HealthBarUI healthBarUI = GetComponentInChildren<HealthBarUI>();
        if (healthBarUI != null)
        {
            //healthBarUI.SetHealth(currentHealth, maxHealth);
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        isDead = true;

        // 모든 코루틴 중지
        StopAllCoroutines();

        // GameManager에 보스 처치 알림
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnBossDefeated();
        }

        // 사망 애니메이션이나 이펙트 추가 가능
        // 예: Destroy(gameObject, 1f);

        Debug.Log($"{gameObject.name} 보스가 처치되었습니다!");

        // 일단 즉시 파괴 (나중에 이펙트 추가 가능)
        Destroy(gameObject);
    }

    /// <summary>
    /// 현재 체력 비율 (0~1)
    /// </summary>
    public float GetHealthRatio()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// 패턴 변경
    /// </summary>
    public void SetPattern(BulletPattern newPattern)
    {
        currentPattern = newPattern;
    }

    /// <summary>
    /// 발사 속도 변경
    /// </summary>
    public void SetFireRate(float newFireRate)
    {
        fireRate = newFireRate;
    }

    /// <summary>
    /// 체력에 따라 패턴 변경 (예시)
    /// </summary>
    private void CheckPhaseTransition()
    {
        float healthRatio = GetHealthRatio();

        // 체력 75% 이하 - 연발 모드
        if (healthRatio <= 0.75f && currentPattern == BulletPattern.Single)
        {
            SetPattern(BulletPattern.Burst);
            Debug.Log("보스 패턴 변경: Burst");
        }
        // 체력 50% 이하 - 부채꼴 모드
        else if (healthRatio <= 0.5f && currentPattern == BulletPattern.Burst)
        {
            SetPattern(BulletPattern.Spread);
            SetFireRate(1.5f);
            Debug.Log("보스 패턴 변경: Spread");
        }
        // 체력 25% 이하 - 원형 모드
        else if (healthRatio <= 0.25f && currentPattern == BulletPattern.Spread)
        {
            SetPattern(BulletPattern.Circle);
            SetFireRate(0.5f);
            Debug.Log("보스 패턴 변경: Circle (최종 페이즈!)");
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 발사 방향 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (firePoints == null || firePoints.Length == 0) return;

        Gizmos.color = Color.red;

        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null) continue;

            // 발사 지점 표시
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);

            // 발사 방향 표시 (패턴에 따라)
            switch (currentPattern)
            {
                case BulletPattern.Single:
                    Gizmos.DrawRay(firePoint.position, firePoint.forward * 2f);
                    break;

                case BulletPattern.Spread:
                    float startAngle = -spreadAngle / 2f;
                    float angleStep = spreadAngle / (spreadCount - 1);
                    for (int i = 0; i < spreadCount; i++)
                    {
                        float currentAngle = startAngle + (angleStep * i);
                        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
                        Vector3 direction = rotation * firePoint.forward;
                        Gizmos.DrawRay(firePoint.position, direction * 2f);
                    }
                    break;
            }
        }
    }
#endif
}
