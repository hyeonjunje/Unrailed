using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class PathFindingField : MonoBehaviour
{
    #region �̱���
    /// public static PathFindingField Instance { get; private set; } = null;

    private static PathFindingField _instance = null;
    public static PathFindingField Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PathFindingField>();
            }
            return _instance;
        }
    }
/*
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }*/
    #endregion
    private bool[,] _mapData;

    public int MinX { get; private set; }
    public int MinY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }


    // �� ������ �ʱ�ȭ
    public void InitData(Transform map)
    {
        MinX = Mathf.RoundToInt(map.GetChild(0).localPosition.x);
        MinY = Mathf.RoundToInt(map.GetChild(0).localPosition.z);

        Width = (MinX * -1 + 1) * 2;
        Height = (MinY * -1 + 1) * 2;

        _mapData = new bool[Height, Width * 2];
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                _mapData[i,j] = true;
    }

    public void UpdateMapData(float x, float y, bool flag)
    {
        int xPos = Mathf.RoundToInt(x) - MinX;
        int yPos = Mathf.RoundToInt(y) - MinY;

        _mapData[yPos, xPos] = flag;
    }

    public bool GetMapData(int x, int y)
    {
        return _mapData[y, x];
    }
}
