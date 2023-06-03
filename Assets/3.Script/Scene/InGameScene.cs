using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("�׽�Ʈ")]
    [SerializeField] private bool isTest = false;

    [Header("UI")]
     public GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private WorldManager _worldManager;

    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private BaseAI _robot;

    // ��ȯ�� �� isStart�� true�� ���� player�� �۵��� �� �ְ� ����~~
    public bool isStart { get; private set; } = false;

    private bool _isEnding = false;

    private void Awake()
    {
        // ���� �ε�
        FileManager.LoadGame();

        // �ε�
        _loadingSceneUI.SetActive(true);
        isStart = false;

        // �ε� ����
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
    /// �� �����ϸ� ����� �޼ҵ�
    /// </summary>
    public void ArriveStation()
    {
        // ���� ������ ���̶�� ����
        if(_isEnding)
        {
            Debug.Log("�����Դϴ�~~~~");
        }
        else
        {
            // ��Ʈ �ϳ� �߰� ���ְ�

            // �����ִٰ� ���������ֱ�

            _shopManager.ShopOn();
        }
    }

    /// <summary>
    /// �� ������ ����� �޼ҵ�
    /// </summary>
    public void LeaveStation()
    {
        // ������ ������ 100�Ǹ� ����� �޼ҵ�

        // ���ο� �� ����
        _shopManager.currentStation = _worldManager.stations[2].transform;

        // ���� ����ǰ�
        _shopManager.ShopOff();

        // �� ����ġ �����ֱ�
        UnitaskInvoke(1.5f, () => { RePositionAsync().Forget(); }).Forget();

        // ���� 2�� �ۿ� �����ϱ� �ѹ� ���� ������ �����غ� �Ϸ�
        _isEnding = true;
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _worldManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(bool isTest, System.Action onCreateNextMapAsyncEvent = null)
    {
        // �� ����
        await _worldManager.GenerateWorld(isTest);
        onCreateNextMapAsyncEvent?.Invoke();
    }

    private async UniTaskVoid UnitaskInvoke(float time, System.Action action)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(time));

        action?.Invoke();
    }
}
