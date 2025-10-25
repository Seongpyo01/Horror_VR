using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// VR 권총 시스템 - XR Grab Interactable과 함께 사용
/// 그랩 키를 누르면 총알 발사
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class VRGun : MonoBehaviour
{
    [Header("총 설정")]
    [Tooltip("총알 프리팹")]
    public GameObject bulletPrefab;

    [Tooltip("총알 발사 위치")]
    public Transform firePoint;

    [Tooltip("총알 속도")]
    public float bulletSpeed = 50f;

    [Tooltip("발사 간격 (초)")]
    public float fireRate = 0.2f;

    [Tooltip("데미지")]
    public float damage = 10f;

    [Tooltip("최대 탄창")]
    public int maxAmmo = 30;

    [Tooltip("현재 탄약")]
    public int currentAmmo;

    [Tooltip("자동 재장전")]
    public bool autoReload = true;

    [Tooltip("재장전 시간")]
    public float reloadTime = 1.5f;

    [Header("이펙트")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    [Header("진동 설정")]
    public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;

    private XRGrabInteractable grabInteractable;
    private bool canFire = true;
    private bool isReloading = false;
    private IXRSelectInteractor currentInteractor;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        currentAmmo = maxAmmo;

        if (firePoint == null)
        {
            // 총구 위치가 없으면 자동 생성
            GameObject fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0, 0, 0.2f);
            firePoint = fp.transform;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D 사운드
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        grabInteractable.activated.AddListener(OnTriggerPressed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        Debug.Log($"{gameObject.name} grabbed by {args.interactorObject.transform.name}");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        // 그랩 키(트리거)를 누르면 발사
        Fire();
    }

    public void Fire()
    {
        if (!canFire || isReloading)
            return;

        if (currentAmmo <= 0)
        {
            // 탄약 없음
            PlaySound(emptySound);

            if (autoReload)
            {
                StartCoroutine(Reload());
            }
            return;
        }

        // 총알 발사
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
            }
        }

        // 탄약 감소
        currentAmmo--;

        // 이펙트
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        PlaySound(shootSound);

        // 진동 피드백
        TriggerHaptic();

        // 발사 대기
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private IEnumerator Reload()
    {
        if (isReloading) yield break;

        isReloading = true;
        PlaySound(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log($"{gameObject.name} reloaded!");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void TriggerHaptic()
    {
        if (currentInteractor != null)
        {
            // XR Controller에 햅틱 피드백 전송
            if (currentInteractor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }

    // 외부에서 재장전 호출 가능
    public void ManualReload()
    {
        if (!isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    // UI 또는 디버그용
    public string GetAmmoDisplay()
    {
        return $"{currentAmmo} / {maxAmmo}";
    }
}
