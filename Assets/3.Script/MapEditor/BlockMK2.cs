using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMK2 : MonoBehaviour
{
    private int _index;
    private Renderer _renderer;
    private Material _originMaterial;

    [SerializeField] private ItemPrefabData _itemPrefabData;

    public int Index => _index;


    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _originMaterial = _renderer.material;
    }

    public void Init(int index, Material material)
    {
        _index = index;
        if (index > (int)EBlock.blackRock)
            _renderer.material = _originMaterial;
        else
            _renderer.material = material;

        InitData();

        InstantiateItem(index);
    }

    private void InitData()
    {
        transform.DestroyAllChild();

        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.tag = "Untagged";
    }

    private void InstantiateItem(int index)
    {
        GameObject prefab = _itemPrefabData.itemPrefabs[index];
        switch(index)
        {
            case (int)EBlock.water:
                transform.localScale -= Vector3.up * 0.2f;
                transform.position -= Vector3.up * 0.1f;
                transform.tag = "Water";
                break;
            case (int)EBlock.tree1:
            case (int)EBlock.tree2:
                GameObject go = Instantiate(prefab, transform);
                go.transform.localPosition = Vector3.up * 0.5f;
                go.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                break;
            case (int)EBlock.iron:
            case (int)EBlock.blackRock:
                Instantiate(prefab, transform).transform.localPosition = Vector3.up;
                break;
            case (int)EBlock.station:
            case (int)EBlock.fence:
            case (int)EBlock.rail:
            case (int)EBlock.treeItem:
            case (int)EBlock.ironItem:
            case (int)EBlock.axe:
            case (int)EBlock.pick:
            case (int)EBlock.bucket:
            case (int)EBlock.bolt:
                Instantiate(prefab, transform).transform.localPosition = Vector3.up * 0.5f;
                break;
        }
    }
}
