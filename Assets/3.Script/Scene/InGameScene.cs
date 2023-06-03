using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("테스트")]
    [SerializeField] private bool isTest = false;

    [Header("UI")]
     public GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private WorldManager _worldManager;

    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private BaseAI _robot;

    // 석환이 형 isStart가 true일 때만 player가 작동할 수 있게 해줘~~
    public bool isStart { get; private set; } = false;

    private bool _isEnding = false;

    private void Awake()
    {
        // 게임 로드
        FileManager.LoadGame();

        // 로딩
        _loadingSceneUI.SetActive(true);
        isStart = false;

        // 로딩 시작
        LoadingFirstGame(isTest, 
            () =>
            {
                _loadingSceneUI.SetActive(false);
                RePositionAsync(
                    () =>
                    {
                        Instantiate(_robot, Vector3.up * 0.5f, Quaternion.identity).SetHome(FindObjectOfType<Resource>());
                    }).Forget();

                _shopManager.StartTrainMove();
            }).Forget();
    }


    /// <summary>
    /// 역 도착하면 실행될 메소드
    /// </summary>
    public void ArriveStation()
    {
        // 만약 마지막 역이라면 엔딩
        if(_isEnding)
        {
            Debug.Log("엔딩입니다~~~~");
        }
        else
        {
            // 볼트 하나 추가 해주고

            // 조금있다가 상점보여주기

            _shopManager.ShopOn();
        }
    }

    /// <summary>
    /// 역 떠나면 실행될 메소드
    /// </summary>
    public void LeaveStation()
    {
        // 나가기 게이지 100되면 실행될 메소드

        // 새로운 역 세팅
        _shopManager.currentStation = _worldManager.stations[2].transform;

        // 상점 종료되고
        _shopManager.ShopOff();

        // 맵 재위치 시켜주기
        UnitaskInvoke(1.5f, () => { RePositionAsync().Forget(); }).Forget();

        // 맵은 2개 밖에 없으니까 한번 역을 떠나면 엔딩준비 완료
        _isEnding = true;
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _worldManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(bool isTest, System.Action onCreateNextMapAsyncEvent = null)
    {
        // 맵 생성
        await _worldManager.GenerateWorld(isTest);
        onCreateNextMapAsyncEvent?.Invoke();
    }

    private async UniTaskVoid UnitaskInvoke(float time, System.Action action)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(time));

        action?.Invoke();
    }
}
