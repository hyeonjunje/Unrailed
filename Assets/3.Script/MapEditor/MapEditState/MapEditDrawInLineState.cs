using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawInLineState : BaseMapEditState
{
    private bool isStart = false;
    private bool isEnd = false;

    private Vector2Int startPos;
    private Vector2Int endPos;

    private Block startBlock;
    private Block endBlock;

    public MapEditDrawInLineState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
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
        if(startBlock != null)
            GameObject.Destroy(startBlock.gameObject);
        if(endBlock != null)
            GameObject.Destroy(endBlock.gameObject);
        startBlock = null;
        endBlock = null;
        isStart = false;
        isEnd = false;
    }

    public override void Update()
    {
        if(!isStart && !isEnd)
        {
            _content.InputBlock();
        }

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
                if(!isStart && !isEnd)
                {
                    Debug.Log("시작점!!");

                    isStart = true;

                    // 시작점 찍어주기
                    startBlock = _content.selectedBlock;
                    startBlock.transform.position = hitAroundPoint;
                    startBlock.SelectBlock();

                    _content.selectedBlock = null;
                    _content.SelectBlock(_content.CurrentBlockIndex);
                    startPos = new Vector2Int(x, y);
                }
                else if(isStart && !isEnd)
                {
                    Debug.Log("끝점!!");

                    isEnd = true;

                    // 끝점 찍어주기
                    endBlock = _content.selectedBlock;
                    endBlock.transform.position = hitAroundPoint;
                    endBlock.SelectBlock();

                    _content.selectedBlock = null;
                    _content.SelectBlock(_content.CurrentBlockIndex);
                    endPos = new Vector2Int(x, y);
                }

                if(isStart && isEnd)
                {
                    Debug.Log("그린다!!");

                    isStart = false;
                    isEnd = false;

                    // 좌표 가득 채워주기
                    int minX = startPos.x <= endPos.x ? startPos.x : endPos.x;
                    int maxX = startPos.x > endPos.x ? startPos.x : endPos.x;

                    int minY = startPos.y <= endPos.y ? startPos.y : endPos.y;
                    int maxY = startPos.y > endPos.y ? startPos.y : endPos.y;

                    Debug.Log(minX + " " + maxX + " " + minY + " " + maxY);

                    GameObject.Destroy(startBlock.gameObject);
                    GameObject.Destroy(endBlock.gameObject);

                    for(int i = minX; i <= maxX; i++)
                    {
                        for(int j = minY; j <= maxY; j++)
                        {
                            if(_content.mapData[j, i] == 0)
                            {
                                _content.SelectBlock(_content.CurrentBlockIndex);
                                int currentX = i - _content.MinX;
                                int currentY = j - _content.MinY;
                                _content.selectedBlock.transform.position = new Vector3(currentX, 1, currentY);
                                _content.selectedBlock.SetPos(i, j);
                                _content.mapData[j, i] = _content.CurrentBlockIndex + 1;
                                _content.selectedBlock = null;
                            }
                        }
                    }
                    _content.SelectBlock(_content.CurrentBlockIndex);
                }
            }
        }
    }
}
