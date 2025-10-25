# ShootingBoss 설정 가이드

탄막 슈팅 VR 게임용 보스 설정 방법입니다.

---

## 📋 목차
1. [기본 설정](#기본-설정)
2. [총알 패턴 설정](#총알-패턴-설정)
3. [발사 지점 설정](#발사-지점-설정)
4. [체력 시스템 연결](#체력-시스템-연결)
5. [문제 해결](#문제-해결)

---

## 🎯 기본 설정

### 1. 보스 오브젝트 생성

```
Hierarchy:
  Boss
  ├── Model (보스 3D 모델)
  ├── FirePoint (발사 위치)
  └── HealthBar (체력바 UI - 선택사항)
```

### 2. ShootingBoss 컴포넌트 추가

1. Boss 오브젝트 선택
2. **Add Component** → `ShootingBoss`

### 3. 필수 설정

#### Inspector에서 설정:
```
ShootingBoss
├── 보스 설정
│   ├── Max Health: 100
│   └── Show Health Bar: ✓
│
├── 발사 설정
│   ├── Enemy Bullet Prefab: [EnemyBullet Prefab 드래그]
│   ├── Fire Points: [FirePoint Transform 드래그]
│   ├── Bullet Speed: 10
│   ├── Bullet Damage: 10
│   └── Bullet Lifetime: 5
│
└── 패턴 설정
    ├── Current Pattern: Single
    ├── Fire Rate: 1
    ├── Burst Count: 3
    ├── Burst Delay: 0.1
    ├── Spread Count: 5
    └── Spread Angle: 30
```

---

## 🔫 총알 패턴 설정

ShootingBoss는 4가지 패턴을 지원합니다:

### 1. Single (단발)
- 정면으로 한 발씩 발사
- 가장 기본적인 패턴
```
설정:
  Current Pattern: Single
  Fire Rate: 1 (초당 1발)
```

### 2. Burst (연발)
- 짧은 시간에 여러 발 연속 발사
- 빠른 속도로 위협적
```
설정:
  Current Pattern: Burst
  Fire Rate: 1 (초당 1회 연발)
  Burst Count: 3 (3발씩)
  Burst Delay: 0.1 (0.1초 간격)
```

### 3. Spread (부채꼴)
- 여러 방향으로 동시 발사
- 회피 난이도 증가
```
설정:
  Current Pattern: Spread
  Fire Rate: 0.5
  Spread Count: 5 (5개 방향)
  Spread Angle: 30 (30도 범위)
```

### 4. Circle (원형)
- 360도 모든 방향으로 발사
- 최고 난이도 패턴
```
설정:
  Current Pattern: Circle
  Fire Rate: 0.3 (천천히)
```

---

## 📍 발사 지점 설정

### 방법 1: 단일 발사 지점
```
Boss
└── FirePoint (Empty GameObject)
    Position: (0, 1.5, 0.5) - 보스 정면
```

Inspector:
```
Fire Points: Size 1
  Element 0: FirePoint (드래그)
```

### 방법 2: 다중 발사 지점 (양손)
```
Boss
├── FirePoint_Left (왼손)
│   Position: (-0.5, 1.5, 0.5)
└── FirePoint_Right (오른손)
    Position: (0.5, 1.5, 0.5)
```

Inspector:
```
Fire Points: Size 2
  Element 0: FirePoint_Left
  Element 1: FirePoint_Right
```

### ⚠️ 중요!
- Fire Points가 비어있으면 보스 중심에서 발사됩니다
- FirePoint는 **정면(Z축)**이 발사 방향입니다

---

## 💚 체력 시스템 연결

### 방법 1: HealthBarUI 사용 (권장)

1. HealthBar 캔버스 생성:
```
Boss
└── HealthBar (Canvas - World Space)
    └── Fill (Image)
```

2. HealthBarUI 컴포넌트 추가:
```
HealthBar에 HealthBarUI.cs 추가
└── Health Fill: Fill Image 연결
```

3. ShootingBoss 설정:
```
Show Health Bar: ✓ (체크)
```

### 방법 2: 체력바 없이 사용
```
Show Health Bar: ☐ (체크 해제)
```

---

## 🎮 PlayerBullet 연결

### PlayerBullet.cs에 보스 데미지 처리 추가

`PlayerBullet.cs`의 `OnTriggerEnter`에서:

```csharp
private void OnTriggerEnter(Collider other)
{
    // ShootingBoss에 데미지
    ShootingBoss shootingBoss = other.GetComponent<ShootingBoss>();
    if (shootingBoss != null)
    {
        shootingBoss.TakeDamage(damage);
        Destroy(gameObject);
        return;
    }

    // 기존 BossBase도 지원 (하위 호환)
    BossBase bossBase = other.GetComponent<BossBase>();
    if (bossBase != null)
    {
        bossBase.TakeDamage(damage);
        Destroy(gameObject);
        return;
    }

    // 적 총알 상쇄
    EnemyBullet enemyBullet = other.GetComponent<EnemyBullet>();
    if (enemyBullet != null)
    {
        Destroy(enemyBullet.gameObject);
        Destroy(gameObject);
        return;
    }
}
```

---

## 🎨 보스 패턴 단계별 변경 (선택사항)

체력에 따라 자동으로 패턴을 변경하려면:

### ShootingBoss.cs의 TakeDamage에 추가:

```csharp
public void TakeDamage(float damage)
{
    if (isDead) return;

    currentHealth -= damage;

    // 체력바 업데이트
    if (showHealthBar)
    {
        UpdateHealthBar();
    }

    // 페이즈 전환 체크 (추가!)
    CheckPhaseTransition();

    // 사망 체크
    if (currentHealth <= 0)
    {
        Die();
    }
}
```

패턴 자동 변경:
- 100% ~ 75%: Single (단발)
- 75% ~ 50%: Burst (연발)
- 50% ~ 25%: Spread (부채꼴)
- 25% ~ 0%: Circle (원형)

---

## ⚙️ GameManager 연결

ShootingBoss가 죽으면 `GameManager.OnBossDefeated()`를 호출합니다.

### GameManager.cs에 추가 필요:

```csharp
public void OnBossDefeated()
{
    Debug.Log("보스 처치! 다음 웨이브로...");

    // 웨이브 진행 또는 게임 클리어 처리
    // 예: SpawnNextWave();
}
```

---

## 🐛 문제 해결

### Q1. 총알이 발사되지 않아요
**확인 사항:**
1. Enemy Bullet Prefab이 설정되었는지 확인
2. Fire Points에 Transform이 연결되었는지 확인
3. Console에서 에러 메시지 확인

### Q2. 총알이 이상한 방향으로 날아가요
**해결:**
- FirePoint의 **Z축(파란 화살표)**가 발사 방향입니다
- Scene 뷰에서 FirePoint 회전 확인
- Gizmo로 발사 방향 시각화 (Boss 선택 시 빨간 선)

### Q3. 보스가 플레이어를 안 봐요
**확인:**
- Player 오브젝트에 **"Player" 태그**가 있는지 확인
- `GameObject → Tag → Player`

### Q4. 체력바가 안 보여요
**확인:**
1. Show Health Bar가 체크되었는지
2. Boss 하위에 HealthBarUI 컴포넌트가 있는지
3. Canvas가 World Space로 설정되었는지

### Q5. 보스가 죽어도 다음 웨이브가 안 나와요
**확인:**
- GameManager에 `OnBossDefeated()` 메서드 구현
- GameManager가 Scene에 존재하는지 확인

---

## 🎯 추천 설정값

### 쉬운 난이도
```
Max Health: 50
Fire Rate: 0.5 (느림)
Bullet Speed: 8
Pattern: Single
```

### 보통 난이도
```
Max Health: 100
Fire Rate: 1
Bullet Speed: 10
Pattern: Burst
```

### 어려운 난이도
```
Max Health: 150
Fire Rate: 1.5 (빠름)
Bullet Speed: 12
Pattern: Spread
```

### 최종 보스
```
Max Health: 200
Fire Rate: 2
Bullet Speed: 15
Pattern: Circle (또는 체력별 자동 변경)
```

---

## 📊 체크리스트

설정 완료 확인:
- [ ] ShootingBoss 컴포넌트 추가
- [ ] Enemy Bullet Prefab 연결
- [ ] Fire Points 설정
- [ ] 총알 속도/데미지 설정
- [ ] 발사 패턴 선택
- [ ] Player 태그 설정
- [ ] PlayerBullet에 ShootingBoss 데미지 처리 추가
- [ ] GameManager에 OnBossDefeated 구현
- [ ] (선택) HealthBarUI 연결

---

## 🚀 빠른 시작

최소 설정으로 바로 테스트:

1. Boss 오브젝트 생성
2. ShootingBoss.cs 추가
3. Enemy Bullet Prefab만 연결
4. Play!

→ 보스 중앙에서 정면으로 단발 발사됩니다.

---

**작성일**: 2025-10-25
**버전**: 1.0
**게임 컨셉**: 탄막 슈팅 VR
