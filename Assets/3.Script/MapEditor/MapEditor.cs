using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapEditor : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private Material[] blockMaterials;
    [SerializeField] private GameObject _baseBlockPrefab;
    [SerializeField] private LineGrid _lineGrid;
    [SerializeField] private Block _blockPrefab;

    [Header("Level")]
    [SerializeField] private Transform _level;

    [Header("UI")]
    [SerializeField] private MapEditorUI _mapEditorUI;

    

    [HideInInspector] public Block selectedBlock = null;
    [HideInInspector] public Block prevBlock = null;
    public Block[,] mapArr;

    private int _currentBlockIndex = 0;
    private int _minX, _minY;
    private Camera _mainCam;
    private MapEditStateFactory _factory;
    private MapCreator _mapCreator;

    public int EraseSize => _mapEditorUI.EraseSize;
    public int MinX => _minX;
    public int MinY => _minY;
    public int CurrentBlockIndex => _currentBlockIndex; 
    public Camera MainCam => _mainCam;

    private void Awake()
    {
        FileManager.LoadGame();

        _mainCam = Camera.main;
        _factory = new MapEditStateFactory(this);
        _mapCreator = GetComponent<MapCreator>();
    }

    private void Update()
    {
        if(_factory.CurrentState != null)
            _factory.CurrentState.Update();
    }

    // ���� ��� ��ȯ
    public void ChangeEditMode(EMapEditState editState)
    {
        _factory.ChangeState(editState);
    }

    // �� ���� ���� ��ư�� �־���
    public void StartMapEdit()
    {
        _mapEditorUI.gameObject.SetActive(false);
        InitLevel();
    }

    // ���� �ʱ�ȭ (�ȿ� �ִ� ������Ʈ �� ����)
    private void InitLevel()
    {
        _level.DestroyAllChild();
    }

    public Material GetMaterial(int index)
    {
        return blockMaterials[index];
    }

    public void MakeBaseMap(int x, int z)
    {
        // UI ����
        _mapEditorUI.gameObject.SetActive(true);

        // ��ǥ, �迭 �ʱ�ȭ
        mapArr = new Block[z, x];
        _minX = (x / 2) - 1;
        _minY = (z / 2) - 1;

        // ������ �ʱ�ȭ
        _currentBlockIndex = 0;

        // �����ŭ�� ū �Ƕ��� ����
        GameObject baseBlock = Instantiate(_baseBlockPrefab, _level);
        baseBlock.transform.localScale = new Vector3(x, 1, z);

        // ������ ����ŭ �� ����, MapData ���� �־��ְ� ���� �ʱ�ȭ
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Block block = Instantiate(_blockPrefab, _level);
                block.transform.localPosition = new Vector3(i - _minX, 1, j - _minY);
                block.SetPos(i, j);
                mapArr[j, i] = block;
                SetMapData(i, j, _currentBlockIndex);
            }
        }

        // �� ���� �׸��� �׸���
        LineGrid lineGrid = Instantiate(_lineGrid, baseBlock.transform);
        lineGrid.transform.localPosition = new Vector3(0, 1.51f, 0);
        lineGrid.MakeGrid(-(x / 2 - 0.5f), -(z / 2 - 0.5f), x, z);

        // ��ο� ���� ����
        ChangeEditMode(EMapEditState.Draw);

        _currentBlockIndex = 1;
    }

    public void SetMapData(int x, int y, int materialIndex)
    {
        mapArr[y, x].SetBlockInfo(blockMaterials[materialIndex], materialIndex);
    }

    public void InputBlock()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            _currentBlockIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _currentBlockIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            _currentBlockIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _currentBlockIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _currentBlockIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _currentBlockIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            _currentBlockIndex = 7;
        }
    }

    // �� ���� ��ư
    public void CreateMap()
    {
        _mapCreator.CreateMap(mapArr);
    }

    // ���� ��ư
    public void SaveMap()
    {
        MyArr<int>[] mapIndexData = new MyArr<int>[mapArr.GetLength(0)];

        for(int i = 0; i < mapArr.GetLength(0); i++)
        {
            mapIndexData[i] = new MyArr<int>(mapArr.GetLength(1));
            for (int j = 0; j < mapArr.GetLength(1); j++)
            {
                mapIndexData[i].arr[j] = mapArr[i, j].Index;
            }
        }

        MapData mapData = new MapData(0, mapIndexData);
        FileManager.MapsData.Add(mapData);
        FileManager.SaveGame();
    }


    // �ҷ����� ��ư
    public void LoadMap(int index)
    {
        MapData mapData = FileManager.MapsData.mapsData[index];

        // �� �ʱ�ȭ
        _level.DestroyAllChild();
        int y = mapData.mapData.Length;
        int x = mapData.mapData[0].arr.Length;
        MakeBaseMap(x, y);

        // �� ������
        for(int i = 0; i < y; i++)
        {
            for(int j = 0; j < x; j++)
            {
                int blockIndex = mapData.mapData[i].arr[j];
                mapArr[i, j].SetBlockInfo(blockMaterials[blockIndex], blockIndex);
            }
        }
    }
}
