# VR 보스 러쉬 게임 설정 가이드

## 📋 개요
무한 스크롤링 환경에서 3종류의 보스 몬스터를 처치하는 VR 액션 게임입니다.
쌍권총을 들고 트리거 키(검지 버튼)로 발사하여 보스들을 물리치세요!

## 🎮 조작법
- **그랩 키 (옆면 버튼)**: 권총 잡기/놓기
- **트리거 키 (검지 버튼)**: 발사

## 🎮 주요 기능
- ✅ 무한 스크롤링 환경
- ✅ 3종류 보스 (근거리/원거리/혼합형)
- ✅ 쌍권총 시스템 (트리거 키로 발사)
- ✅ 연속 발사 및 단발 모드 지원
- ✅ 플레이어 체력 및 피격 시스템
- ✅ 웨이브 시스템
- ✅ 게임 UI
- ✅ 햅틱 피드백 (발사 시 진동)

---

## 🗂️ 스크립트 목록

### 핵심 시스템
1. **GameManager.cs** - 게임 전체 관리
2. **InfiniteScroller.cs** - 무한 스크롤링 환경
3. **BossSpawner.cs** - 보스 스폰 시스템

### 플레이어
4. **PlayerHealth.cs** - 플레이어 체력 관리
5. **VRGun.cs** - VR 권총 (트리거 발사)
6. **VRGunController.cs** - VR 권총 (그랩 키 발사)
7. **Bullet.cs** - 총알 시스템

### 보스 시스템
8. **BossBase.cs** - 보스 베이스 클래스
9. **MeleeBoss.cs** - 근거리 보스
10. **RangedBoss.cs** - 원거리 보스
11. **HybridBoss.cs** - 혼합형 보스
12. **BossProjectile.cs** - 보스 투사체

### UI
13. **GameUI.cs** - 게임 UI 관리
14. **HealthBarUI.cs** - 범용 체력바
15. **BossHealthBar.cs** - 보스 전용 체력바

---

## ⚙️ 씬 설정 가이드

### 1️⃣ GameManager 설정
1. 빈 GameObject 생성 → 이름: "GameManager"
2. `GameManager.cs` 컴포넌트 추가
3. 설정:
   - Time Between Bosses: 5초
   - Max Waves: 0 (무한) 또는 원하는 웨이브 수
   - Boss Spawner: BossSpawner 오브젝트 연결
   - Player Health: PlayerHealth 컴포넌트 연결

### 2️⃣ 무한 스크롤링 설정
1. 빈 GameObject 생성 → 이름: "InfiniteScroller"
2. `InfiniteScroller.cs` 컴포넌트 추가
3. 설정:
   - Player: XR Origin의 Main Camera 연결
   - Auto Scroll: true (자동 전진)
   - Scroll Speed: 5
   - Tile Prefabs: 반복할 바닥 타일 프리팹 배열
   - Number Of Tiles: 5
   - Tile Length: 20 (타일 하나의 길이)

### 3️⃣ XR Origin 설정
1. XR Origin 오브젝트에 `PlayerHealth.cs` 추가
2. 설정:
   - Max Health: 100
   - Auto Regeneration: true
   - Regeneration Rate: 5
   - Invincibility Duration: 1

### 4️⃣ 쌍권총 설정

#### ⚠️ XR Starter Assets 사용 시 중요!
XR Interaction Toolkit의 Starter Assets를 사용하는 경우:
- **VRGunControllerImproved.cs** 사용 (디버깅 기능 포함)
- **[VR_TROUBLESHOOTING.md](Assets/Scripts/VR_TROUBLESHOOTING.md)** 문서 참조

#### VRGunControllerImproved (권장) ⭐
XR Starter Assets와 100% 호환되는 버전

1. **권총 오브젝트 만들기**:
   - Capsule이나 Cube로 임시 제작 (크기: 0.1 x 0.05 x 0.2)
   - 나중에 3D 모델로 교체 가능

2. **컴포넌트 추가**:
   - `XRGrabInteractable` 컴포넌트 추가
   - `VRGunControllerImproved.cs` 컴포넌트 추가

3. **XRGrabInteractable 설정**:
   ```
   Movement Type: Instantaneous
   Throw on Detach: false
   Interaction Layer Mask: Interactable (기본값)
   ```

4. **VRGunControllerImproved 설정**:
   - **Bullet Prefab**: Bullet 프리팹 연결 ⚠️ 필수!
   - **Muzzle**: 자동 생성됨 (수동 설정 가능)
   - **Damage**: 25
   - **Fire Rate**: 0.2
   - **Bullet Speed**: 50
   - **Continuous Fire**: true (연속 발사)
   - **Enable Debug Logs**: true (처음엔 켜두기)

5. **프리팹으로 저장**:
   - 설정이 완료되면 Prefabs 폴더에 저장
   - 왼손용/오른손용 복사

6. **씬에 배치하지 말고 손에 직접 부착하지 않기**:
   - 권총은 독립적인 오브젝트로 씬에 배치
   - 게임 시작 시 플레이어가 잡는 방식

#### VRGunController (기본 버전)
- 단순한 버전
- 디버깅 기능 없음

#### VRGun (탄약 시스템 포함)
- 탄약 시스템이 필요한 경우
- 재장전 기능 포함

