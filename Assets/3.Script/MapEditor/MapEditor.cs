using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    [Header("Blocks")]
    public GameObject[] blockPrefabs;
    [SerializeField] private GameObject _baseBlockPrefab;
    [SerializeField] private GameObject _testBlockPrefab;

    [Header("Level")]
    [SerializeField] private Transform _level;

    private int _currentBlockIndex = 0;
    private int _minX, _minY;
    private GameObject _selectedBlock;
    private Camera _mainCam;

    private int[,] _mapData;

    private void Awake()
    {
        SelectBlock(_currentBlockIndex);
        _mainCam = Camera.main;
    }

    // 맵 편집 시작 버튼에 넣어줌
    public void StartMapEdit()
    {
        InitLevel();
    }

    private void InitLevel()
    {
        _level.DestroyAllChild();
    }

    public void MakeBaseMap(int x, int z)
    {
        GameObject baseBlock = Instantiate(_baseBlockPrefab, _level);
        baseBlock.transform.localScale = new Vector3(x, 1, z);

        _mapData = new int[z, x];

        _minX = (x / 2) - 1;
        _minY = (z / 2) - 1;
    }

    private void SelectBlock(int index)
    {
        if (_selectedBlock != null)
            Destroy(_selectedBlock.gameObject);
        _selectedBlock = Instantiate(blockPrefabs[index], _level);
    }

    private void Update()
    {
        // 블럭 선택
        InputBlock();

        // 블럭 움직이기
        RaycastHit hit;
        if (Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out hit, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            // 만약 이미 블럭이 있다면 불가
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            if (_mapData[(int)hitAroundPoint.z + _minY, (int)hitAroundPoint.x + _minX] != 0)
                return;

            _selectedBlock.transform.position = hitAroundPoint;

            // 블럭 놓기
            if (Input.GetMouseButtonDown(0))
            {
                _mapData[(int)hitAroundPoint.z + _minY, (int)hitAroundPoint.x + _minX] = _currentBlockIndex + 1;

                _selectedBlock.transform.position = hitAroundPoint;
                _selectedBlock = null;
                SelectBlock(_currentBlockIndex);
            }
        }
    }

    private void InputBlock()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            _currentBlockIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Keypad1))
            _currentBlockIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            _currentBlockIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            _currentBlockIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Keypad4))
            _currentBlockIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Keypad5))
            _currentBlockIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Keypad6))
            _currentBlockIndex = 6;

        SelectBlock(_currentBlockIndex);
    }
}
