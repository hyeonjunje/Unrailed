using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingSceneUI;
    [SerializeField] private MapCreator _mapCreator;

    [SerializeField] private MapManager _mapManager;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private Transform test;

    // 석환이 형 isStart가 true일 때만 player가 작동할 수 있게 해줘~~
    public bool isStart { get; private set; } = false;

    private void Awake()
    {
        FileManager.LoadGame();
        _loadingSceneUI.SetActive(true);
        isStart = false;

        LoadingFirstGame(
            () =>
            {
                _loadingSceneUI.SetActive(false);

                RePositionAsync().Forget();
            }).Forget();
    }

/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RePositionAsync().Forget();
    }*/

    /// <summary>
    /// 역 도착하면 실행될 메소드
    /// </summary>
    public void ArriveStation()
    {
        // 볼트 하나 추가 해주고
        // 조금있다가 상점보여주기
    }

    /// <summary>
    /// 역 떠나면 실행될 메소드
    /// </summary>
    public void LeaveStation()
    {
        // 나가기 게이지 100되면 실행될 메소드

        // 상점 종료되고
        // 맵 재위치 시켜주기
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _mapManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(System.Action onCreateNextMapAsyncEvent = null)
    {
        // 맵 로드
        await _mapManager.LoadMap();
        onCreateNextMapAsyncEvent?.Invoke();
    }
}
