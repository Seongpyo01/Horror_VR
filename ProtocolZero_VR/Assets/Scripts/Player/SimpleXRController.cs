using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 간단한 XR 컨트롤러 트래킹
/// XR Interaction Toolkit 없이도 작동
///
/// 사용법:
/// LeftHand Controller와 RightHand Controller에 각각 추가
/// </summary>
public class SimpleXRController : MonoBehaviour
{
    [Header("컨트롤러 설정")]
    [Tooltip("왼손(LeftHand) 또는 오른손(RightHand)")]
    public XRNode controllerNode = XRNode.RightHand;

    [Header("회전 보정")]
    [Tooltip("컨트롤러 회전 보정 (총이 정면을 향하도록)")]
    public Vector3 rotationOffset = new Vector3(-60f, 0f, 0f);

    [Tooltip("회전 보정 활성화")]
    public bool applyRotationOffset = true;

    [Header("디버그")]
    public bool showDebugInfo = false;
    public bool showGizmo = true;

    private InputDevice targetDevice;

    private void Start()
    {
        // 컨트롤러 찾기
        InitializeController();
    }

    private void Update()
    {
        // 컨트롤러가 유효하지 않으면 재탐색
        if (!targetDevice.isValid)
        {
            InitializeController();
        }

        // 컨트롤러 위치/회전 업데이트
        UpdatePose();
    }

    private void InitializeController()
    {
        targetDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (targetDevice.isValid)
        {
            string handName = controllerNode == XRNode.LeftHand ? "Left" : "Right";
            Debug.Log($"[SimpleXRController] {handName} hand controller found: {targetDevice.name}");
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning($"[SimpleXRController] Controller not found for {controllerNode}");
        }
    }

    private void UpdatePose()
    {
        if (!targetDevice.isValid)
            return;

        // 위치 업데이트
        Vector3 position;
        if (targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out position))
        {
            transform.localPosition = position;

            if (showDebugInfo)
                Debug.Log($"[SimpleXRController] {controllerNode} position: {position}");
        }

        // 회전 업데이트
        Quaternion rotation;
        if (targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
        {
            // 회전 보정 적용
            if (applyRotationOffset)
            {
                Quaternion offset = Quaternion.Euler(rotationOffset);
                transform.localRotation = rotation * offset;
            }
            else
            {
                transform.localRotation = rotation;
            }

            if (showDebugInfo)
                Debug.Log($"[SimpleXRController] {controllerNode} rotation: {transform.localRotation.eulerAngles}");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        // 컨트롤러의 정면 방향 표시
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 0.5f);

        // 컨트롤러의 위 방향 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up * 0.3f);

        // 컨트롤러의 오른쪽 방향 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 0.3f);
    }

    // 디버그용: 현재 컨트롤러 상태 출력
    [ContextMenu("Print Controller Info")]
    public void PrintControllerInfo()
    {
        Debug.Log($"=== {controllerNode} Controller Info ===");
        Debug.Log($"Device Valid: {targetDevice.isValid}");
        Debug.Log($"Device Name: {targetDevice.name}");
        Debug.Log($"Position: {transform.localPosition}");
        Debug.Log($"Rotation: {transform.localRotation.eulerAngles}");
        Debug.Log($"================================");
    }
}
