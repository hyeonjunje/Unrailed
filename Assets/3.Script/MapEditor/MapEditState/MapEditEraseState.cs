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
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Block")))
        {
            // 만약 이미 블럭이 있다면 실행
            _content.prevBlock = _content.selectedBlock;
            _content.selectedBlock = hit.transform.GetComponent<Block>();

            // 블럭에 Block 컴포넌트가 없다면 그냥 return
            if (_content.selectedBlock == null)
            {
                Debug.Log("Block 컴포넌트가 없습니다.");
                return;
            }


            if (_content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] != 0)
            {
                if (_content.selectedBlock != _content.prevBlock)
                {
                    if (_content.prevBlock != null)
                        _content.prevBlock.NonSelectBlock();
                    _content.selectedBlock.SelectBlock();
                }

                // 블럭 지우기
                if (Input.GetMouseButtonDown(0))
                {
                    _content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] = 0;
                    _content.DestroyBlock();
                }
            }
        }
    }
}
