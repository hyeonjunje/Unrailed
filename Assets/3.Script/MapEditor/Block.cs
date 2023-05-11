using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    private int _x;
    private int _y;
    public int X => _x;
    public int Y => _y;
    
    private Renderer _renderer;

    [SerializeField] private Material _selectedMaterial;
    private Material _originMaterial;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void InitBlock(Material material)
    {
        // 머테리얼 설정
        _renderer.material = material;
        _originMaterial = material;
    }

    public void SetPos(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SelectBlock()
    {
        _renderer.material = _selectedMaterial;
    }

    public void NonSelectBlock()
    {
        _renderer.material = _originMaterial;
    }
}
