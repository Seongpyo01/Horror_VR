# VR 손 컨트롤러 문제 해결 가이드

## 🔍 문제: XR Interaction Setup을 사용할 때 손 컨트롤러가 작동하지 않음

XR Starter Assets의 **XR Interaction Setup** 프리팹을 사용할 때 VRGunController가 제대로 작동하지 않는 이유와 해결 방법입니다.

---

## ❓ 왜 작동하지 않나요?

### XR Interaction Setup의 구조
XR Starter Assets는 **Action-based** 입력 시스템을 사용합니다:

```
XR Origin (XR Rig)
├── Camera Offset
│   ├── Main Camera
│   ├── LeftHand Controller (Action-based)  ← Input Actions 사용
│   │   └── Direct Interactor
│   └── RightHand Controller (Action-based) ← Input Actions 사용
│       └── Direct Interactor
```

### 주요 차이점
1. **Input Actions**: Unity의 새로운 Input System 사용
2. **Interaction Layers**: 상호작용 레이어 설정 필요
3. **Activate 이벤트**: Input Action에서 발생

---

## ✅ 해결 방법

### 방법 1: Interaction Layers 설정 (가장 흔한 문제)

#### 1단계: Interaction Layer Mask 생성
1. **Edit → Project Settings → XR Plug-in Management → XR Interaction Toolkit**
2. **Interaction Layers** 섹션에서:
   - Layer 8: "Interactable"
   - Layer 9: "Teleport"

#### 2단계: 총 프리팹 설정
1. 총 프리팹 선택
2. **XRGrabInteractable** 컴포넌트에서:
   - **Interaction Layer Mask**: "Interactable" 선택 ✅

#### 3단계: Interactor 설정 확인
1. XR Origin → LeftHand Controller → Direct Interactor
2. **Interaction Layer Mask**: "Interactable" 선택 ✅
3. RightHand Controller도 동일하게 설정

---

### 방법 2: Input Actions 확인

XR Starter Assets는 `XRI Default Input Actions` 에셋을 사용합니다.

#### 확인 사항:
1. **Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/** 폴더 확인
2. `XRI Default Input Actions.inputactions` 파일 확인
3. XR Origin의 각 컨트롤러에서:
   - **XR Controller (Action-based)** 컴포넌트
   - **Select Action**: `XRI RightHand Interaction/Select` 매핑됨
   - **Activate Action**: `XRI RightHand Interaction/Activate` 매핑됨

#### 수정이 필요한 경우:
**XR Controller (Action-based)** 컴포넌트에서:
- **Activate Action**: 트리거 버튼에 매핑되어 있는지 확인
- **Select Action**: 그랩 버튼에 매핑되어 있는지 확인

---

### 방법 3: VRGunController 개선 버전 사용

XR Starter Assets와 100% 호환되는 개선된 버전을 제공합니다.

---

## 🎯 빠른 체크리스트

### ✅ 총 프리팹 설정
- [ ] XRGrabInteractable 컴포넌트 추가됨
- [ ] Interaction Layer Mask = "Interactable" (기본값)
- [ ] VRGunController 스크립트 추가됨
- [ ] Bullet Prefab 연결됨

### ✅ XR Origin 설정
- [ ] XR Interaction Setup 프리팹 사용 중
- [ ] Left/Right Hand Controller 존재
- [ ] Direct Interactor의 Interaction Layer Mask = "Interactable"
- [ ] XR Controller (Action-based) 컴포넌트 확인

### ✅ 입력 설정
- [ ] Input System 패키지 설치됨
- [ ] XRI Default Input Actions 에셋 존재
- [ ] Activate Action이 트리거에 매핑됨

---

## 🔧 단계별 테스트

### 1단계: 그랩 테스트
1. Play 모드 실행
2. VR 헤드셋 착용
3. **그랩 키**(옆면 버튼)를 눌러 총 잡기
4. ✅ 총이 손에 붙으면 성공
5. ❌ 총이 안 잡히면 → Interaction Layers 확인

### 2단계: 발사 테스트
1. 총을 잡은 상태에서
2. **트리거 키**(검지 버튼) 당기기
3. ✅ 총알이 발사되면 성공
4. ❌ 발사 안 되면 → Activate Action 확인

### 3단계: 콘솔 로그 확인
총을 잡았을 때 콘솔에 다음 메시지가 나와야 합니다:
```
[총 이름] grabbed by [컨트롤러 이름]
```

메시지가 안 나오면 → XRGrabInteractable 이벤트가 발생하지 않음

---

## 🐛 흔한 오류와 해결법

### 오류 1: "총을 잡을 수 없어요"
**원인**: Interaction Layer 불일치
**해결**:
- 총: Interaction Layer Mask = "Interactable"
- Interactor: Interaction Layer Mask = "Interactable"

### 오류 2: "총은 잡히는데 발사가 안돼요"
**원인**: Activate Action이 트리거에 매핑되지 않음
**해결**:
1. XR Controller (Action-based) 확인
2. Activate Action 재설정
3. 또는 아래의 개선된 VRGunController 사용

### 오류 3: "Input System이 없다는 오류가 나와요"
**원인**: Unity Input System 패키지 미설치
**해결**:
1. Window → Package Manager
2. Input System 설치
3. Project Settings → Player → Active Input Handling = "Input System Package (New)" 또는 "Both"

### 오류 4: "XRI Default Input Actions를 찾을 수 없어요"
**원인**: Starter Assets 샘플 미설치
**해결**:
1. Window → Package Manager
2. XR Interaction Toolkit 선택
3. Samples → Starter Assets → Import

---

## 📝 권장 설정값

### XRGrabInteractable (총 프리팹)
```
Movement Type: Instantaneous
Throw on Detach: false
Interaction Layer Mask: Interactable
Select Mode: Single
```

### VRGunController
```
Damage: 25
Fire Rate: 0.2
Bullet Speed: 50
Continuous Fire: true
Recoil Strength: 0.1
```

---

## 🚀 여전히 안 되면?

1. **씬 재구성**:
   - XR Origin (XR Rig) 프리팹 새로 배치
   - XR Interaction Setup 프리팹 새로 배치

2. **프로젝트 재임포트**:
   - Assets/Samples 폴더 삭제
   - Package Manager에서 Starter Assets 재설치

3. **Unity 재시작**

---

## 📞 추가 도움이 필요하면

콘솔에서 다음 정보를 확인하고 알려주세요:
1. 그랩 시 콘솔 메시지
2. 발사 시 콘솔 메시지
3. XR Interaction Toolkit 버전
4. Unity 버전
