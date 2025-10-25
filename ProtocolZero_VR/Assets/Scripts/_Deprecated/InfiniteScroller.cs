using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무한 스크롤링 환경 시스템
/// 플레이어가 앞으로 이동하면 뒤의 타일을 앞으로 재배치하여 무한 이동 효과 생성
/// </summary>
public class InfiniteScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    [Tooltip("플레이어(VR 카메라) Transform")]
    public Transform player;

    [Tooltip("자동 스크롤 활성화")]
    public bool autoScroll = true;

    [Tooltip("자동 스크롤 속도")]
    public float scrollSpeed = 5f;

    [Header("타일 설정")]
    [Tooltip("반복할 타일 프리팹 배열")]
    public GameObject[] tilePrefabs;

    [Tooltip("한 번에 활성화되는 타일 수")]
    public int numberOfTiles = 5;

    [Tooltip("각 타일의 길이 (Z축)")]
    public float tileLength = 20f;

    [Tooltip("타일 재배치 거리 (플레이어가 이 거리만큼 이동하면 재배치)")]
    public float recycleOffset = 10f;

    [Header("랜덤 생성 설정")]
    public bool randomizeTiles = true;
    public bool randomizeRotation = false;

    private List<GameObject> activeTiles = new List<GameObject>();
    private float nextTilePosition = 0f;
    private Vector3 playerStartPosition;

    private void Start()
    {
        if (player == null)
        {
            // XR Origin 카메라 자동 찾기
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                player = mainCam.transform;
            }
            else
            {
                Debug.LogError("InfiniteScroller: Player Transform을 찾을 수 없습니다!");
                return;
            }
        }

        playerStartPosition = player.position;

        // 초기 타일 생성
        InitializeTiles();
    }

    private void Update()
    {
        if (player == null) return;

        // 자동 스크롤
        if (autoScroll)
        {
            Vector3 moveDirection = Vector3.forward * scrollSpeed * Time.deltaTime;
            player.position += moveDirection;
        }

        // 타일 재배치 체크
        CheckTileRecycle();
    }

    private void InitializeTiles()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogError("InfiniteScroller: 타일 프리팹이 설정되지 않았습니다!");
            return;
        }

        // 초기 타일 생성
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    private void SpawnTile()
    {
        GameObject tilePrefab = GetRandomTilePrefab();
        if (tilePrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.forward * nextTilePosition;
        Quaternion rotation = Quaternion.identity;

        if (randomizeRotation)
        {
            rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
        }

        GameObject newTile = Instantiate(tilePrefab, spawnPosition, rotation, transform);
        activeTiles.Add(newTile);

        nextTilePosition += tileLength;
    }

    private void CheckTileRecycle()
    {
        if (activeTiles.Count == 0) return;

        // 가장 뒤에 있는 타일 확인
        GameObject oldestTile = activeTiles[0];
        float distanceTraveled = player.position.z - playerStartPosition.z;

        // 플레이어가 타일을 지나쳤는지 확인
        if (oldestTile.transform.position.z + tileLength + recycleOffset < player.position.z)
        {
            // 타일을 앞으로 재배치
            RecycleTile(oldestTile);
        }
    }

    private void RecycleTile(GameObject tile)
    {
        // 타일을 리스트에서 제거하고 뒤로 이동
        activeTiles.Remove(tile);

        // 새 위치 계산
        Vector3 newPosition = transform.position + Vector3.forward * nextTilePosition;
        tile.transform.position = newPosition;

        if (randomizeRotation)
        {
            tile.transform.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);
        }

        // 리스트 끝에 추가
        activeTiles.Add(tile);

        nextTilePosition += tileLength;
    }

    private GameObject GetRandomTilePrefab()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0) return null;

        if (randomizeTiles && tilePrefabs.Length > 1)
        {
            return tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        }
        else
        {
            return tilePrefabs[0];
        }
    }

    // 외부에서 스크롤 속도 조정
    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }

    public void EnableAutoScroll(bool enable)
    {
        autoScroll = enable;
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            // 재배치 거리 시각화
            Gizmos.color = Color.yellow;
            Vector3 recyclePoint = player.position - Vector3.forward * recycleOffset;
            Gizmos.DrawWireCube(recyclePoint, new Vector3(10f, 0.1f, 0.5f));
        }
    }
}
