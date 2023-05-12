using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawInLineState : BaseMapEditState
{
    private bool isStart = false;
    private bool isEnd = false;

    private Vector2Int startPos;
    private Vector2Int endPos;


    private Block _prevBlock;
    private Block _currentBlock;

    private int _prevIndex;
    private int _currentIndex;


    public MapEditDrawInLineState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Enter()
    {
        isStart = false;
        isEnd = false;
    }

    public override void Exit()
    {
        if(_currentBlock != null)
            _currentBlock.SetBlockInfo(_content.GetMaterial(0), 0);
    }

    public override void Update()
    {
        if(!isStart && !isEnd)
        {
            _content.InputBlock();
        }

        // 시작점 끝점 찍고 그 사이 모든 그리드에 선택된 block들 다 생성한다.
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
            _currentBlock.SetBlockInfo(_content.GetMaterial(_content.CurrentBlockIndex), _content.CurrentBlockIndex);


            if (Input.GetMouseButtonDown(0))
            {
                // 한 번 누를 때
                if(!isStart && !isEnd)
                {
                    startPos = new Vector2Int(x, y);
                    isStart = true;

                    _currentBlock.SetBlockInfo(_content.GetMaterial(_content.CurrentBlockIndex), _content.CurrentBlockIndex);
                    _currentBlock = null;
                }
                // 두 번 누를 때
                else if(isStart && !isEnd)
                {
                    endPos = new Vector2Int(x, y);

                    isStart = false;
                    isEnd = false;

                    // 좌표 가득 채우기
                    int minX = startPos.x <= endPos.x ? startPos.x : endPos.x;
                    int maxX = startPos.x > endPos.x ? startPos.x : endPos.x;

                    int minY = startPos.y <= endPos.y ? startPos.y : endPos.y;
                    int maxY = startPos.y > endPos.y ? startPos.y : endPos.y;

                    for(int i = minX; i <= maxX; i++)
                    {
                        for(int j = minY; j <= maxY; j++)
                        {
                            _content.mapData[j, i].SetBlockInfo(_content.GetMaterial(_content.CurrentBlockIndex), _content.CurrentBlockIndex);
                        }
                    }
                    _currentBlock = null;
                }
            }
        }
    }
}
