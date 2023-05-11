using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawState : BaseMapEditState
{
    public MapEditDrawState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        Debug.Log("들어왔습니다.");

        // 들어올 때 이전에 사용했던 블럭을 선택함 (없으면 0)
        _content.SelectBlock(_content.CurrentBlockIndex);
    }

    public override void Exit()
    {
        Debug.Log("나갔습니다.");
    }

    public override void Update()
    {
        Debug.Log("진행중입니다.");

        _content.InputBlock();

        // 시작점 끝점 찍고 그 사이 모든 그리드에 선택된 block들 다 생성한다.
        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            // 만약 이미 블럭이 있다면 불가
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            if (_content.mapData[y, x] != 0)
                return;

            _content.selectedBlock.transform.position = hitAroundPoint;

            // 블럭 놓기
            if (Input.GetMouseButtonDown(0))
            {
                _content.mapData[y, x] = _content.CurrentBlockIndex + 1;
                _content.selectedBlock.SetPos(x, y);
                _content.selectedBlock.transform.position = hitAroundPoint;
                _content.selectedBlock = null;
                _content.SelectBlock(_content.CurrentBlockIndex);
            }
        }
    }
}
