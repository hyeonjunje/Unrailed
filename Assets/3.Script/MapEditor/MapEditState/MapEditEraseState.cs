using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditEraseState : BaseMapEditState
{
    private Block _prevBlock;
    private Block _currentBlock;

    private int _prevIndex;
    private int _currentIndex;

    public MapEditEraseState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {

    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        if (_currentBlock != null)
            _currentBlock.SetBlockInfo(_content.GetMaterial(_currentIndex), _currentIndex);
    }

    public override void Update()
    {
        // �� ����� ����
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            int x = Mathf.RoundToInt(hit.point.x) + _content.MinX;
            int y = Mathf.RoundToInt(hit.point.z) + _content.MinY;

            if (_currentBlock != null)
            {
                _prevBlock = _currentBlock;
                _prevIndex = _currentIndex;
                _prevBlock.SetBlockInfo(_content.GetMaterial(_prevIndex), _prevIndex);
            }
            _currentBlock = _content.mapData[y, x];
            _currentIndex = _currentBlock.Index;
            _currentBlock.SetBlockInfo(_content.GetMaterial(0), 0);

            // ��� ������ ��������
            if(Input.GetMouseButton(0))
            {
                int size = _content.EraseSize / 2;
                for(int i = -size; i <= size; i++)
                {
                    for(int j = -size; j <= size; j++)
                    {
                        if(isRange(x + j, y + i))
                            _content.mapData[y + i, x + j].SetBlockInfo(_content.GetMaterial(0), 0);

                        // _content.mapData[y, x].SetBlockInfo(_content.GetMaterial(0), 0);
                    }
                }

                _currentBlock = null;
            }
        }
    }

    private bool isRange(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;

        if (x >= _content.mapData.GetLength(1) || y >= _content.mapData.GetLength(0))
            return false;

        return true;
    }
}
