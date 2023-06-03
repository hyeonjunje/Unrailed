using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [Header("������ ������")]
    [SerializeField] private BlockMK2 _blockPrefab;
    [SerializeField] private Transform _waterFallUp;
    [SerializeField] private Transform _waterFallDown;
    [SerializeField] private ItemPrefabData _itemPrefabData;
    [SerializeField] private BlockTransformerData[] _blockTransformerData;
    [SerializeField] private Material[] _blocksMaterial;

    [Header("AI")]
    [SerializeField] private Helper _robot;

    [Header("ETC")]
    [SerializeField] private float _intervalTimeToMoveBlockBundle = 0.1f;  // �� ������� �������� �ð�����

    // ���⼭ �Ÿ� UI �����??
    private List<List<BlockMK2>> _groundList;

    /// <summary>
    /// ���� �����ؼ� ��ġ���� �ʱ�ȭ���ݴϴ�.
    /// </summary>
    /// <param name="mapData">������ ���� ������</param>
    /// <param name="parent">���� �θ�</param>
    /// <returns>������ 2���� ����Ʈ(��)</returns>
    public async UniTask<List<List<BlockMK2>>> CreateMapAsync(MapData mapData, Transform parent, bool isInit = false)
    {
        InitMap(mapData, parent);

        if(isInit)
            PathFindingField.Instance.InitData(parent);

        await UniTask.Yield();
        InitBlock(mapData);
        await UniTask.Yield();
        SetHeightMap();



        return _groundList;
    }

    /// <summary>
    /// blockBundle ������ ���� ���ġ �˴ϴ�.
    /// </summary>
    /// <param name="blockBundleList">���ġ�� blockBundle�� ����Ʈ</param>
    /// <returns></returns>
    public async UniTask RePositionAsync(List<BlockBundle> blockBundleList)
    {
        foreach (BlockBundle blockBundle in blockBundleList)
        {
            blockBundle.RePositionAsync().Forget();
            await UniTask.Delay(System.TimeSpan.FromSeconds(_intervalTimeToMoveBlockBundle));
        }
    }

    // �� ����
    private void InitMap(MapData mapData, Transform parent)
    {
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;

        int minX = (x / 2) - 1;
        int minY = (y / 2) - 1;

        _groundList = new List<List<BlockMK2>>();

        for (int i = 0; i < y; i++)
        {
            _groundList.Add(new List<BlockMK2>());
            for (int j = 0; j < x; j++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, parent);
                go.transform.localPosition = new Vector3(j - minX, 0, i - minY);
                _groundList[i].Add(go);
            }
        }
    }

    // ������Ʈ ����
    private void InitBlock(MapData mapData)
    {
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                int index = mapData.mapData[i].arr[j];
                InitBlock(_groundList[i][j], index);
            }
        }
    }

    // ���� ����
    private void SetHeightMap()
    {
        int x = _groundList[0].Count;
        int y = _groundList.Count;

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if (_groundList[i][j].Index == (int)EBlock.blackRock)
                {
                    int height = _groundList[i][j].GetHeight();
                    Transform child = _groundList[i][j].transform.GetChild(0);
                    child.localScale = Vector3.one + Vector3.up * height * 0.3f;
                }
            }
        }
    }

    // �� ����
    private void InitBlock(BlockMK2 block, int index)
    {
        BlockTransformerData blockTransformerData = null;
        if (index == (int)EBlock.iron)
            blockTransformerData = _blockTransformerData[0];
        else if (index == (int)EBlock.blackRock)
            blockTransformerData = _blockTransformerData[1];

        Material blockMaterial = null;

        if (index > (int)EBlock.blackRock)
            blockMaterial = _blocksMaterial[(int)EBlock.grass];
        else
            blockMaterial = _blocksMaterial[index];

        block.Init(index, blockMaterial, _itemPrefabData.itemPrefabs[index], blockTransformerData);

        if (index == (int)EBlock.water)
        {
            // �����̸� _waterFallUp, �����̸� _waterFallDown ����
            if (block.transform.localPosition.z == -9)
                Instantiate(_waterFallDown.gameObject, block.transform);
            else if (block.transform.localPosition.z == _groundList.Count - 10)
                Instantiate(_waterFallUp.gameObject, block.transform);
        }
    }
}
