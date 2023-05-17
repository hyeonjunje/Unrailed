using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorMK2 : MonoBehaviour
{
    private enum EBlock { empty, grass, water, tree, iron, blackRock, size}


    [Header("Blocks")]
    [SerializeField] private BlockMK2 _blockPrefab;
    [SerializeField] private Material[] _blocksMaterial; 

    [Header("Transform")]
    [SerializeField] private Transform _blockParent;
    [SerializeField] private Transform _environmentParent;

    [Header("Info")]
    [SerializeField] private int _defaultX = 40;
    [SerializeField] private int _defaultY = 20;

    [Header("ETC")]
    [SerializeField] private Transform _target;

    public int materialIndex;

    // 한번에 합쳐도 될듯
    private int[,] groundArr;
    private int[,] itemArr;

    // 계속 증가하니까 리스트로?
    private List<List<BlockMK2>> groundList = new List<List<BlockMK2>>();

    private int _minX;
    private int _minY;
    private Camera _mainCam;

    public void Awake()
    {
        _mainCam = Camera.main;
        _target.gameObject.SetActive(true);

        // 리스트 초기화
        groundList = new List<List<BlockMK2>>();

        _minX = (_defaultX / 2) - 1;
        _minY = (_defaultY / 2) - 1;

        for (int i = 0; i < _defaultY; i++)
        {
            groundList.Add(new List<BlockMK2>());
            for (int j = 0; j < _defaultX; j++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.Init((int)EBlock.empty, _blocksMaterial[(int)EBlock.empty]);
                go.transform.localPosition = new Vector3(j - _minX, 0, i - _minY);
                groundList[i].Add(go);
            }
        }
    }

    // x축 변화
    public void ResizeXMap(int value)
    {
        if(value > 0)
        {
            int x = groundList[0].Count;
            for(int i = 0; i < groundList.Count; i++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.transform.localPosition = new Vector3(x - _minX, 0, i - _minY);
                groundList[i].Add(go);
            }
        }
        else if(value < 0)
        {
            for (int i = 0; i < groundList.Count; i++)
            {
                BlockMK2 go = groundList[i][groundList[i].Count - 1];
                groundList[i].RemoveAt(groundList[i].Count - 1);
                Destroy(go);
            }
        }
        _mainCam.transform.position = new Vector3((groundList[0].Count - _defaultX) / 2, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    // y축 변화
    public void ResizeYMap(int value)
    {
        if(value > 0)
        {
            int y = groundList.Count;
            groundList.Add(new List<BlockMK2>());
            for(int i = 0; i < groundList[0].Count; i++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);

                go.transform.localPosition = new Vector3(i - _minX, 0, y - _minY);
                groundList[groundList.Count - 1].Add(go);
            }
        }
        else if(value < 0)
        {
            for (int i = 0; i < groundList[0].Count; i++)
            {
                BlockMK2 go = groundList[groundList.Count - 1][i];
                Destroy(go);
            }
            groundList.RemoveAt(groundList.Count - 1);
        }
        _mainCam.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, (groundList.Count - _defaultY) / 2 + 2);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Block")))
        {
            int x = Mathf.RoundToInt(hit.point.x);
            int z = Mathf.RoundToInt(hit.point.z);

            _target.position = new Vector3(x, 0.51f, z);


            if (Input.GetMouseButton(0))
            {
                // 다를 때만 다른 색으로 칠해준다.
                if(groundList[z + _minY][x + _minX].Index != materialIndex)
                {
                    groundList[z + _minY][x + _minX].Init(materialIndex, _blocksMaterial[materialIndex]);
                }
            }
        }
    }
}
