using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditEraseState : BaseMapEditState
{
    public MapEditEraseState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        // 선택된 블럭 지움
        _content.DestroyBlock();
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        // 블럭 지우기 로직
        // _content.EraseBlock();

        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            // 만약 이미 블럭이 있다면 지워줌
            if (_content.mapData[y, x] != null)
            {
                _content.prevBlock = _content.selectedBlock;
                _content.selectedBlock = _content.mapData[y, x];

                if(_content.selectedBlock != _content.prevBlock)
                {
                    if (_content.prevBlock != null)
                        _content.prevBlock.NonSelectBlock();
                    _content.selectedBlock.SelectBlock();
                }

                // 블럭 지우기
                if (Input.GetMouseButtonDown(0))
                {
                    _content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] = null;
                    _content.DestroyBlock();
                }
            }
            else
            {
                if(_content.selectedBlock != null)
                    _content.selectedBlock.NonSelectBlock();
            }
        }
    }
}
