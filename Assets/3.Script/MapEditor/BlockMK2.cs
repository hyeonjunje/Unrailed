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

        if (index == (int)EBlock.tree1)
        {
            Instantiate(_itemPrefabData.tree1, transform).transform.localPosition = Vector3.up * 0.5f;
        }
        else if(index == (int)EBlock.tree2)
        {
            Instantiate(_itemPrefabData.tree2, transform).transform.localPosition = Vector3.up * 0.5f;
        }
        else if(index == (int)EBlock.iron)
        {
            Instantiate(_itemPrefabData.iron, transform).transform.localPosition = Vector3.up;
        }
        else if(index == (int)EBlock.blackRock)
        {
            Instantiate(_itemPrefabData.blackStone, transform).transform.localPosition = Vector3.up;
        }
        else if(index == (int)EBlock.water)
        {
            transform.localScale -= Vector3.up * 0.2f;
            transform.position -= Vector3.up * 0.1f;
            transform.tag = "Water";
        }
    }

    private void InitData()
    {
        transform.DestroyAllChild();

        transform.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.tag = "Untagged";
    }
}
