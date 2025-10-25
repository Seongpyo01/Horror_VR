using UnityEngine;

/// <summary>
/// VR HMD 머리 기울기로 플레이어 좌우 이동
/// - 머리를 왼쪽으로 기울이면 왼쪽으로 이동
/// - 머리를 오른쪽으로 기울이면 오른쪽으로 이동
/// - 최소 기울기 각도로 의도하지 않은 움직임 방지
/// </summary>
public class HeadTiltMovement : MonoBehaviour
{
    [Header("VR 카메라 설정")]
    [Tooltip("VR 카메라 (HMD). 비어있으면 자동으로 MainCamera 찾기")]
    public Transform vrCamera;

    [Header("기울기 감지 설정")]
    [Tooltip("이동을 시작하는 최소 기울기 각도 (도)")]
    [Range(5f, 45f)]
    public float minTiltAngle = 15f;

    [Tooltip("최대 기울기 각도 - 이 각도 이상은 최대 속도로 이동")]
    [Range(10f, 60f)]
    public float maxTiltAngle = 45f;

    [Header("이동 설정")]
    [Tooltip("최대 이동 속도 (m/s)")]
    public float moveSpeed = 3f;

    [Tooltip("이동 부드러움 (낮을수록 더 부드럽게)")]
    [Range(1f, 20f)]
    public float movementSmoothness = 8f;

    [Header("이동 범위 제한")]
    [Tooltip("이동 범위 제한 사용")]
    public bool useMoveLimit = true;

    [Tooltip("왼쪽 이동 한계 (X 좌표)")]
    public float leftLimit = -5f;

    [Tooltip("오른쪽 이동 한계 (X 좌표)")]
    public float rightLimit = 5f;

    [Header("디버그")]
    [Tooltip("디버그 로그 표시")]
    public bool showDebugLogs = false;

    [Tooltip("Scene 뷰에 이동 범위 표시")]
    public bool showGizmos = true;

    private float currentVelocity = 0f; // 현재 이동 속도
    private float targetVelocity = 0f; // 목표 이동 속도