### 5️⃣ Bullet 프리팹 만들기
1. Sphere 생성 (크기 0.1)
2. Rigidbody 추가 (Use Gravity: false)
3. Sphere Collider 추가 (Is Trigger: true)
4. `Bullet.cs` 스크립트 추가
5. Material 적용 (발광 효과)
6. 프리팹으로 저장

### 6️⃣ 보스 프리팹 만들기

#### 근거리 보스 (MeleeBoss)
1. Capsule 생성 + NavMeshAgent
2. `MeleeBoss.cs` 스크립트 추가
3. 특징:
   - 빠른 이동속도 (5)
   - 높은 데미지 (25)
   - 돌진 공격 가능

#### 원거리 보스 (RangedBoss)
1. Capsule 생성 + NavMeshAgent
2. `RangedBoss.cs` 스크립트 추가
3. Projectile Prefab 설정 필요
4. 특징:
   - 느린 이동속도 (3)
   - 원거리 공격 (15 데미지)
   - 거리 유지 AI

#### 혼합형 보스 (HybridBoss)
1. Capsule 생성 + NavMeshAgent
2. `HybridBoss.cs` 스크립트 추가
3. Projectile Prefab 설정
4. 특징:
   - 중간 이동속도 (4)
   - 근거리/원거리 전환 공격
   - 점프 공격 가능

### 7️⃣ BossSpawner 설정
1. 빈 GameObject 생성 → "BossSpawner"
2. `BossSpawner.cs` 추가
3. 설정:
   - Boss Prefabs: 3개 보스 프리팹 배열
   - Spawn Points: 스폰 위치 Transform 배열
   - Random Boss Selection: true
   - Default Spawn Offset: (0, 0, 20)

### 8️⃣ NavMesh 베이킹
1. Window → AI → Navigation
2. 바닥 오브젝트 선택 → Navigation Static 체크
3. Bake 탭 → Bake 버튼 클릭

### 9️⃣ UI 설정
1. Canvas 생성 (World Space)
2. GameUI GameObject 생성
3. `GameUI.cs` 추가
4. UI 요소 생성:
   - Wave Text
   - Score Text
   - Health Bar (Image - Fill)
   - Ammo Text
   - Game Over Panel
   - Victory Panel

---

## 🎯 보스 특징

### 🔴 근거리 보스 (MeleeBoss)
- 체력: 150
- 공격력: 25
- 공격 범위: 3m
- 특수 능력: 돌진 공격

### 🔵 원거리 보스 (RangedBoss)
- 체력: 100
- 공격력: 15
- 공격 범위: 15m
- 특수 능력: 연속 발사 (3발)

### 🟣 혼합형 보스 (HybridBoss)
- 체력: 200
- 근접 공격력: 30
- 원거리 공격력: 15
- 특수 능력: 점프 공격 (범위 5m, 40 데미지)

---

## 🕹️ 게임 흐름

1. **게임 시작** → GameManager.StartGame()
2. **웨이브 시작** → 보스 스폰 (5초 대기)
3. **전투** → 플레이어가 보스 처치
4. **보스 처치** → 다음 웨이브
5. **플레이어 사망** → 게임 오버
6. **목표 달성** → 승리 (maxWaves 설정 시)

---

## ⚡ 최적화 팁

1. **Object Pooling**: Bullet과 BossProjectile에 오브젝트 풀링 적용 권장
2. **LOD**: 보스 모델에 LOD 적용
3. **Occlusion Culling**: 씬에 Occlusion Culling 적용
4. **NavMesh**: NavMesh Agent Count 제한 (한 번에 1-2개 보스만)

---

## 🐛 트러블슈팅

### 보스가 움직이지 않음
- NavMesh가 베이킹되었는지 확인
- NavMeshAgent 컴포넌트 확인
- Player Transform이 제대로 연결되었는지 확인

### 총이 발사되지 않음
- XRGrabInteractable 컴포넌트 확인
- Bullet Prefab 연결 확인
- Fire Point Transform 확인

### 무한 스크롤링이 작동하지 않음
- Player Transform (Main Camera) 연결 확인
- Tile Prefabs 배열 확인
- Tile Length 값 확인

### 피격 판정이 안됨
- Collider 설정 확인 (Is Trigger)
- Layer 설정 확인
- PlayerHealth/BossBase 컴포넌트 확인

---

## 📝 추가 개발 아이디어

1. **파워업 시스템**: 체력 회복, 데미지 증가 아이템
2. **다양한 무기**: 샷건, 레이저건, 로켓 런처
3. **보스 변종**: 각 보스마다 색상/크기 변종
4. **스코어 시스템**: 콤보, 헤드샷 보너스
5. **난이도 조절**: 시간이 지날수록 보스 강화
6. **리더보드**: 최고 기록 저장

---

## 📧 문제 발생 시

1. 콘솔 로그 확인
2. 모든 참조가 제대로 연결되었는지 확인
3. Unity XR Interaction Toolkit 버전 확인 (2.6.5 이상 권장)
4. NavMesh 베이킹 확인

---

**제작 완료!** 🎉
이제 VR 헤드셋을 착용하고 보스들을 물리치세요!
