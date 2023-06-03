using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    [Header("Map ����")]
    // �� �Ӹ� �ƴ϶� ��ü�� �����ϴ� Ŭ����
    [SerializeField] private MapCreator _mapCreator;
    [SerializeField] private MapSlicer _mapSlicer;

    [Header("������Ʈ ����")]
    // �÷��̾�, ����, ai
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private GameObject _trainPrefab;
    // aiaiai

    // �� ������ ����
    private int _worldCount = 0;

    // ��ü �� ����
    public List<List<BlockMK2>>[] entireMap { get; private set; } = new List<List<BlockMK2>>[2];
    // ��ü �� ���� ����
    public List<BlockBundle>[] entireMapBlockBundle { get; private set; } = new List<BlockBundle>[2];
    // ��ü ���������Ʈ ���� (�÷��̾�, ����, ai)
    public List<List<Transform>> worldObject { get; private set; } = new List<List<Transform>>();
    // ������ ������ ����Ʈ
    public List<Station> stations { get; private set; } = new List<Station>();

    public async UniTask GenerateWorld(bool isTest)
    {
        // �� �α׸� ����
        MapData[] mapData = new MapData[2];
        _worldCount = mapData.Length;

        if (isTest)
        {
            mapData[0] = FileManager.MapsData.mapsData[3];
            mapData[1] = FileManager.MapsData.mapsData[4];
        }
        else
        {
            mapData[0] = FileManager.MapsData.mapsData[5];
            mapData[1] = FileManager.MapsData.mapsData[7];
        }

        for (int i = 0; i < _worldCount; i++)
        {
            // �θ��� ��ġ ����
            float width = mapData[i].mapData[0].arr.Length;
            Vector3 parentPosition = Vector3.right * width * i;
            Transform currentParent = new GameObject("World " + i).transform;
            currentParent.position = parentPosition;

            entireMap[i] = await _mapCreator.CreateMapAsync(mapData[i], currentParent, i == 0);
        }

        // ���� ������ ������Ʈ �ʱ�ȭ (�÷��̾�, ��, ����, AI)
        await InitWorldObject();

        // �� �ڸ��� �ʱ���ġ �̵�
        for (int i = 0; i < mapData.Length; i++)
        {
            entireMapBlockBundle[i] = await _mapSlicer.SliceMap(entireMap[i]);
        }

        // ������ ��ٸ���
        await UniTask.WaitForEndOfFrame(this);
    }

    // �� ����ġ ��Ű��
    public async UniTask RePositionAsync(int index)
    {
        await _mapCreator.RePositionAsync(entireMapBlockBundle[index]);
    }


    private async UniTask InitWorldObject()
    {
        // �� �ʱ�ȭ �ϸ鼭 �������� ����!! (���� ������ �����ϱ�!!)
        stations = FindObjectsOfType<Station>().OrderBy(elem => elem.transform.position.x).ToList();
        for (int i = 0; i < stations.Count; i++)
        {
            if (i == 0)
                stations[i].InitStation(true, _trainPrefab);
            else
                stations[i].InitStation(false);
        }
        FindObjectOfType<ShopManager>().currentStation = stations[1].transform;


        // �÷��̾� ����, ���� ����, AI ����

        await UniTask.Yield();
    }
}
