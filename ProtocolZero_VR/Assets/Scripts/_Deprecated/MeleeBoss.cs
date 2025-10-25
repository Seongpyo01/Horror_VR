using UnityEngine;

/// <summary>
/// 근거리 보스 - 플레이어에게 빠르게 접근하여 강력한 근접 공격을 가함
/// </summary>
public class MeleeBoss : BossBase
{
    [Header("근접 공격 설정")]
    [Tooltip("공격 애니메이션 트리거 이름")]
    public string attackAnimationTrigger = "Attack";

    [Tooltip("돌진 공격 활성화")]
    public bool useChargeAttack = true;

    [Tooltip("돌진 속도 배율")]
    public float chargeSpeedMultiplier = 2f;

    [Tooltip("돌진 거리")]
    public float chargeDistance = 5f;

    [Tooltip("돌진 쿨다운")]
    public float chargeCooldown = 5f;

    private float lastChargeTime;
    private bool isCharging = false;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        // 근접 보스 특성 설정
        bossName = "Melee Boss";
        moveSpeed = 5f; // 빠른 이동
        attackRange = 3f; // 근접 범위
        attackDamage = 25f; // 높은 데미지
        maxHealth = 150f;
        currentHealth = maxHealth;
    }

    protected override void Update()
    {
        base.Update();

        // 돌진 공격 체크
        if (useChargeAttack && !isCharging && currentState == BossState.Chasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 중거리에서 돌진 공격
            if (distanceToPlayer > attackRange && distanceToPlayer <= chargeDistance
                && Time.time >= lastChargeTime + chargeCooldown)
            {
                StartCharge();
            }
        }
    }

    protected override void PerformAttack()
    {
        PlaySound(attackSound);

        // 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger(attackAnimationTrigger);
        }

        // 근접 공격 - Sphere Cast로 범위 내 플레이어 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, attackRange * 0.7f);

        foreach (Collider hitCollider in hitColliders)
        {
            PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"{bossName} hit player for {attackDamage} damage!");

                // 넉백 효과
                ApplyKnockback(hitCollider.transform);
                break;
            }
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        lastChargeTime = Time.time;

        // 일정 시간 동안 빠르게 이동
        if (agent != null)
        {
            agent.speed = moveSpeed * chargeSpeedMultiplier;
        }

        Invoke(nameof(EndCharge), 1.5f);

        Debug.Log($"{bossName} is charging!");
    }

    private void EndCharge()
    {
        isCharging = false;

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    private void ApplyKnockback(Transform target)
    {
        // 플레이어에게 넉백 효과 (간단한 버전)
        Vector3 knockbackDirection = (target.position - transform.position).normalized;
        knockbackDirection.y = 0;

        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.Move(knockbackDirection * 2f);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 돌진 거리 시각화
        if (useChargeAttack)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chargeDistance);
        }
    }
}
