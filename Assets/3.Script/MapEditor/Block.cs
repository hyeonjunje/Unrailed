using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    private int _index;

    private int _x;
    private int _y;

    public int Index => _index;
    public int X => _x;
    public int Y => _y;
    
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void SetPos(int x, int y)
    {
        _x = x;
        _y = y;
    }

    // 블럭의 정보 설정
    public void SetBlockInfo(Material material, int index)
    {
        if (_index == index)
            return;

        _index = index;
        _renderer.material = material;
    }
}
