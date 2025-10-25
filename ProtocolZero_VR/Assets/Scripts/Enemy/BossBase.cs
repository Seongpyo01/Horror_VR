using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// 보스 몬스터 베이스 클래스
/// 모든 보스는 이 클래스를 상속받음
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class BossBase : MonoBehaviour
{
    [Header("보스 기본 정보")]
    public string bossName = "Boss";
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float detectionRange = 20f;

    [Header("공격 설정")]
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    protected float lastAttackTime;

    [Header("AI 설정")]
    public Transform player;
    protected NavMeshAgent agent;
    protected BossState currentState = BossState.Idle;

    [Header("이펙트")]
    public GameObject deathEffectPrefab;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    protected AudioSource audioSource;

    [Header("UI")]
    public Transform healthBarPosition;

    [Header("이벤트")]
    public UnityEvent OnBossSpawned;
    public UnityEvent OnBossDeath;
    public UnityEvent<float> OnHealthChanged;

    protected enum BossState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        if (player == null)
        {
            // 플레이어(VR 카메라) 자동 찾기
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                player = mainCam.transform;
            }
        }

        agent.speed = moveSpeed;
        OnBossSpawned?.Invoke();
    }

    protected virtual void Update()
    {
        if (currentState == BossState.Dead || player == null)
            return;

        UpdateBehavior();
    }

    protected virtual void UpdateBehavior()
    {
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
                ChasePlayer();

                if (distanceToPlayer <= attackRange)
                {
                    currentState = BossState.Attacking;
                }
                break;

            case BossState.Attacking:
                AttackPlayer();

                if (distanceToPlayer > attackRange)
                {
                    currentState = BossState.Chasing;
                }
                break;
        }
    }

    protected virtual void ChasePlayer()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    protected virtual void AttackPlayer()
    {
        // 플레이어를 향해 회전
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }

        // 이동 정지
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }

        // 공격 쿨다운 체크
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    protected abstract void PerformAttack();

    public virtual void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead)
            return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        PlaySound(hurtSound);

        Debug.Log($"{bossName} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        currentState = BossState.Dead;

        Debug.Log($"{bossName} defeated!");

        // 이펙트
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        PlaySound(deathSound);

        // 게임 매니저에 알림
        if (GameManager.Instance != null)
        {
           //GameManager.Instance.OnBossKilled();
        }

        OnBossDeath?.Invoke();

        // NavMeshAgent 비활성화
        if (agent != null)
        {
            agent.enabled = false;
        }

        // 오브젝트 파괴
        Destroy(gameObject, 2f);
    }

    protected void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 탐지 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
