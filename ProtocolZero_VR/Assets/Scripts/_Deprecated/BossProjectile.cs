using UnityEngine;

/// <summary>
/// 보스가 발사하는 투사체
/// 플레이어에게 충돌 시 데미지를 주고 파괴됨
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BossProjectile : MonoBehaviour
{
    [Header("투사체 설정")]
    public float damage = 15f;
    public float lifetime = 10f;
    public bool destroyOnHit = true;

    [Header("이펙트")]
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 2f;
    public TrailRenderer trail;

    private void Start()
    {
        // 일정 시간 후 자동 파괴
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// 투사체 초기화
    /// </summary>
    public void Initialize(float projectileDamage, float projectileLifetime)
    {
        damage = projectileDamage;
        lifetime = projectileLifetime;

        // 자동 파괴 시간 갱신
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 히트 이펙트 생성
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectLifetime);
        }

        // 플레이어에게 데미지
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Projectile hit player for {damage} damage!");
        }

        // 투사체 파괴
        if (destroyOnHit)
        {
            // 트레일이 있으면 분리
            if (trail != null)
            {
                trail.transform.SetParent(null);
                Destroy(trail.gameObject, trail.time);
            }

            Destroy(gameObject);
        }
    }
}
