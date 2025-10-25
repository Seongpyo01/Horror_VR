# VR 컨트롤러 트래킹 문제 해결

## 🔍 문제: 컨트롤러가 손을 따라오지 않음

XR Origin을 생성했는데 컨트롤러가 실제 손 위치를 추적하지 않는 경우

---

## ✅ 해결 방법

### 방법 1: XR Controller (Action-based) 추가 ⭐ 권장

XR Interaction Toolkit이 설치된 경우:

#### 왼손 컨트롤러
1. **LeftHand Controller** 선택
2. **Add Component** 클릭
3. **XR Controller (Action-based)** 추가
4. 설정:
   ```
   Position Action: XRI LeftHand/Position
   Rotation Action: XRI LeftHand/Rotation
   ```

#### 오른손 컨트롤러
1. **RightHand Controller** 선택
2. **Add Component** 클릭
3. **XR Controller (Action-based)** 추가
4. 설정:
   ```
   Position Action: XRI RightHand/Position
   Rotation Action: XRI RightHand/Rotation
   ```

---

### 방법 2: Tracked Pose Driver 사용 (XR Interaction Toolkit 없는 경우)

Unity 기본 기능만 사용:

#### 왼손 컨트롤러
1. **LeftHand Controller** 선택
2. **Add Component** → **Tracked Pose Driver**
3. 설정:
   ```
   Device: Generic XR Controller
   Pose Source: Left Controller
   Tracking Type: Rotation And Position
   Update Type: Update And Before Render
   ```

#### 오른손 컨트롤러
1. **RightHand Controller** 선택
2. **Add Component** → **Tracked Pose Driver**
3. 설정:
   ```
   Device: Generic XR Controller
   Pose Source: Right Controller
   Tracking Type: Rotation And Position
   ```

---

### 방법 3: 프리팹 교체 (가장 빠름!)

기본 XR Origin 대신 제대로 설정된 프리팹 사용:

1. **Package Manager** 열기 (Window → Package Manager)
2. **XR Interaction Toolkit** 찾기
3. **Samples** 탭에서 **Starter Assets** Import
4. **Prefabs** 폴더에서:
   - **XR Origin (XR Rig).prefab** 사용

이 프리팹은 모든 설정이 완료되어 있습니다!

---

## 🔧 프로젝트 설정 확인

### 1. XR Plugin Management
**Edit → Project Settings → XR Plug-in Management**

사용할 VR 플랫폼 체크:
- ✅ Oculus (Meta Quest)
- ✅ OpenXR
- ✅ Windows Mixed Reality

### 2. Input System
**Edit → Project Settings → Player → Active Input Handling**

설정:
- **Input System Package (New)** 또는
- **Both** (구 + 신 모두)

### 3. XR Interaction Toolkit 설치 확인
**Window → Package Manager**
- **XR Interaction Toolkit** 설치되어 있는지 확인
- 버전 2.0.0 이상 권장

---

## 🎯 권장 설정 (최종)

### 간단한 방법 (추천!)

기본 XR Origin 대신:

```
Packages/XR Interaction Toolkit/Starter Assets/Prefabs/
└── XR Origin (XR Rig).prefab  ← 이것 사용!
```

이 프리팹을 씬에 드래그하면:
- ✅ 컨트롤러 트래킹 자동
- ✅ Input Actions 설정됨
- ✅ 모든 기능 작동

그 다음:
- LeftHand Controller 하위에 LeftGun 배치
- RightHand Controller 하위에 RightGun 배치

끝!

---

## 🧪 테스트

Play 모드에서:
1. VR 헤드셋 착용
2. 손을 움직여보기
3. ✅ Scene 뷰에서 컨트롤러가 따라오는지 확인

---

## ⚠️ 여전히 안 되면?

### 디버깅 단계

1. **콘솔 확인**:
   ```
   [XR] Failed to initialize XR Plugin...
   ```
   → XR Plugin 설정 확인

2. **컨트롤러 연결 확인**:
   - VR 기기가 PC에 연결되었는지
   - 컨트롤러 배터리 확인
   - Oculus/SteamVR 앱 실행 중인지

3. **Unity 재시작**:
   - XR 설정 변경 후 Unity 재시작 필요할 수 있음

---

## 📝 요약

### 가장 빠른 해결법
1. XR Interaction Toolkit Starter Assets Import
2. XR Origin (XR Rig) 프리팹 사용
3. LeftGun/RightGun을 컨트롤러에 배치

### 수동 설정
1. XR Controller (Action-based) 컴포넌트 추가
2. Position/Rotation Action 설정
3. Input Actions 연결
