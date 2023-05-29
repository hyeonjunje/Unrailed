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

    // ��ȯ�� �� isStart�� true�� ���� player�� �۵��� �� �ְ� ����~~
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

    // �������� �����ϱ� ������ 50% ���� ���� �� �������� �󿬾�~~
    // �ε� �� �ϰ� �ʿ��Ѱ� ������ ���ڷ� Action �־ ��~
    // ����ó�� �����ϱ� ������ 50% �Ѱ� ������ �����ٰ� �ٽ� ���͵� �ƹ� ���� �����ž�~
    public void CreateMap(System.Action onFinishedAsyncEvent = null)
    { 
        if(isGenerate == false)
        {
            isGenerate = true;
            CreateNextMapAsync(onFinishedAsyncEvent).Forget();
        }
    }

    // ������ ������ �پ�� �� �� �پ��� �̰� ��������
    // �� ��ġ �����ϴ°Űŵ�. �� ��ġ ���� �� ������ �ʿ��Ѱ� ������ ���ڷ� Action �־ ��~
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
