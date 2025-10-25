using UnityEngine;

/// <summary>
/// 혼합형 보스 - 근거리와 원거리 공격을 모두 사용
/// 거리에 따라 전투 방식을 변경
/// </summary>
public class HybridBoss : BossBase
{
    [Header("근접 공격 설정")]
    [Tooltip("근접 공격 데미지")]
    public float meleeDamage = 30f;

    [Tooltip("근접 공격 범위")]
    public float meleeRange = 3f;

    [Tooltip("근접 공격 애니메이션")]
    public string meleeAnimationTrigger = "MeleeAttack";

    [Header("원거리 공격 설정")]
    [Tooltip("투사체 프리팹")]
    public GameObject projectilePrefab;

    [Tooltip("발사 위치")]
    public Transform firePoint;

    [Tooltip("투사체 속도")]
    public float projectileSpeed = 20f;

    [Tooltip("원거리 공격 데미지")]
    public float rangedDamage = 15f;

    [Tooltip("원거리 공격 애니메이션")]
    public string rangedAnimationTrigger = "RangedAttack";

    [Header("전투 모드")]
    [Tooltip("중거리 기준점")]
    public float midRangeThreshold = 7f;

    [Tooltip("특수 공격 - 점프 어택")]
    public bool useJumpAttack = true;

    [Tooltip("점프 공격 쿨다운")]
    public float jumpAttackCooldown = 8f;

    [Tooltip("점프 공격 데미지")]
    public float jumpAttackDamage = 40f;

    [Tooltip("점프 공격 범위")]
    public float jumpAttackRadius = 5f;

    private float lastJumpAttackTime;
    private Animator animator;
    private AttackMode currentAttackMode = AttackMode.Melee;

    private enum AttackMode
    {
        Melee,
        Ranged
    }

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        // 혼합형 보스 특성 설정
        bossName = "Hybrid Boss";
        moveSpeed = 4f; // 중간 이동속도
        attackRange = 12f; // 넓은 범위
        maxHealth = 200f; // 높은 체력
        currentHealth = maxHealth;
        detectionRange = 20f;

        if (firePoint == null)
        {
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

        // 거리에 따라 공격 모드 결정
        if (distanceToPlayer <= meleeRange)
        {
            currentAttackMode = AttackMode.Melee;
        }
        else if (distanceToPlayer > meleeRange && distanceToPlayer <= attackRange)
        {
            currentAttackMode = AttackMode.Ranged;
        }

        // 기본 AI 로직
        base.UpdateBehavior();

        // 점프 공격 체크 (중거리에서)
        if (useJumpAttack && distanceToPlayer > meleeRange && distanceToPlayer <= midRangeThreshold)
        {
            if (Time.time >= lastJumpAttackTime + jumpAttackCooldown)
            {
                PerformJumpAttack();
            }
        }
    }

    protected override void PerformAttack()
    {
        switch (currentAttackMode)
        {
            case AttackMode.Melee:
                PerformMeleeAttack();
                break;

            case AttackMode.Ranged:
                PerformRangedAttack();
                break;
        }
    }

    private void PerformMeleeAttack()
    {
        PlaySound(attackSound);

        if (animator != null)
        {
            animator.SetTrigger(meleeAnimationTrigger);
        }

        // 근접 공격 판정
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, meleeRange * 0.7f);

        foreach (Collider hitCollider in hitColliders)
        {
            PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
                Debug.Log($"{bossName} melee hit player for {meleeDamage} damage!");
                break;
            }
        }
    }

    private void PerformRangedAttack()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        BossProjectile projScript = projectile.GetComponent<BossProjectile>();
        if (projScript != null)
        {
            projScript.damage = rangedDamage;
        }

        if (animator != null)
        {
            animator.SetTrigger(rangedAnimationTrigger);
        }

        PlaySound(attackSound);
    }

    private void PerformJumpAttack()
    {
        lastJumpAttackTime = Time.time;

        Debug.Log($"{bossName} performing jump attack!");

        // 점프 애니메이션 (있다면)
        if (animator != null)
        {
            animator.SetTrigger("JumpAttack");
        }

        // 범위 공격
        Invoke(nameof(JumpAttackImpact), 0.5f); // 점프 후 착지 시 데미지
    }

    private void JumpAttackImpact()
    {
        // 범위 내 플레이어에게 데미지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, jumpAttackRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(jumpAttackDamage);
                Debug.Log($"{bossName} jump attack hit player for {jumpAttackDamage} damage!");
            }
        }

        // 충격파 이펙트 (선택사항)
        PlaySound(attackSound);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 근접 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // 중거리 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midRangeThreshold);

        // 점프 공격 범위
        if (useJumpAttack)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, jumpAttackRadius);
        }
    }
}