    private void Start()
    {
        // VR 카메라 자동 찾기
        if (vrCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                vrCamera = mainCam.transform;
                Debug.Log($"[HeadTiltMovement] VR Camera 자동 설정: {vrCamera.name}");
            }
            else
            {
                Debug.LogError("[HeadTiltMovement] VR Camera를 찾을 수 없습니다! MainCamera 태그를 확인하세요.");
            }
        }
    }

    private void Update()
    {
        if (vrCamera == null) return;

        // 1. 머리 기울기 각도 계산
        float tiltAngle = GetHeadTiltAngle();

        // 2. 기울기를 이동 속도로 변환
        targetVelocity = CalculateVelocity(tiltAngle);

        // 3. 부드러운 이동 속도 전환
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * movementSmoothness);

        // 4. 실제 이동 적용
        ApplyMovement(currentVelocity);

        // 5. 디버그 정보
        if (showDebugLogs && Mathf.Abs(tiltAngle) > minTiltAngle)
        {
            Debug.Log($"[HeadTiltMovement] Tilt: {tiltAngle:F1}° | Velocity: {currentVelocity:F2} m/s | Position: {transform.position.x:F2}");
        }
    }

    /// <summary>
    /// 머리 기울기 각도 계산 (Z축 회전)
    /// </summary>
    private float GetHeadTiltAngle()
    {
        // VR 카메라의 로컬 회전에서 Z축(Roll) 값 추출
        Vector3 eulerAngles = vrCamera.localRotation.eulerAngles;
        float zRotation = eulerAngles.z;

        // Unity의 각도는 0~360이므로 -180~180으로 변환
        if (zRotation > 180f)
        {
            zRotation -= 360f;
        }

        // Unity Z축 회전: 왼쪽 기울기 = +각도, 오른쪽 기울기 = -각도
        // 부호 반전: 오른쪽 기울기를 +, 왼쪽 기울기를 -로 변환
        return -zRotation;
    }

    /// <summary>
    /// 기울기 각도를 이동 속도로 변환
    /// </summary>
    private float CalculateVelocity(float tiltAngle)
    {
        // 최소 각도 이하면 이동하지 않음
        if (Mathf.Abs(tiltAngle) < minTiltAngle)
        {
            return 0f;
        }

        // 기울기를 -1 ~ 1 범위로 정규화
        float normalizedTilt;

        if (tiltAngle > 0) // 왼쪽 기울기
        {
            // minTiltAngle ~ maxTiltAngle을 0 ~ 1로 매핑
            normalizedTilt = Mathf.Clamp01((tiltAngle - minTiltAngle) / (maxTiltAngle - minTiltAngle));
        }
        else // 오른쪽 기울기
        {
            // -minTiltAngle ~ -maxTiltAngle을 0 ~ -1로 매핑
            normalizedTilt = -Mathf.Clamp01((Mathf.Abs(tiltAngle) - minTiltAngle) / (maxTiltAngle - minTiltAngle));
        }

        // 속도 계산 (왼쪽: +속도, 오른쪽: -속도)
        return normalizedTilt * moveSpeed;
    }

    /// <summary>
    /// 실제 이동 적용
    /// </summary>
    private void ApplyMovement(float velocity)
    {
        // X축 방향으로만 이동 (좌우)
        Vector3 movement = new Vector3(velocity * Time.deltaTime, 0, 0);
        Vector3 newPosition = transform.position + movement;

        // 이동 범위 제한
        if (useMoveLimit)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, leftLimit, rightLimit);
        }

        // 위치 적용
        transform.position = newPosition;
    }

    /// <summary>
    /// 현재 기울기 각도 가져오기 (외부 참조용)
    /// </summary>
    public float GetCurrentTiltAngle()
    {
        if (vrCamera == null) return 0f;
        return GetHeadTiltAngle();
    }

    /// <summary>
    /// 현재 이동 속도 가져오기 (외부 참조용)
    /// </summary>
    public float GetCurrentVelocity()
    {
        return currentVelocity;
    }

    /// <summary>
    /// 이동 범위 리셋 (중앙으로)
    /// </summary>
    public void ResetPosition()
    {
        Vector3 resetPos = transform.position;
        resetPos.x = 0f;
        transform.position = resetPos;
        currentVelocity = 0f;
        targetVelocity = 0f;

        Debug.Log("[HeadTiltMovement] 위치 리셋 완료");
    }

    /// <summary>
    /// 이동 활성화/비활성화
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        this.enabled = enabled;

        if (!enabled)
        {
            currentVelocity = 0f;
            targetVelocity = 0f;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Scene 뷰에 이동 범위 표시
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGizmos || !useMoveLimit) return;

        // 이동 범위 영역 표시
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f); // 반투명 초록색

        Vector3 centerPosition = transform.position;
        centerPosition.y = 0; // 바닥 기준

        // 왼쪽 한계선
        Vector3 leftPos = new Vector3(leftLimit, centerPosition.y, centerPosition.z);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftPos + Vector3.forward * 10, leftPos - Vector3.forward * 10);
        Gizmos.DrawWireSphere(leftPos, 0.2f);

        // 오른쪽 한계선
        Vector3 rightPos = new Vector3(rightLimit, centerPosition.y, centerPosition.z);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rightPos + Vector3.forward * 10, rightPos - Vector3.forward * 10);
        Gizmos.DrawWireSphere(rightPos, 0.2f);

        // 중앙선
        Vector3 centerPos = new Vector3(0, centerPosition.y, centerPosition.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(centerPos + Vector3.forward * 10, centerPos - Vector3.forward * 10);

        // 이동 가능 영역 박스
        Vector3 boxCenter = new Vector3((leftLimit + rightLimit) / 2f, centerPosition.y + 1f, centerPosition.z);
        Vector3 boxSize = new Vector3(rightLimit - leftLimit, 2f, 20f);
        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.DrawCube(boxCenter, boxSize);
    }

    /// <summary>
    /// 선택했을 때 더 자세한 정보 표시
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (vrCamera == null || !Application.isPlaying) return;

        // 현재 기울기 각도 표시
        float tiltAngle = GetHeadTiltAngle();

        // VR 카메라에서 플레이어로 선 그리기
        Gizmos.color = Mathf.Abs(tiltAngle) > minTiltAngle ? Color.green : Color.gray;
        Gizmos.DrawLine(vrCamera.position, transform.position);

        // 현재 위치 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // 이동 방향 화살표
        if (Mathf.Abs(currentVelocity) > 0.01f)
        {
            Vector3 velocityDirection = new Vector3(Mathf.Sign(currentVelocity), 0, 0);
            Gizmos.color = currentVelocity > 0 ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, velocityDirection * 2f);
        }
    }
#endif
}
