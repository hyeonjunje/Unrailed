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
    public Block[,] mapData;

    private int _currentBlockIndex = 0;
    private int _minX, _minY;
    private Camera _mainCam;
    private MapEditStateFactory _factory;


    public int eraseSize => _mapEditorUI.EraseSize;
    public int MinX => _minX;
    public int MinY => _minY;
    public int CurrentBlockIndex => _currentBlockIndex;
    public Camera MainCam => _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
        _factory = new MapEditStateFactory(this);
    }

    private void Update()
    {
        if(_factory.CurrentState != null)
            _factory.CurrentState.Update();
    }

    // 편집 모드 전환
    public void ChangeEditMode(EMapEditState editState)
    {
        _factory.ChangeState(editState);
    }

    // 맵 편집 시작 버튼에 넣어줌
    public void StartMapEdit()
    {
        _mapEditorUI.gameObject.SetActive(false);
        InitLevel();
    }

    // 레벨 초기화 (안에 있는 오브젝트 다 삭제)
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
        // UI 생성
        _mapEditorUI.gameObject.SetActive(true);

        // 좌표, 배열 초기화
        mapData = new Block[z, x];
        _minX = (x / 2) - 1;
        _minY = (z / 2) - 1;

        // 변수값 초기화
        _currentBlockIndex = 0;

        // 사이즈만큼의 큰 판때기 생성
        GameObject baseBlock = Instantiate(_baseBlockPrefab, _level);
        baseBlock.transform.localScale = new Vector3(x, 1, z);

        // 사이즈 수만큼 블럭 생성, MapData 블럭들 넣어주고 값들 초기화
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Block block = Instantiate(_blockPrefab, _level);
                block.transform.localPosition = new Vector3(i - _minX, 1, j - _minY);
                block.SetPos(i, j);
                mapData[j, i] = block;
                SetMapData(i, j, _currentBlockIndex);
            }
        }

        // 그 위에 그리드 그리기
        LineGrid lineGrid = Instantiate(_lineGrid, baseBlock.transform);
        lineGrid.transform.localPosition = new Vector3(0, 1.51f, 0);
        lineGrid.MakeGrid(-(x / 2 - 0.5f), -(z / 2 - 0.5f), x, z);

        // 드로우 모드로 변경
        ChangeEditMode(EMapEditState.Draw);

        _currentBlockIndex = 1;
    }

    public void SetMapData(int x, int y, int materialIndex)
    {
        mapData[y, x].SetBlockInfo(blockMaterials[materialIndex], materialIndex);
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
}
