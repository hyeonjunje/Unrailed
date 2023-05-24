using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMK2 : MonoBehaviour
{
    [SerializeField] private LayerMask _blockLayer;

    private int _index;
    private Renderer _renderer;
    private GameObject _itemPrefab;
    private BlockTransformerData _blockTransformerData;

    private Vector3[] dir = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

    public int Index => _index;
    public bool isTransformed => Index == (int)EBlock.iron || Index == (int)EBlock.blackRock;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Init(int index, Material material, GameObject itemPrefab, BlockTransformerData blockTransformerData = null)
    {
        _index = index;
        _renderer.material = material;
        _itemPrefab = itemPrefab;
        _blockTransformerData = blockTransformerData;

        InitData();
        InstantiateItem(index);
    }

    private void InitData()
    {
        transform.DestroyAllChild();

        _renderer.enabled = true;
        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.tag = "Untagged";
    }

    private void InstantiateItem(int index)
    {
        GameObject go = null;

        if (_itemPrefab != null)
        {
            go = Instantiate(_itemPrefab, transform);
            go.transform.localPosition = Vector3.up * 0.5f;
            go.transform.localRotation = Quaternion.identity;
        }

        if(index == (int)EBlock.empty)
        {
            _renderer.enabled = false;
            transform.tag = "Empty";
        }
        else if(index == (int)EBlock.water)
        {
            transform.localScale -= Vector3.up * 0.2f;
            transform.position -= Vector3.up * 0.1f;
            transform.tag = "Water";
        }
        else if(index == (int)EBlock.tree1 || index == (int)EBlock.tree2)
        {
            go.transform.GetChild(0).localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        else if(isTransformed)
        {
            if(CheckAroundBlock())
            {
                InstantiateTransformedOrigin();
            }
            else
            {
                InstantiateTransformedCarve();
            }
        }

        CheckAroundTransformedBlock();
    }

    private void CheckAroundTransformedBlock()
    {
        for(int i = 0; i < dir.Length; i++)
        {
            if(Physics.Raycast(transform.position, dir[i], out RaycastHit hit, 1f, _blockLayer))
            {
                BlockMK2 block = hit.transform.GetComponent<BlockMK2>();
                if(block != null)
                {
                    if(block.isTransformed)
                    {
                        if(block.CheckAroundBlock())
                        {
                            block.InstantiateTransformedOrigin();
                        }
                        else
                        {
                            block.InstantiateTransformedCarve();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>앞뒤좌우가 다 돌이나 철로 둘러싸여있으면 true
    /// 아니면 false</returns>
    public bool CheckAroundBlock()
    {
        for(int i = 0; i < dir.Length; i++)
        {
            if(Physics.Raycast(transform.position, dir[i], out RaycastHit hit, 1f, _blockLayer))
            {
                BlockMK2 block = hit.transform.GetComponent<BlockMK2>();
                if(block != null)
                    if (!block.isTransformed)
                        return false;
            }
        }
        return true;
    }

    public void InstantiateTransformedOrigin()
    {
        transform.DestroyAllChild();

        GameObject go = Instantiate(_blockTransformerData.originBlock, transform);
        go.transform.localPosition = Vector3.up * 0.5f;
        go.transform.localRotation = Quaternion.identity;

        float randomHeight = 0;
        if (Random.Range(1, 4) == 1)
            randomHeight = 0.1f;

        go.transform.localScale = Vector3.one + Vector3.up * randomHeight;
    }

    public void InstantiateTransformedCarve()
    {
        transform.DestroyAllChild();

        float[] angles = new float[4] { 0f, 90f, 180f, 270f };

        GameObject prefab = _blockTransformerData.transformedBlock[Random.Range(0, _blockTransformerData.transformedBlock.Length)];
        GameObject go = Instantiate(prefab, transform);
        go.transform.localPosition = Vector3.up * 0.5f;
        go.transform.localRotation = Quaternion.Euler(Vector3.up * angles[Random.Range(0, angles.Length)]);
    }
}
