# GameManager 설정 가이드

간소화된 웨이브 시스템 GameManager 설정 방법입니다.

---

## 📋 목차
1. [기본 설정](#기본-설정)
2. [보스 스폰 설정](#보스-스폰-설정)
3. [UI 연결](#ui-연결)
4. [이벤트 시스템](#이벤트-시스템)
5. [문제 해결](#문제-해결)

---

## 🎯 기본 설정

### 1. GameManager 오브젝트 생성

```
Hierarchy:
  GameManager (Empty GameObject)
  ├── GameUI (Canvas)
  └── (기타 매니저들)
```

### 2. GameManager 컴포넌트 추가

1. GameManager 오브젝트 선택
2. **Add Component** → `GameManager`

### 3. 필수 설정

#### Inspector:
```
GameManager
├── 게임 설정
│   ├── Current Wave: 0
│   ├── Bosses Defeated: 0
│   ├── Game Time: 0
│   └── Is Game Active: ☐
│
├── 웨이브 설정
│   ├── Max Waves: 5 (0 = 무한)
│   └── Time Between Waves: 3 (초)
│
├── 보스 스폰 설정
│   ├── Boss Prefabs: [보스 프리팹 배열]
│   ├── Boss Spawn Point: [스폰 위치 Transform]
│   └── Spawn Bosses Automatically: ✓
│
└── 참조
    ├── Player Health: [PlayerHealth 드래그]
    └── Game UI: [GameUI 드래그]
```

---

## 🎮 보스 스폰 설정

### 방법 1: 자동 스폰 (권장)

1. **보스 프리팹 배열 설정:**
```
Boss Prefabs: Size 3
  Element 0: ShootingBoss_Easy
  Element 1: ShootingBoss_Medium
  Element 2: ShootingBoss_Hard
```

2. **스폰 위치 설정:**
```
Hierarchy에 Empty GameObject 생성:
  BossSpawnPoint
    Position: (0, 1, 10) - 플레이어 정면
    Rotation: (0, 180, 0) - 플레이어 바라보기
```

3. **GameManager 연결:**
```
Boss Spawn Point: BossSpawnPoint (드래그)
Spawn Bosses Automatically: ✓ (체크)
```

### 방법 2: 수동 스폰

```
Spawn Bosses Automatically: ☐ (체크 해제)
```

스크립트에서 수동 호출:
```csharp
GameManager.Instance.SpawnBoss();
```

---

## 🎨 UI 연결

### GameUI 설정

1. **GameUI 컴포넌트 확인:**
   - Canvas 오브젝트에 GameUI.cs가 있어야 함

2. **GameManager에 연결:**
```
Game UI: [GameUI 드래그]
```

3. **GameUI가 자동 호출하는 메서드:**
   - `UpdateWave(int wave)` - 웨이브 번호 업데이트
   - `UpdateBossKills(int kills)` - 보스 처치 수 업데이트
   - `ShowGameOver()` - 게임 오버 화면
   - `ShowGameWin()` - 게임 클리어 화면

---

## ⚙️ 이벤트 시스템

GameManager는 다음 UnityEvent를 제공합니다:

### 1. OnWaveStart (int wave)
```csharp
// 새 웨이브 시작 시 호출
gameManager.OnWaveStart.AddListener((wave) => {
    Debug.Log($"웨이브 {wave} 시작!");
});
```

### 2. OnBossDefeat (int totalKills)
```csharp
// 보스 처치 시 호출
gameManager.OnBossDefeat.AddListener((kills) => {
    Debug.Log($"총 {kills}마리 처치!");
});
```

### 3. OnGameStart
```csharp
// 게임 시작 시 호출
gameManager.OnGameStart.AddListener(() => {
    Debug.Log("게임 시작!");
});
```

### 4. OnGameOver
```csharp
// 플레이어 사망 시 호출
gameManager.OnGameOver.AddListener(() => {
    Debug.Log("게임 오버!");
});
```

### 5. OnGameWin
```csharp
// 모든 웨이브 클리어 시 호출
gameManager.OnGameWin.AddListener(() => {
    Debug.Log("게임 클리어!");
});
```

---

## 🔄 게임 플로우

### 자동 진행 (기본)

```
게임 시작 (StartGame)
  ↓
웨이브 1 시작 → 보스 스폰
  ↓
보스 처치 (ShootingBoss.Die → OnBossDefeated)
  ↓
웨이브 2 시작 → 보스 스폰
  ↓
...
  ↓
웨이브 5 클리어 → 게임 클리어 (WinGame)
```

### 플레이어 사망 시

```
플레이어 체력 0
  ↓
PlayerHealth → GameManager.GameOver()
  ↓
게임 오버 화면 표시
  ↓
재시작 버튼 → RestartGame()
```

---

## 🚀 ShootingBoss 연동

ShootingBoss는 자동으로 GameManager와 연동됩니다:

### ShootingBoss.cs의 Die() 메서드:
```csharp
private void Die()
{
    isDead = true;
    StopAllCoroutines();

    // GameManager에 알림
    GameManager gameManager = FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
        gameManager.OnBossDefeated(); // 자동 호출!
    }

    Destroy(gameObject);
}
```

**따라서:**
- 보스가 죽으면 자동으로 다음 웨이브 시작
- 수동 처리 불필요!

---

## 📊 웨이브 진행 예시

### 예시 1: 5웨이브 고정
```
Max Waves: 5
Boss Prefabs: Size 3 (Easy, Medium, Hard)

웨이브 1 → ShootingBoss_Easy
웨이브 2 → ShootingBoss_Medium
웨이브 3 → ShootingBoss_Hard
웨이브 4 → ShootingBoss_Easy (다시 순환)
웨이브 5 → ShootingBoss_Medium

→ 웨이브 5 클리어 시 게임 클리어!
```

### 예시 2: 무한 모드
```
Max Waves: 0 (무한)
Boss Prefabs: Size 3

웨이브 1, 4, 7... → ShootingBoss_Easy
웨이브 2, 5, 8... → ShootingBoss_Medium
웨이브 3, 6, 9... → ShootingBoss_Hard

→ 플레이어가 죽을 때까지 계속!
```

---

## 🐛 문제 해결

### Q1. 보스가 스폰되지 않아요
**확인 사항:**
1. Boss Prefabs 배열이 비어있지 않은지
2. Spawn Bosses Automatically가 체크되어 있는지
3. Console에서 에러 메시지 확인

**해결:**
```
Boss Prefabs에 최소 1개 이상의 보스 프리팹 추가
```

### Q2. 보스가 이상한 곳에 나타나요
**확인:**
- Boss Spawn Point가 설정되었는지
- 설정되지 않았다면 기본 위치 (0, 1, 10)에 스폰됨

**해결:**
```
Boss Spawn Point에 원하는 위치의 Transform 연결
```

### Q3. 보스를 죽여도 다음 웨이브가 안 나와요
**확인:**
1. ShootingBoss.cs의 Die() 메서드에서 `GameManager.OnBossDefeated()` 호출 확인
2. GameManager가 Scene에 존재하는지 확인

**해결:**
- ShootingBoss가 최신 버전인지 확인 (SHOOTING_BOSS_SETUP.md 참조)

### Q4. 게임이 자동으로 시작 안 돼요
**확인:**
- GameManager의 Start()에서 자동으로 StartGame() 호출됨

**수동 시작:**
```csharp
GameManager.Instance.StartGame();
```

### Q5. UI가 업데이트 안 돼요
**확인:**
1. GameUI가 GameManager에 연결되었는지
2. GameUI의 이벤트 리스너가 제대로 등록되었는지

**해결:**
```
GameManager → Game UI에 GameUI 오브젝트 드래그
```

---

## 🎯 추천 설정값

### 쉬운 난이도
```
Max Waves: 3
Time Between Waves: 5 (여유롭게)
Boss Prefabs: [Easy Boss만]
```

### 보통 난이도
```
Max Waves: 5
Time Between Waves: 3
Boss Prefabs: [Easy, Medium, Hard]
```

### 어려운 난이도
```
Max Waves: 10
Time Between Waves: 2 (빠르게)
Boss Prefabs: [Medium, Hard, Extreme]
```

### 무한 모드
```
Max Waves: 0
Time Between Waves: 1
Boss Prefabs: [모든 보스]
```

---

## 📊 체크리스트

설정 완료 확인:
- [ ] GameManager 오브젝트 생성
- [ ] GameManager 컴포넌트 추가
- [ ] Boss Prefabs 배열에 보스 추가
- [ ] Boss Spawn Point 설정 (또는 기본값 사용)
- [ ] Player Health 연결
- [ ] Game UI 연결
- [ ] Max Waves 설정
- [ ] Play 버튼 눌러서 테스트

---

## 🚀 빠른 시작

최소 설정으로 바로 테스트:

1. GameManager 오브젝트 생성
2. GameManager.cs 추가
3. Boss Prefabs에 ShootingBoss 프리팹 1개 추가
4. Play!

→ 기본 위치 (0, 1, 10)에서 보스가 스폰되고 웨이브가 진행됩니다.

---

## 💡 스크립트에서 사용하기

### 싱글톤 접근
```csharp
GameManager.Instance.SpawnBoss();
GameManager.Instance.OnBossDefeated();
GameManager.Instance.GameOver();
```

### 현재 상태 확인
```csharp
int currentWave = GameManager.Instance.currentWave;
bool hasActiveBoss = GameManager.Instance.HasActiveBoss();
GameObject currentBoss = GameManager.Instance.GetCurrentBoss();
```

---

**작성일**: 2025-10-25
**버전**: 1.0 (간소화 버전)
**게임 컨셉**: 탄막 슈팅 VR
