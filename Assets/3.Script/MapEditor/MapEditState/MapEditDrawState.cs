using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawState : BaseMapEditState
{
    private Block _prevBlock;
    private Block _currentBlock;

    private int _prevIndex;
    private int _currentIndex;

    public MapEditDrawState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
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
        _content.InputBlock();

        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            int x = Mathf.RoundToInt(hit.point.x) + _content.MinX;
            int y = Mathf.RoundToInt(hit.point.z) + _content.MinY;

            if (_currentBlock != null)
            {
                _prevBlock = _currentBlock;
                _prevIndex = _currentIndex;
                _prevBlock.SetBlockInfo(_content.GetMaterial(_prevIndex), _prevIndex);
            }
            _currentBlock = _content.mapArr[y, x];
            _currentIndex = _currentBlock.Index;
            _currentBlock.SetBlockInfo(_content.GetMaterial(_content.CurrentBlockIndex), _content.CurrentBlockIndex);

            if (Input.GetMouseButton(0))
            {
                _content.mapArr[y, x].SetBlockInfo(_content.GetMaterial(_content.CurrentBlockIndex), _content.CurrentBlockIndex);
                _currentBlock = null;
            }
        }
    }
}
