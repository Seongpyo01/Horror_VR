using UnityEngine;

/// <summary>
/// 총알 스크립트 - Transform 기반 Projectile 이동
/// 충돌 시 데미지를 주고 파괴됨
/// </summary>
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float damage = 10f;
    public float speed = 50f;
    public float lifetime = 5f;
    public bool destroyOnHit = true;

    [Header("이펙트")]
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 2f;

    private Vector3 moveDirection;
    private bool initialized = false;
    private Rigidbody rb;

    private void Awake()
    {
        // Rigidbody 설정 (충돌 감지용, 물리 영향 없음)
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
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
        if (!initialized)
        {
            Initialize(transform.forward, damage, speed, lifetime);
        }
    }

    /// <summary>
    /// 총알 초기화 및 발사 (새 방식 - 방향 포함)
    /// </summary>
    public void Initialize(Vector3 direction, float bulletDamage, float bulletSpeed, float bulletLifetime)
    {
        moveDirection = direction.normalized;
        damage = bulletDamage;
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        initialized = true;

        // 일정 시간 후 자동 파괴
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!initialized) return;

        // Transform을 직접 이동 (Projectile 방식)
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.contacts[0].point, collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.ClosestPoint(transform.position), other.gameObject);
    }

    private void HandleHit(Vector3 hitPoint, GameObject hitObject)
    {
        // 히트 이펙트 생성
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            Destroy(effect, hitEffectLifetime);
        }

        // 보스에게 데미지
        BossBase boss = hitObject.GetComponent<BossBase>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
        }

        // 총알 파괴
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
