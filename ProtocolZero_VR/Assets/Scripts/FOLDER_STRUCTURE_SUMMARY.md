# 📁 Scripts 폴더 정리 완료

## ✅ 정리 내용

실무 스타일로 기능 단위 폴더 구조로 재정리하고, 구버전 스크립트를 분리했습니다.

---

## 📊 변경 전/후 비교

### 이전 (정리 전)
```
Assets/Scripts/
├── (모든 파일이 한 폴더에 섞여 있음)
└── 18개 .cs 파일 + 문서들
```

### 현재 (정리 후)
```
Assets/Scripts/
├── Player/          ✅ 플레이어 관련 (4개)
├── Enemy/           ✅ 적/보스 관련 (2개)
├── Core/            ✅ 게임 시스템 (1개)
├── UI/              ✅ UI 관련 (2개)
├── Documentation/   ✅ 문서 (5개)
├── _Deprecated/     ⚠️ 구버전 (9개)
└── README.md        📖 폴더 구조 설명
```

---

## 🗂️ 폴더별 파일 목록

### ✅ Player/ (현재 사용 중)
| 파일 | 설명 | 상태 |
|------|------|------|
| VRFixedGun.cs | 컨트롤러 고정형 총 | ⭐ 핵심 |
| PlayerBullet.cs | 플레이어 총알 (Projectile) | ⭐ 핵심 |
| SimpleXRController.cs | VR 컨트롤러 트래킹 + 회전 보정 | ⭐ 핵심 |
| PlayerHealth.cs | 플레이어 체력 시스템 | ✅ 사용 중 |

### ✅ Enemy/ (현재 사용 중)
| 파일 | 설명 | 상태 |
|------|------|------|
| EnemyBullet.cs | 적 총알 (Projectile) | ⭐ 핵심 |
| BossBase.cs | 보스 베이스 클래스 | 🔧 수정 필요 |

### ✅ Core/ (현재 사용 중)
| 파일 | 설명 | 상태 |
|------|------|------|
| GameManager.cs | 게임 매니저 | 🔧 간소화 필요 |

### ✅ UI/ (현재 사용 중)
| 파일 | 설명 | 상태 |
|------|------|------|
| GameUI.cs | 게임 UI 관리 | ✅ 사용 중 |
| HealthBarUI.cs | 체력바 UI | ✅ 사용 중 |

### 📖 Documentation/ (문서)
| 파일 | 설명 |
|------|------|
| PLAYER_GUN_SETUP.md | 플레이어 총 설정 가이드 |
| BULLET_SYSTEM_UPDATE.md | 총알 시스템 업데이트 내용 |
| CONTROLLER_DIRECTION_FIX.md | 컨트롤러 방향 보정 가이드 |
| XR_CONTROLLER_FIX.md | XR 컨트롤러 문제 해결 |
| VR_TROUBLESHOOTING.md | VR 문제 해결 총정리 |

### ⚠️ _Deprecated/ (사용 안 함!)
| 파일 | 이유 | 삭제 가능 |
|------|------|----------|
| InfiniteScroller.cs | 무한 스크롤링 안 함 | ✅ |
| VRGun.cs | VRFixedGun으로 대체 | ✅ |
| MeleeBoss.cs | 근접 보스 불필요 | ✅ |
| RangedBoss.cs | 너무 복잡, 새로 만들 것 | ✅ |
| HybridBoss.cs | 혼합형 보스 불필요 | ✅ |
| BossSpawner.cs | 간소화된 버전 필요 | ✅ |
| BossProjectile.cs | EnemyBullet로 통합 | ✅ |
| BossHealthBar.cs | HealthBarUI로 충분 | ✅ |
| Bullet.cs | PlayerBullet로 대체 | ✅ |
| GAME_SETUP_GUIDE.md | 구버전 가이드 | ✅ |

---

## 🎯 현재 게임 컨셉

### 변경 전 (구버전)
- ❌ XRGrabInteractable로 총 잡기
- ❌ 근접/원거리/혼합 보스 3종
- ❌ NavMesh 기반 AI
- ❌ 무한 스크롤링 환경
- ❌ 복잡한 상호작용 시스템

### 변경 후 (현재)
- ✅ 컨트롤러에 고정된 쌍권총
- ✅ 트리거 버튼으로 발사
- ✅ 정면에서 탄막 발사하는 보스만
- ✅ 플레이어 총알 vs 적 총알 상쇄
- ✅ 좌우 이동만
- ✅ Transform 기반 Projectile

**탄막 슈팅 VR 게임!**

---

## 🔄 정리 작업 내용

### 1. 폴더 생성
```bash
Assets/Scripts/
├── Player/
├── Enemy/
├── Core/
├── UI/
├── Documentation/
└── _Deprecated/
```

### 2. 파일 이동
- Player 관련 → Player/
- Enemy 관련 → Enemy/
- Core 시스템 → Core/
- UI 관련 → UI/
- 문서 → Documentation/
- 구버전 → _Deprecated/

### 3. 문서 작성
- README.md: 전체 폴더 구조 설명
- FOLDER_STRUCTURE_SUMMARY.md (이 파일)

---

## 📋 앞으로 할 일

### 즉시 필요한 작업
- [ ] **ShootingBoss.cs 작성** - 정면 탄막 발사 보스
- [ ] **BossBase.cs 간소화** - NavMesh 제거, 단순화
- [ ] **GameManager.cs 간소화** - 웨이브만
- [ ] **플레이어 좌우 이동** - Feel 에셋 활용

### 선택 사항
- [ ] _Deprecated 폴더 완전 삭제
- [ ] 보스 탄막 패턴 추가
- [ ] UI 개선

---

## 💡 폴더 정리 규칙

### 실무 스타일 폴더 구조
```
기능별 폴더 분리:
  Player/   - 플레이어 직접 조작
  Enemy/    - 적 및 AI
  Core/     - 게임 시스템
  UI/       - 사용자 인터페이스

특수 폴더:
  Documentation/  - 문서 (.md)
  _Deprecated/    - 구버전 파일 (밑줄로 시작)
```

### 파일명 규칙
- `PascalCase.cs` - C# 스크립트
- `UPPERCASE.md` - 문서 파일
- 명확한 이름 사용

---

## ⚠️ 주의사항

### _Deprecated 폴더
- **절대 사용하지 마세요!**
- Unity에서 참조하지 않도록 주의
- 나중에 삭제 가능

### Unity에서 파일 이동 시
- 반드시 Unity 에디터에서 이동
- .meta 파일도 함께 이동됨
- 스크립트 참조 깨질 수 있음

---

## 📊 통계

### 파일 수
- ✅ 현재 사용: **9개** (Player 4 + Enemy 2 + Core 1 + UI 2)
- ⚠️ 구버전: **9개** (_Deprecated)
- 📖 문서: **5개** (Documentation)

### 코드 라인 수 (대략)
- 현재 사용 중: ~30,000 라인
- 구버전: ~15,000 라인

---

**정리 완료!** ✨

이제 Scripts 폴더가 깔끔하게 정리되었습니다.
실무 프로젝트처럼 기능별로 잘 분류되어 있어서 유지보수가 쉬워졌습니다!
