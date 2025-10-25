# HeadTiltMovement 설정 가이드

VR에서 머리 기울기로 플레이어를 좌우로 이동시키는 시스템입니다.

---

## 📋 목차
1. [기본 설정](#기본-설정)
2. [기울기 감지 설정](#기울기-감지-설정)
3. [이동 범위 제한](#이동-범위-제한)
4. [세부 튜닝](#세부-튜닝)
5. [문제 해결](#문제-해결)

---

## 🎯 기본 설정

### 1. 플레이어 오브젝트 설정

```
Hierarchy:
  XR Origin
  ├── Camera Offset
  │   ├── Main Camera (VR HMD)
  │   ├── LeftHand Controller
  │   └── RightHand Controller
  └── (다른 컴포넌트들)
```

### 2. HeadTiltMovement 컴포넌트 추가

**어디에 추가할까?**
- **XR Origin**에 추가 (권장) - 전체 플레이어가 이동
- 또는 **Camera Offset**에 추가 - 카메라만 이동

1. XR Origin 오브젝트 선택
2. **Add Component** → `HeadTiltMovement`

### 3. 필수 설정

#### Inspector:
```
HeadTiltMovement
├── VR 카메라 설정
│   └── VR Camera: [Main Camera 드래그] (또는 비워두면 자동 찾기)
│
├── 기울기 감지 설정
│   ├── Min Tilt Angle: 15° (최소 기울기)
│   └── Max Tilt Angle: 45° (최대 기울기)
│
├── 이동 설정
│   ├── Move Speed: 3 (m/s)
│   └── Movement Smoothness: 8
│
└── 이동 범위 제한
    ├── Use Move Limit: ✓
    ├── Left Limit: -5
    └── Right Limit: 5
```

---

## 🎮 기울기 감지 설정

### 최소 기울기 각도 (Min Tilt Angle)

의도하지 않은 움직임을 방지하는 가장 중요한 설정입니다.

```
Min Tilt Angle: 15° (기본값)
```

**효과:**
- 머리를 15도 이상 기울여야 이동 시작
- 15도 미만은 무시 → 의도하지 않은 움직임 방지

**추천 설정:**
- **민감하게**: 10° (작은 기울기로 이동)
- **보통**: 15° (기본값)
- **둔감하게**: 20~25° (큰 기울기 필요)

### 최대 기울기 각도 (Max Tilt Angle)

최대 속도에 도달하는 각도입니다.

```
Max Tilt Angle: 45° (기본값)
```

**효과:**
- 45도 이상 기울이면 최대 속도로 이동
- 15°~45° 사이는 기울기에 비례해서 속도 증가

**예시:**
```
머리 기울기:  10° → 이동 없음 (최소각 미만)
머리 기울기:  15° → 천천히 이동 시작
머리 기울기:  30° → 중간 속도
머리 기울기:  45° → 최대 속도
머리 기울기:  60° → 최대 속도 유지
```

---

## 🚧 이동 범위 제한

### 범위 제한 사용

```
Use Move Limit: ✓ (체크)
Left Limit: -5 (왼쪽 한계)
Right Limit: 5 (오른쪽 한계)
```

**효과:**
- 플레이어가 X축 -5 ~ 5 범위 내에서만 이동
- 맵 밖으로 나가는 것 방지

### Scene 뷰에서 확인

```
Show Gizmos: ✓ (체크)
```

**Scene 뷰에 표시되는 것:**
- 🔴 빨간선: 왼쪽 한계 (Left Limit)
- 🔵 파란선: 오른쪽 한계 (Right Limit)
- 🟡 노란선: 중앙선 (X = 0)
- 🟢 초록 박스: 이동 가능 영역

### 범위 설정 예시

#### 좁은 복도 (1인용 레일 슈팅)
```
Left Limit: -2
Right Limit: 2
```

#### 보통 (3~5명이 서있을 정도)
```
Left Limit: -5
Right Limit: 5
```

#### 넓은 공간 (자유로운 이동)
```
Left Limit: -10
Right Limit: 10
```

#### 범위 제한 없음
```
Use Move Limit: ☐ (체크 해제)
```

---

## ⚙️ 세부 튜닝

### 이동 속도 (Move Speed)

```
Move Speed: 3 (m/s)
```

**추천 설정:**
- **느린 이동**: 1~2 m/s (탄막 회피용)
- **보통**: 3~4 m/s (기본값)
- **빠른 이동**: 5~7 m/s (빠른 액션)

### 이동 부드러움 (Movement Smoothness)

```
Movement Smoothness: 8
```

**효과:**
- 높을수록 빠르게 반응 (즉각적)
- 낮을수록 부드럽게 이동 (천천히)

**추천 설정:**
- **즉각 반응**: 15~20 (빠른 게임)
- **보통**: 8~10 (기본값)
- **부드럽게**: 3~5 (느긋한 게임)

---

## 🎮 플레이 팁

### 머리 기울이는 방법

1. **왼쪽 이동**: 머리를 왼쪽으로 기울이기
   - 왼쪽 귀를 왼쪽 어깨 쪽으로

2. **오른쪽 이동**: 머리를 오른쪽으로 기울이기
   - 오른쪽 귀를 오른쪽 어깨 쪽으로

3. **정지**: 머리를 곧게 세우기

### 주의사항

⚠️ **머리를 앞뒤로 숙이는 것이 아닙니다!**
- ❌ 고개 숙이기/올리기 (X축 회전) → 감지 안 됨
- ✅ 머리를 옆으로 기울이기 (Z축 회전) → 감지됨

---

## 🔧 스크립트에서 사용하기

### 외부에서 제어

```csharp
// HeadTiltMovement 찾기
HeadTiltMovement movement = FindObjectOfType<HeadTiltMovement>();

// 현재 기울기 각도 가져오기
float tiltAngle = movement.GetCurrentTiltAngle();
Debug.Log($"현재 기울기: {tiltAngle}도");

// 현재 이동 속도 가져오기
float velocity = movement.GetCurrentVelocity();
Debug.Log($"현재 속도: {velocity} m/s");

// 위치 리셋 (중앙으로)
movement.ResetPosition();

// 이동 활성화/비활성화
movement.SetMovementEnabled(false); // 이동 멈춤
movement.SetMovementEnabled(true);  // 이동 재개
```

### 특정 상황에서 이동 제한

```csharp
// 게임 오버 시 이동 정지
void OnGameOver()
{
    HeadTiltMovement movement = GetComponent<HeadTiltMovement>();
    movement.SetMovementEnabled(false);
}

// 보스 등장 시 중앙으로 리셋
void OnBossSpawn()
{
    HeadTiltMovement movement = GetComponent<HeadTiltMovement>();
    movement.ResetPosition();
}
```

---

## 🐛 문제 해결

### Q1. 머리를 기울여도 안 움직여요
**확인 사항:**
1. VR Camera가 제대로 연결되었는지
2. Main Camera에 "MainCamera" 태그가 있는지
3. Min Tilt Angle보다 많이 기울이고 있는지

**해결:**
```
1. Inspector에서 VR Camera 필드 확인
2. Show Debug Logs를 체크하고 Console 확인
3. Min Tilt Angle을 10도로 낮춰보기
```

### Q2. 의도하지 않게 계속 움직여요
**원인:** Min Tilt Angle이 너무 낮음

**해결:**
```
Min Tilt Angle을 20~25도로 높이기
```

### Q3. 너무 민감하게 반응해요
**해결:**
```
1. Min Tilt Angle을 높이기 (20~25도)
2. Movement Smoothness를 낮추기 (3~5)
```

### Q4. 너무 둔하게 반응해요
**해결:**
```
1. Min Tilt Angle을 낮추기 (10~12도)
2. Movement Smoothness를 높이기 (15~20)
3. Move Speed를 높이기 (5~7 m/s)
```

### Q5. 이동 범위 밖으로 나가요
**확인:**
```
Use Move Limit: ✓ (체크되어 있는지)
Left Limit, Right Limit 값 확인
```

### Q6. Scene 뷰에서 범위선이 안 보여요
**해결:**
```
Show Gizmos: ✓ (체크)
Use Move Limit: ✓ (체크)
```

### Q7. 머리를 앞뒤로 숙이면 움직이나요?
**답변:** 아니오!
- 이 시스템은 **Z축 회전(Roll)** 만 감지합니다
- 앞뒤로 숙이는 것(X축 Pitch)은 감지하지 않습니다
- 머리를 **옆으로 기울여야** 합니다

---

## 🎯 추천 설정값

### 탄막 슈팅 게임 (현재 프로젝트)
```
Min Tilt Angle: 15°
Max Tilt Angle: 40°
Move Speed: 4
Movement Smoothness: 10
Left Limit: -5
Right Limit: 5
```

**특징:** 빠른 반응, 적당한 이동 속도

### 느긋한 슈팅 게임
```
Min Tilt Angle: 20°
Max Tilt Angle: 50°
Move Speed: 2
Movement Smoothness: 5
Left Limit: -3
Right Limit: 3
```

**특징:** 부드러운 움직임, 좁은 공간

### 격렬한 액션 게임
```
Min Tilt Angle: 10°
Max Tilt Angle: 30°
Move Speed: 6
Movement Smoothness: 15
Left Limit: -8
Right Limit: 8
```

**특징:** 매우 민감, 빠른 움직임

---

## 📊 체크리스트

설정 완료 확인:
- [ ] HeadTiltMovement 컴포넌트 추가 (XR Origin)
- [ ] VR Camera 연결 (또는 자동 찾기 확인)
- [ ] Min Tilt Angle 설정 (15도 권장)
- [ ] Max Tilt Angle 설정 (45도 권장)
- [ ] Move Speed 설정 (3~4 권장)
- [ ] 이동 범위 제한 설정
- [ ] Show Gizmos 체크 (Scene 뷰 확인)
- [ ] Play 모드에서 머리 기울여보기

---

## 🚀 빠른 시작

최소 설정으로 바로 테스트:

1. XR Origin 선택
2. Add Component → HeadTiltMovement
3. Play!

→ 자동으로 Main Camera를 찾아서 기본값으로 작동합니다.

VR HMD를 쓰고 머리를 **옆으로 기울여보세요**!

---

## 💡 개선 아이디어

### 좌우 이동 표시 UI
```csharp
// UI에 현재 기울기 표시
Text tiltIndicator;

void Update()
{
    float tilt = movement.GetCurrentTiltAngle();
    tiltIndicator.text = $"기울기: {tilt:F0}°";
}
```

### 이동 중 파티클 효과
```csharp
ParticleSystem movementEffect;

void Update()
{
    float velocity = movement.GetCurrentVelocity();
    if (Mathf.Abs(velocity) > 0.1f)
    {
        if (!movementEffect.isPlaying)
            movementEffect.Play();
    }
    else
    {
        movementEffect.Stop();
    }
}
```

---

**작성일**: 2025-10-25
**버전**: 1.0
**게임 컨셉**: 탄막 슈팅 VR
