using UnityEngine;

/// <summary>
/// 보스/적이 발사한 총알
/// - Transform을 직접 이동 (Projectile 방식)
/// - 플레이어에게 데미지
/// - 플레이어 총알과 충돌 시 둘 다 파괴
/// </summary>
[RequireComponent(typeof(Collider))]
public class EnemyBullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float damage = 10f;
    public float speed = 15f;
    public float lifetime = 10f;

    [Header("이펙트")]
    public GameObject hitEffectPrefab;
    public float hitEffectDuration = 1f;

    [Header("시각 효과")]
    public TrailRenderer trail;

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
        // 플레이어 총알과 충돌
        PlayerBullet playerBullet = other.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            // PlayerBullet 쪽에서 처리됨 (둘 다 파괴)
            return;
        }

        // 플레이어와 충돌
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            // XR Origin이나 Camera Offset에 있을 수 있음
            playerHealth = other.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            CreateHitEffect(other.ClosestPoint(transform.position));
            Destroy(gameObject);
            Debug.Log($"[EnemyBullet] Hit player for {damage} damage!");
            return;
        }
    }

    private void CreateHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }
    }

    private void OnDestroy()
    {
        // 트레일 처리
        if (trail != null)
        {
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time);
        }
    }
}
