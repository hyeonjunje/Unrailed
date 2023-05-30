using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingSceneUI;
    [SerializeField] private MapCreator _mapCreator;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private Transform test;

    // 석환이 형 isStart가 true일 때만 player가 작동할 수 있게 해줘~~
    public bool isStart { get; private set; } = false;

    private bool isGenerate = false;

    private void Awake()
    {
        FileManager.LoadGame();
        _loadingSceneUI.SetActive(true);
        isStart = false;

        LoadingFirstGame(
            () =>
            {
                _loadingSceneUI.SetActive(false);
            },
            () =>
            {
                isStart = true;
            }).Forget();
    }

    // 상점에서 게임하기 게이지 50% 정도 됐을 때 실행해줘 상연아~~
    // 로딩 다 하고 필요한거 있으면 인자로 Action 넣어도 돼~
    // 예외처리 했으니까 게이지 50% 넘고 게이지 나갔다가 다시 들어와도 아무 문제 없을거야~
    public void CreateMap(System.Action onFinishedAsyncEvent = null)
    { 
        if(isGenerate == false)
        {
            isGenerate = true;
            CreateNextMapAsync(onFinishedAsyncEvent).Forget();
        }
    }

    // 상점이 원으로 줄어들 때 다 줄어들면 이거 실행해줘
    // 맵 위치 설정하는거거든. 맵 위치 설정 다 끝나고 필요한거 있으면 인자로 Action 넣어도 돼~
    public void RePositionMap(System.Action onFinishedAsyncEvent = null)
    {
        RePositionAsync(onFinishedAsyncEvent).Forget();
        isGenerate = false;
    }

    private async UniTaskVoid CreateNextMapAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _mapCreator.CreateMapAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _mapCreator.RePositionAsync();
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(System.Action onCreateNextMapAsyncEvent = null, System.Action onPositionAsyncEvent = null)
    {
        await _mapCreator.CreateMapAsync(worldCount++);
        onCreateNextMapAsyncEvent?.Invoke();

        await _mapCreator.RePositionAsync();
        onPositionAsyncEvent?.Invoke();
    }
}
