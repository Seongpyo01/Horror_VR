using UnityEngine;

/// <summary>
/// 여러 개의 배경 조각(Pieces)을 배열로 받아 Z축으로 무한 스크롤시킵니다.
/// 카메라/플레이어는 z=0 근처에 고정되어 있고 배경이 뒤로 움직이는 방식입니다.
/// </summary>
public class InfiniteBackgroundScroller : MonoBehaviour
{
    [Tooltip("연속으로 이어붙일 배경 오브젝트 배열")]
    public GameObject[] backgroundPieces;

    [Tooltip("배경이 스크롤되는 속도 (초당 월드 유닛)")]
    public float scrollSpeed = 5.0f;

    [Tooltip("배경 조각 하나의 Z축 방향 길이. (매우 중요!)")]
    public float backgroundLength = 20.0f;

    // 모든 배경 조각을 합친 총 길이
    private float totalLength;

    // 배경 조각을 텔레포트시킬 Z축 경계선
    // (예: z=0에서 시작한 조각이 z=-20에 도달하면 맨 뒤로 보냄)
    private float teleportBoundaryZ;

    void Start()
    {
        if (backgroundPieces == null || backgroundPieces.Length == 0 || backgroundLength <= 0)
        {
            Debug.LogError("InfiniteBackgroundScroller: 'Background Pieces' 배열이 비어있거나 'Background Length'가 0 이하입니다. 스크립트를 비활성화합니다.");
            enabled = false; // 이 스크립트의 Update 실행 중지
            return;
        }

        // 1. 전체 길이 계산
        totalLength = backgroundLength * backgroundPieces.Length;

        // 2. 텔레포트 경계 설정 (첫 번째 조각의 시작 위치(0) - 조각 길이)
        // 이 경계는 조각의 중심(pivot)을 기준으로 합니다.
        teleportBoundaryZ = -backgroundLength;

        // 3. (선택적) 초기 위치 설정 (필요시 주석 해제)
        // 이 스크립트가 자동으로 초기 위치를 잡아주도록 할 수 있습니다.
        /*
        for (int i = 0; i < backgroundPieces.Length; i++)
        {
            Vector3 startPos = new Vector3(0, 0, i * backgroundLength);
            backgroundPieces[i].transform.position = startPos;
        }
        */
    }

    void Update()
    {
        foreach (GameObject piece in backgroundPieces)
        {
            // 1. 각 배경 조각을 Z축 뒤쪽(Vector3.back)으로 이동시킵니다.
            // Space.World를 기준으로 하여, 부모 오브젝트의 회전에 영향을 받지 않게 합니다.
            piece.transform.Translate(Vector3.back * scrollSpeed * Time.deltaTime, Space.World);

            // 2. 텔레포트 경계를 확인합니다.
            // 조각의 Z 위치가 설정된 경계선(teleportBoundaryZ)보다 뒤로 갔는지 확인합니다.
            if (piece.transform.position.z <= teleportBoundaryZ)
            {
                // 3. 경계를 지난 조각을 맨 뒤로 텔레포트시킵니다.
                // 현재 위치에서 전체 배경 길이(totalLength)만큼 Z축으로 더해줍니다.
                Vector3 newPos = piece.transform.position;
                newPos.z += totalLength;
                piece.transform.position = newPos;
            }
        }
    }
}