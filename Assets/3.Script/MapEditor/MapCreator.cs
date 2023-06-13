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
    [SerializeField] private Transform _barricade;
    [SerializeField] private ItemPrefabData _itemPrefabData;
    [SerializeField] private BlockTransformerData[] _blockTransformerData;
    [SerializeField] private Material[] _blocksMaterial;

    [Header("AI")]
    [SerializeField] private Helper _robot;

    [Header("ETC")]
    [SerializeField] private float _intervalTimeToMoveBlockBundle = 0.1f;  // �� ������� �������� �ð�����

    [Header("����")]
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

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
        await UniTask.WaitForEndOfFrame(this);
        if (isInit)
            PathFindingField.Instance.InitData(parent);

        await UniTask.WaitForEndOfFrame(this);
        InitBlock(mapData);
        await UniTask.WaitForEndOfFrame(this);
        SetHeightMap();
        await UniTask.WaitForEndOfFrame(this);


        return _groundList;
    }

    /// <summary>
    /// blockBundle ������ ���� ���ġ �˴ϴ�.
    /// </summary>
    /// <param name="blockBundleList">���ġ�� blockBundle�� ����Ʈ</param>
    /// <returns></returns>
    public async UniTask RePositionAsync(List<BlockBundle> blockBundleList)
    {
            SoundManager.Instance.PlaySoundEffect("Btn_MapCreate");
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
                    child.localScale = Vector3.one + Vector3.up * height * Random.Range(minHeight, maxHeight);
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

        if (index > (int)EBlock.blackRock && index < (int)EBlock.duck)
            blockMaterial = _blocksMaterial[(int)EBlock.grass];
        else if (index >= (int)EBlock.duck)
            blockMaterial = _blocksMaterial[(int)EBlock.water];
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

    // �����ڸ� �ٸ����̵� ����
    public List<Transform> SetBarricade(Transform startTransform, Transform endTransform)
    {
        List<Transform> betweenVerticalBarricadeList = new List<Transform>();
        Transform parent = new GameObject("BarricadeParent").transform;

        int minX = Mathf.RoundToInt(startTransform.position.x);
        int maxX = Mathf.RoundToInt(endTransform.position.x);

        int minY = Mathf.RoundToInt(startTransform.position.z);
        int maxY = Mathf.RoundToInt(endTransform.position.z);

        // ���� ������
        for(int i = minY; i <= maxY; i++)
        {
            Vector3 pos = new Vector3((minX + maxX) / 2 + 1, 0.5f, i);
            betweenVerticalBarricadeList.Add(Instantiate(_barricade, pos, Quaternion.identity, parent));
        }

        // ���� ����
        for (int i = minX; i <= maxX; i++)
        {
            Vector3 pos = new Vector3(i, 0.5f, minY - 1);
            Instantiate(_barricade, pos, Quaternion.identity, parent);
        }

        // ū ����
        for (int i = minX; i <= maxX; i++)
        {
            Vector3 pos = new Vector3(i, 0.5f, maxY + 1);
            Instantiate(_barricade, pos, Quaternion.identity, parent);
        }

        // ���� ������
        for (int i = minY; i <= maxY; i++)
        {
            Vector3 pos = new Vector3(minX - 1, 0.5f, i);
            Instantiate(_barricade, pos, Quaternion.identity, parent);
        }

        // ū ����
        for (int i = minY; i <= maxY; i++)
        {
            Vector3 pos = new Vector3(maxX + 1, 0.5f, i);
            Instantiate(_barricade, pos, Quaternion.identity, parent);
        }

        return betweenVerticalBarricadeList;
    }
}
