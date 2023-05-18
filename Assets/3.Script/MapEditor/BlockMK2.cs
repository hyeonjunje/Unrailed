using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMK2 : MonoBehaviour
{
    private int _index;
    private Renderer _renderer;
    public int Index => _index;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void Init(int index, Material material)
    {
        _index = index;
        _renderer.material = material;
    }
}
