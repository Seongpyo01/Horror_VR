using UnityEngine;

/// <summary>
/// 원거리 보스 - 멀리서 투사체를 발사하여 공격
/// </summary>
public class RangedBoss : BossBase
{
    [Header("원거리 공격 설정")]
    [Tooltip("투사체 프리팹")]
    public GameObject projectilePrefab;

    [Tooltip("발사 위치")]
    public Transform firePoint;

    [Tooltip("투사체 속도")]
    public float projectileSpeed = 15f;

    [Tooltip("최적 공격 거리")]
    public float optimalRange = 10f;

    [Tooltip("후퇴 거리 (플레이어가 너무 가까우면 후퇴)")]
    public float retreatDistance = 5f;

    [Tooltip("연속 발사 횟수")]
    public int burstCount = 3;

    [Tooltip("연속 발사 간격")]
    public float burstInterval = 0.3f;

    [Tooltip("애니메이션 트리거")]
    public string shootAnimationTrigger = "Shoot";

    private Animator animator;
    private int currentBurstCount;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        // 원거리 보스 특성 설정
        bossName = "Ranged Boss";
        moveSpeed = 3f; // 느린 이동
        attackRange = 15f; // 원거리 범위
        attackDamage = 15f; // 중간 데미지
        maxHealth = 100f;
        currentHealth = maxHealth;
        detectionRange = 25f;

        if (firePoint == null)
        {
            // 발사 위치 자동 생성
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0, 1.5f, 1f);
            firePoint = fp.transform;
        }
    }

    protected override void UpdateBehavior()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Idle:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = BossState.Chasing;
                }
                break;

            case BossState.Chasing:
                // 최적 거리 유지
                if (distanceToPlayer < retreatDistance)
                {
                    // 너무 가까우면 후퇴
                    Retreat();
                }
                else if (distanceToPlayer > optimalRange)
                {
                    // 너무 멀면 접근
                    ChasePlayer();
                }
                else
                {
                    // 최적 거리에서 공격
                    currentState = BossState.Attacking;
                }
                break;

            case BossState.Attacking:
                AttackPlayer();

                // 거리가 너무 가까워지거나 멀어지면 상태 변경
                if (distanceToPlayer < retreatDistance || distanceToPlayer > attackRange)
                {
                    currentState = BossState.Chasing;
                }
                break;
        }
    }

    private void Retreat()
    {
        // 플레이어 반대 방향으로 이동
        Vector3 retreatDirection = (transform.position - player.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDirection * 3f;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(retreatTarget);
        }
    }

    protected override void PerformAttack()
    {
        if (currentBurstCount < burstCount)
        {
            ShootProjectile();
            currentBurstCount++;

            // 다음 연속 발사 예약
            Invoke(nameof(ContinueBurst), burstInterval);
        }
        else
        {
            // 연속 발사 완료, 리셋
            currentBurstCount = 0;
        }
    }

    private void ContinueBurst()
    {
        if (currentState == BossState.Attacking && currentBurstCount < burstCount)
        {
            ShootProjectile();
            currentBurstCount++;

            if (currentBurstCount < burstCount)
            {
                Invoke(nameof(ContinueBurst), burstInterval);
            }
            else
            {
                currentBurstCount = 0;
            }
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        // 플레이어를 향한 방향 계산
        Vector3 direction = (player.position - firePoint.position).normalized;

        // 투사체 생성
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        // 투사체에 데미지 설정
        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript != null)
        {
            projScript.damage = attackDamage;
        }

        // 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger(shootAnimationTrigger);
        }

        PlaySound(attackSound);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 최적 거리 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, optimalRange);

        // 후퇴 거리 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
