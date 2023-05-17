using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMapEditState
{
    Draw,
    Erase,
    DrawInLine,
    Size
}

public class MapEditStateFactory
{
    private MapEditor _mapEditor;
    private Dictionary<EMapEditState, BaseMapEditState> _dic = new Dictionary<EMapEditState, BaseMapEditState>();

    private BaseMapEditState _currentState;
    public BaseMapEditState CurrentState => _currentState;

    public MapEditStateFactory(MapEditor mapEditor)
    {
        _mapEditor = mapEditor;

        _dic[EMapEditState.Draw] = new MapEditDrawState(_mapEditor, this);
        _dic[EMapEditState.Erase] = new MapEditEraseState(_mapEditor, this);
        _dic[EMapEditState.DrawInLine] = new MapEditDrawInLineState(_mapEditor, this);
    }

    public void ChangeState(EMapEditState editState)
    {
        if(!_dic.ContainsKey(editState))
        {
            Debug.Log("없는 상태입니다.");
            return;
        }

        if(_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = _dic[editState];
        _currentState.Enter();
    }
}
