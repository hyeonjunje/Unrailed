using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawState : BaseMapEditState
{
    private Block prevBlock;
    private Block currentBlock;

    public MapEditDrawState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        // 들어올 때 이전에 사용했던 블럭을 선택함 (없으면 0)
        _content.SelectBlock(_content.CurrentBlockIndex);
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        _content.InputBlock();

        // 시작점 끝점 찍고 그 사이 모든 그리드에 선택된 block들 다 생성한다.
        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            // 만약 이미 블럭이 있다면 불가
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            // 만약 이미 있는 블럭
            if (_content.mapData[y, x] != null)
            {
                prevBlock = currentBlock;
                currentBlock = _content.mapData[y, x];

                if(prevBlock != null && prevBlock != currentBlock)
                {
                    prevBlock.gameObject.SetActive(true);
                    currentBlock.gameObject.SetActive(false);
                }
            }
            else
            {
                if(currentBlock != null)
                    currentBlock.gameObject.SetActive(true);
            }

            _content.selectedBlock.transform.position = hitAroundPoint;

            // 블럭 놓기
            if (Input.GetMouseButtonDown(0))
            {
                // 만약 이미 있는 블럭이면 블럭 없애고 새로 그림
                if (_content.mapData[y, x] != null)
                {
                    GameObject.Destroy(_content.mapData[y, x]);
                    _content.mapData[y, x] = null;
                }

                _content.mapData[y, x] = _content.selectedBlock;
                _content.selectedBlock.SetPos(x, y);
                _content.selectedBlock.transform.position = hitAroundPoint;
                _content.selectedBlock = null;
                _content.SelectBlock(_content.CurrentBlockIndex);
            }
        }
    }
}
