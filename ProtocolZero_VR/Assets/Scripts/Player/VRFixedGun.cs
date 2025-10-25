using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// VR 컨트롤러에 고정된 총
/// 트리거 버튼을 누르면 발사
///
/// 사용법:
/// 1. 왼손/오른손 컨트롤러의 자식 오브젝트로 배치
/// 2. Controller Node 설정 (LeftHand 또는 RightHand)
/// 3. Bullet Prefab 연결
/// </summary>
public class VRFixedGun : MonoBehaviour
{
    [Header("컨트롤러 설정")]
    [Tooltip("왼손(LeftHand) 또는 오른손(RightHand)")]
    public XRNode controllerNode = XRNode.RightHand;

    [Header("총 설정")]
    [Tooltip("발사 데미지")]
    public float damage = 25f;

    [Tooltip("발사 간격 (초)")]
    public float fireRate = 0.15f;

    [Tooltip("총알 속도")]
    public float bulletSpeed = 30f;

    [Tooltip("총알 생존 시간")]
    public float bulletLifetime = 5f;

    [Header("참조")]
    [Tooltip("총알 발사 위치 (없으면 자동 생성)")]
    public Transform muzzle;

    [Tooltip("플레이어 총알 프리팹")]
    public GameObject bulletPrefab;

    [Tooltip("총구 이펙트")]
    public ParticleSystem muzzleFlash;

    [Tooltip("발사 사운드")]
    public AudioClip fireSound;

    [Header("햅틱 피드백")]
    [Tooltip("발사 시 진동 강도 (0~1)")]
    [Range(0f, 1f)]
    public float hapticIntensity = 0.3f;

    [Header("디버그")]
    public bool showDebugLogs = false;

    private InputDevice controller;
    private AudioSource audioSource;
    private float lastFireTime = 0f;

    private void Start()
    {
        // 컨트롤러 찾기
        InitializeController();

        // AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D 사운드
        }

        // 총구가 없으면 자동 생성
        if (muzzle == null)
        {
            GameObject muzzleObj = new GameObject("Muzzle");
            muzzleObj.transform.SetParent(transform);
            muzzleObj.transform.localPosition = new Vector3(0, 0, 0.2f);
            muzzleObj.transform.localRotation = Quaternion.identity;
            muzzle = muzzleObj.transform;

            if (showDebugLogs)
                Debug.Log($"[VRFixedGun] Auto-created muzzle for {controllerNode}");
        }

        string handName = controllerNode == XRNode.LeftHand ? "Left" : "Right";
        Debug.Log($"[VRFixedGun] {handName} hand gun initialized");
    }

    private void InitializeController()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (!controller.isValid && showDebugLogs)
        {
            Debug.LogWarning($"[VRFixedGun] Controller not found for {controllerNode}");
        }
    }

    private void Update()
    {
        // 컨트롤러가 유효하지 않으면 재탐색
        if (!controller.isValid)
        {
            InitializeController();
            return;
        }

        // 트리거 버튼 확인
        bool triggerPressed = false;
        if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed))
        {
            if (triggerPressed)
            {
                TryFire();
            }
        }
    }

    private void TryFire()
    {
        // 연사 속도 제한
        if (Time.time - lastFireTime < fireRate)
        {
            return;
        }

        Fire();
        lastFireTime = Time.time;
    }

    private void Fire()
    {
        // 필수 요소 확인
        if (bulletPrefab == null || muzzle == null)
        {
            Debug.LogError("[VRFixedGun] Cannot fire: bulletPrefab or muzzle is null!");
            return;
        }

        // 컨트롤러의 정확한 방향 사용
        Vector3 firePosition = muzzle.position;
        Vector3 fireDirection = muzzle.forward;

        // 총알 생성 (위치와 회전 모두 컨트롤러 기준)
        GameObject bullet = Instantiate(bulletPrefab, firePosition, muzzle.rotation);

        // 총알 초기화 (방향 벡터 전달)
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.Initialize(fireDirection, damage, bulletSpeed, bulletLifetime);
        }
        else
        {
            // PlayerBullet 컴포넌트가 없으면 경고
            Debug.LogWarning("[VRFixedGun] Bullet prefab missing PlayerBullet component!");
            Destroy(bullet, bulletLifetime);
        }

        // 이펙트 재생
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // 사운드 재생
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        // 햅틱 피드백
        SendHapticFeedback();

        if (showDebugLogs)
        {
            Debug.Log($"[VRFixedGun] {controllerNode} fired!");
            Debug.Log($"  Position: {firePosition}");
            Debug.Log($"  Direction: {fireDirection}");
            Debug.Log($"  Muzzle Forward: {muzzle.forward}");
        }
    }

    private void SendHapticFeedback()
    {
        if (controller.isValid)
        {
            // 햅틱 진동 보내기 (채널 0, 강도, 지속시간)
            controller.SendHapticImpulse(0, hapticIntensity, 0.1f);
        }
    }

    // 디버그용: 총구 방향 표시
    private void OnDrawGizmos()
    {
        if (muzzle != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(muzzle.position, muzzle.forward * 3f);
            Gizmos.DrawWireSphere(muzzle.position, 0.03f);
        }
    }

    // Inspector에서 수동 발사 테스트
    [ContextMenu("Test Fire")]
    public void TestFire()
    {
        Fire();
    }

    /// <summary>
    /// UI용 탄약 표시 (무한 탄약이므로 ∞ 반환)
    /// </summary>
    public string GetAmmoDisplay()
    {
        return "∞"; // 무한 탄약
    }
}
