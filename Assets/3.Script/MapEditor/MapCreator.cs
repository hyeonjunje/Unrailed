using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [SerializeField] private BlockMK2 _blockPrefab;
    [SerializeField] private Material[] _blocksMaterial;
    [SerializeField] private Transform _blockParent;
    [SerializeField] private ItemPrefabData _itemPrefabData;
    [SerializeField] private BlockTransformerData[] _blockTransformerData;
    [SerializeField] private Transform _waterFallUp;
    [SerializeField] private Transform _waterFallDown;

    // 맵은 순서대로 생성하기로 하자
    // 여기서 거리 UI 만들어??

    private List<List<BlockMK2>> groundList;

    public IEnumerator CreateMapCo(int worldIndex)
    {
        // 비동기가 필요하긴 할 듯
        MapData mapData = FileManager.MapsData.mapsData[worldIndex];

        yield return StartCoroutine(InitMapCo(mapData));

        yield return StartCoroutine(InitBlockCo(mapData));

        yield return StartCoroutine(SetHeightMapCo());

        yield return new WaitForEndOfFrame();
    }

    private IEnumerator InitMapCo(MapData mapData)
    {
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;

        int minX = (x / 2) - 1;
        int minY = (y / 2) - 1;

        groundList = new List<List<BlockMK2>>();

        for (int i = 0; i < y; i++)
        {
            groundList.Add(new List<BlockMK2>());
            for (int j = 0; j < x; j++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.transform.localPosition = new Vector3(j - minX, 0, i - minY);
                groundList[i].Add(go);
            }
        }

        yield return null;
    }

    private IEnumerator InitBlockCo(MapData mapData)
    {
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                int index = mapData.mapData[i].arr[j];
                InitBlock(groundList[i][j], index);
            }
        }
        yield return null;
    }


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
            // 위쪽이면 _waterFallUp, 밑쪽이면 _waterFallDown 생성
            if (block.transform.localPosition.z == -9)
                Instantiate(_waterFallDown.gameObject, block.transform);
            else if (block.transform.localPosition.z == groundList.Count - 10)
                Instantiate(_waterFallUp.gameObject, block.transform);
        }
    }

    private IEnumerator SetHeightMapCo()
    {
        int x = groundList[0].Count;
        int y = groundList.Count;

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if (groundList[i][j].Index == (int)EBlock.blackRock)
                {
                    int height = groundList[i][j].GetHeight();
                    Transform child = groundList[i][j].transform.GetChild(0);
                    child.localScale = Vector3.one + Vector3.up * height * 0.3f;
                }
            }
        }

        yield return null;
    }
}
