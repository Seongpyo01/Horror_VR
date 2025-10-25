using UnityEngine;

/// <summary>
/// 플레이어가 발사한 총알
/// - Transform을 직접 이동 (Projectile 방식)
/// - 보스에게 데미지
/// - 적 총알과 충돌 시 둘 다 파괴
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayerBullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float damage = 25f;
    public float speed = 30f;
    public float lifetime = 5f;

    [Header("이펙트")]
    public GameObject hitEffectPrefab;
    public float hitEffectDuration = 1f;

    private Vector3 moveDirection;
    private bool isInitialized = false;
    private Rigidbody rb;

    private void Awake()
    {
        // Rigidbody 설정 (충돌 감지용, 물리 영향 없음)
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // 물리 영향 받지 않음
        rb.useGravity = false;

        // Collider 설정
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        // 초기화되지 않은 경우 기본값으로 발사
        if (!isInitialized)
        {
            Initialize(transform.forward, damage, speed, lifetime);
        }
    }

    /// <summary>
    /// 총알 초기화 및 발사
    /// </summary>
    public void Initialize(Vector3 direction, float bulletDamage, float bulletSpeed, float bulletLifetime)
    {
        moveDirection = direction.normalized;
        damage = bulletDamage;
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        isInitialized = true;

        // 일정 시간 후 자동 파괴
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Transform을 직접 이동 (Projectile 방식)
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 적 총알과 충돌
        EnemyBullet enemyBullet = other.GetComponent<EnemyBullet>();
        if (enemyBullet != null)
        {
            // 둘 다 파괴 (탄막 상쇄)
            CreateHitEffect(transform.position);
            Destroy(enemyBullet.gameObject);
            Destroy(gameObject);
            Debug.Log("[PlayerBullet] Destroyed enemy bullet!");
            return;
        }

        // ShootingBoss와 충돌 (새로운 보스 시스템)
        ShootingBoss shootingBoss = other.GetComponent<ShootingBoss>();
        if (shootingBoss != null)
        {
            shootingBoss.TakeDamage(damage);
            CreateHitEffect(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            Debug.Log($"[PlayerBullet] Hit ShootingBoss for {damage} damage!");
            return;
        }

        // 보스와 충돌 (구버전 BossBase - 하위 호환)
        BossBase boss = other.GetComponent<BossBase>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            CreateHitEffect(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            Debug.Log($"[PlayerBullet] Hit boss for {damage} damage!");
            return;
        }

        // 일반 오브젝트와 충돌 (보스나 적 총알이 아닌 경우)
        // 레이어나 태그로 필터링 가능
    }

    private void CreateHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }
    }
}
