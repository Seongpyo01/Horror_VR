# Scripts 폴더 구조

## 📁 폴더 구성

```
Assets/Scripts/
├── Player/              # 플레이어 관련
│   ├── VRFixedGun.cs           - 컨트롤러 고정형 총
│   ├── PlayerBullet.cs         - 플레이어 총알
│   ├── SimpleXRController.cs   - VR 컨트롤러 트래킹
│   ├── PlayerHealth.cs         - 플레이어 체력 시스템
│   └── HeadTiltMovement.cs     - 머리 기울기 이동 (NEW!)
│
├── Enemy/               # 적/보스 관련
│   ├── ShootingBoss.cs         - 탄막 발사 보스 (NEW!)
│   ├── EnemyBullet.cs          - 적 총알
│   └── BossBase.cs             - 보스 베이스 클래스 (구버전)
│
├── Core/                # 게임 핵심 시스템
│   └── GameManager.cs          - 게임 매니저
│
├── UI/                  # UI 관련
│   ├── GameUI.cs               - 게임 UI 관리
│   └── HealthBarUI.cs          - 체력바 UI
│
├── Documentation/       # 문서
│   ├── PLAYER_GUN_SETUP.md
│   ├── SHOOTING_BOSS_SETUP.md
│   ├── GAME_MANAGER_SETUP.md
│   ├── HEAD_TILT_MOVEMENT_SETUP.md
│   ├── BULLET_SYSTEM_UPDATE.md
│   ├── CONTROLLER_DIRECTION_FIX.md
│   ├── XR_CONTROLLER_FIX.md
│   └── VR_TROUBLESHOOTING.md
│
└── _Deprecated/         # 구버전 파일 (사용 안 함)
    ├── InfiniteScroller.cs
    ├── VRGun.cs
    ├── MeleeBoss.cs
    ├── RangedBoss.cs
    ├── HybridBoss.cs
    ├── BossSpawner.cs
    ├── BossProjectile.cs
    ├── BossHealthBar.cs
    ├── Bullet.cs
    └── GAME_SETUP_GUIDE.md
```

---

## 🎮 현재 게임 컨셉

**탄막 슈팅 VR 게임**

### 플레이어
- 컨트롤러에 고정된 쌍권총
- 트리거로 발사
- **머리 기울기로 좌우 이동** (NEW!)
  - 머리를 왼쪽으로 기울이면 왼쪽으로 이동
  - 머리를 오른쪽으로 기울이면 오른쪽으로 이동
  - 최소 기울기 각도로 의도하지 않은 움직임 방지
- 플레이어 총알로 적 총알을 맞춰 상쇄

### 보스
- 정면에서 탄막 발사
- 근접 공격 없음
- 단순한 패턴

### 총알 시스템
- Transform 기반 Projectile 이동
- 플레이어 총알 + 적 총알 = 둘 다 파괴

---

## 📝 폴더별 설명

### Player/
플레이어 직접 조작 및 상호작용 관련 스크립트
- 총, 총알, 컨트롤러, 체력, 이동

### Enemy/
적 및 보스 관련 스크립트
- 보스 AI, 적 총알

### Core/
게임 전체 시스템
- 게임 매니저, 웨이브 관리

### UI/
사용자 인터페이스
- 체력바, 점수, 게임 오버 UI

### Documentation/
설정 가이드 및 문제 해결 문서

### _Deprecated/
**사용하지 않는 구버전 파일들**
- 삭제 가능하지만 참고용으로 보관
- 새 프로젝트에서는 사용 금지

---

## 🚀 핵심 스크립트

### 필수 스크립트 (현재 사용 중)

1. **VRFixedGun.cs** ⭐
   - 컨트롤러에 고정된 총
   - 트리거 입력 처리
   - 총알 생성

2. **PlayerBullet.cs** ⭐
   - Transform 기반 이동
   - 적 총알과 충돌 시 상쇄
   - 보스에게 데미지

3. **EnemyBullet.cs** ⭐
   - Transform 기반 이동
   - 플레이어에게 데미지
   - 플레이어 총알과 충돌 시 파괴

4. **SimpleXRController.cs** ⭐
   - VR 컨트롤러 트래킹
   - 회전 보정 기능

5. **PlayerHealth.cs** ⭐
   - 플레이어 체력 관리
   - 피격 시스템

6. **ShootingBoss.cs** ⭐ NEW!
   - 탄막 발사 보스
   - 4가지 발사 패턴 (단발/연발/부채꼴/원형)
   - 체력 시스템

7. **HeadTiltMovement.cs** ⭐ NEW!
   - VR 머리 기울기로 좌우 이동
   - 최소 기울기 각도 설정 (의도하지 않은 움직임 방지)
   - 이동 범위 제한

---

## 📖 시작 가이드

### 1. 플레이어 총 설정
→ [Documentation/PLAYER_GUN_SETUP.md](Documentation/PLAYER_GUN_SETUP.md)

### 2. 보스 설정 (NEW!)
→ [Documentation/SHOOTING_BOSS_SETUP.md](Documentation/SHOOTING_BOSS_SETUP.md)

### 3. 게임 매니저 설정 (NEW!)
→ [Documentation/GAME_MANAGER_SETUP.md](Documentation/GAME_MANAGER_SETUP.md)

### 4. 머리 기울기 이동 설정 (NEW!)
→ [Documentation/HEAD_TILT_MOVEMENT_SETUP.md](Documentation/HEAD_TILT_MOVEMENT_SETUP.md)

### 5. 컨트롤러 방향 보정
→ [Documentation/CONTROLLER_DIRECTION_FIX.md](Documentation/CONTROLLER_DIRECTION_FIX.md)

### 6. 문제 해결
→ [Documentation/VR_TROUBLESHOOTING.md](Documentation/VR_TROUBLESHOOTING.md)

---

## ⚠️ 주의사항

### _Deprecated 폴더
- **절대 사용하지 마세요!**
- 구버전 게임 컨셉용 파일들
- 참고용으로만 보관

### 파일 이동 시
- Unity에서 파일을 이동하면 .meta 파일도 함께 이동됩니다
- 스크립트 참조가 깨질 수 있으니 주의

---

## 🔄 TODO

### 완료
- [x] 간단한 슈팅 보스 스크립트 작성 (ShootingBoss.cs)
- [x] PlayerBullet에 ShootingBoss 지원 추가
- [x] GameManager.cs 간소화 (웨이브 시스템 + 자동 보스 스폰)
- [x] GameUI 업데이트 (VRFixedGun 지원)
- [x] 플레이어 좌우 이동 시스템 추가 (HeadTiltMovement.cs)

### 선택적 추가 기능
- [ ] 보스별 탄막 패턴 커스터마이징
- [ ] 사운드 이펙트 및 파티클 추가
- [ ] 난이도 조절 시스템
- [ ] 스코어 시스템

---

**정리 완료 날짜**: 2025-10-25
**게임 컨셉**: 탄막 슈팅 VR
