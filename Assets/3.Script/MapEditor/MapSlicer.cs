using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBundle
{
    private List<BlockMK2> _blockBundle;
    private float _moveSpeed;

    public BlockBundle(float moveSpeed = 5)
    {
        _blockBundle = new List<BlockMK2>();
        _moveSpeed = moveSpeed;
    }

    public void AddBlock(BlockMK2 block)
    {
        _blockBundle.Add(block);
    }

    public void SetPositionBundle(float initPos)
    {
        foreach (BlockMK2 block in _blockBundle)
            block.transform.localPosition += Vector3.up * initPos;
    }

    public async UniTaskVoid RePositionAsync()
    {
        float currentTime = 0f;

        float originY = _blockBundle[0].transform.localPosition.y;

        List<Vector3> originPosList = new List<Vector3>();
        List<Vector3> targetPosList = new List<Vector3>();

        foreach(BlockMK2 block in _blockBundle)
        {
            originPosList.Add(block.transform.localPosition);
            targetPosList.Add(new Vector3(block.transform.localPosition.x, 0, block.transform.localPosition.z));
        }

        while (currentTime * _moveSpeed < 1)
        {
            currentTime += Time.deltaTime;

            for(int i = 0; i < _blockBundle.Count; i++)
                _blockBundle[i].transform.localPosition = Vector3.Lerp(originPosList[i], targetPosList[i], currentTime * _moveSpeed);

            await UniTask.Yield();
        }
    }
}

public class MapSlicer : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _initPos = 40f;

    public async UniTask<List<BlockBundle>> SliceMap(List<List<BlockMK2>> groundList)
    {
        int width = groundList[0].Count;
        int height = groundList.Count;
        List<BlockBundle> separatedBlockList = new List<BlockBundle>();

        List<int> borderYList = MakeBorderYList(width, height);

        List<BlockBundle> blockBundleLower = MakeBlockBundle(width, height, borderYList, groundList, true);
        List<BlockBundle> blockBundleUpper = MakeBlockBundle(width, height, borderYList, groundList, false);

        separatedBlockList = ShuffleBlockBundle(blockBundleLower, blockBundleUpper);

        return separatedBlockList;
    }

    // 위 아래 경계선 반환하는 메소드
    private List<int> MakeBorderYList(int width, int height)
    {
        List<int> borderYList = new List<int>();
        int currentBorderY = height / 2 - 1;
        for (int i = 0; i < width; i++)
        {
            currentBorderY += Random.Range(-1, 2);
            // 예외 처리
            if (currentBorderY < 0 || currentBorderY >= height)
                currentBorderY = height / 2 - 1;
            borderYList.Add(currentBorderY);
        }

        return borderYList;
    }

    // 랜덤한 블럭 번들을 구해서 List로 반환하는 메소드
    private List<BlockBundle> MakeBlockBundle(int width, int height, List<int> borderYList, List<List<BlockMK2>> groundList, bool isLower)
    {
        List<BlockBundle> result = new List<BlockBundle>();

        int currentX = 0;
        BlockBundle blockBundle = null;
        while (true)
        {
            blockBundle = new BlockBundle(_moveSpeed);
            int randomDistance = Random.Range(5, 8);

            if (currentX + randomDistance >= width)
                break;

            for (int x = currentX; x < currentX + randomDistance; x++)
            {
                if(isLower)
                    for (int y = 0; y < borderYList[x]; y++)
                        blockBundle.AddBlock(groundList[y][x]);
                else
                    for (int y = borderYList[x]; y < height; y++)
                        blockBundle.AddBlock(groundList[y][x]);
            }
            result.Add(blockBundle);
            currentX += randomDistance;
        }

        blockBundle = new BlockBundle(_moveSpeed);
        for (int x = currentX; x < width; x++)
        {
            if (isLower)
                for (int y = 0; y < borderYList[x]; y++)
                    blockBundle.AddBlock(groundList[y][x]);
            else
                for (int y = borderYList[x]; y < height; y++)
                    blockBundle.AddBlock(groundList[y][x]);
        }
        result.Add(blockBundle);

        return result;
    }

    // 두 블럭번들을 적절히 섞는 메소드
    private List<BlockBundle> ShuffleBlockBundle(List<BlockBundle> blockBundleLower, List<BlockBundle> blockBundleUpper)
    {
        List<BlockBundle> separatedBlockList = new List<BlockBundle>();

        int maxSize = Mathf.Max(blockBundleLower.Count, blockBundleUpper.Count);
        for (int i = 0; i < maxSize; i++)
        {
            if (i < blockBundleLower.Count)
            {
                separatedBlockList.Add(blockBundleLower[i]);
                blockBundleLower[i].SetPositionBundle(-_initPos);
            }
            if (i < blockBundleUpper.Count)
            {
                separatedBlockList.Add(blockBundleUpper[i]);
                blockBundleUpper[i].SetPositionBundle(_initPos);
            }
        }

        return separatedBlockList;
    }
}
