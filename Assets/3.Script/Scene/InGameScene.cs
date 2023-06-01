using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private int worldCount = 0;

    List<Station> stations = new List<Station>();

    private bool _isInit = false;

    // 석환이 형 isStart가 true일 때만 player가 작동할 수 있게 해줘~~
    public bool isStart { get; private set; } = false;

    private void Awake()
    {
        FileManager.LoadGame();

        // 로딩
        _loadingSceneUI.SetActive(true);
        isStart = false;

                Debug.Log(Time.realtimeSinceStartup);
        // 로딩 시작
        LoadingFirstGame(
            () =>
            {
                _loadingSceneUI.SetActive(false);

                Debug.Log(Time.realtimeSinceStartup);
                RePositionAsync(() => { SettingStation(); }).Forget();
            }).Forget();

    }

/*    private void Update()
    {
        if(isStart && !_isInit)
        {
            _isInit = true;
            SettingStation();
        }
    }*/

    /// <summary>
    /// 역 도착하면 실행될 메소드
    /// </summary>
    public void ArriveStation()
    {
        // 볼트 하나 추가 해주고

        // 조금있다가 상점보여주기
        _shopManager.ShopOn();
    }

    /// <summary>
    /// 역 떠나면 실행될 메소드
    /// </summary>
    public void LeaveStation()
    {
        // 나가기 게이지 100되면 실행될 메소드

        // 새로운 역 세팅
        _shopManager.currentStation = stations[2].transform;

        // 상점 종료되고
        _shopManager.ShopOff();

        // 맵 재위치 시켜주기
        RePositionAsync().Forget();
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

    private void SettingStation()
    {
        // 제일 오른쪽(x값이 제일 작은게 시작점)
        stations = FindObjectsOfType<Station>().OrderBy(elem => elem.transform.position.x).ToList();
        for (int i = 0; i < stations.Count; i++)
        {
            if (i == 0)
                stations[i].InitStation(true);
            else
                stations[i].InitStation(false);
        }

        _shopManager.currentStation = stations[1].transform;
    }
}
