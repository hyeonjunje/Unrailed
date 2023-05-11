using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMapEditState
{
    protected MapEditor _content;
    protected MapEditStateFactory _factory;

    public BaseMapEditState(MapEditor mapEditor, MapEditStateFactory stateFactory)
    {
        _content = mapEditor;
        _factory = stateFactory;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Click();
    public abstract void Exit();
}
