using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode_Selector : BTNodeBase
{
    //실패 시 다음 노드 진행
    protected override bool ContinueEvaluatingIfChildFailed()
    {
        return true;
    }
    //성공 시 리턴
    protected override bool ContinueEvaluatingIfChildSucceeded()
    {
        return false;
    }

    //상태 업데이트
    protected override void OnTickedAllChildren()
    {
        LastStatus = _children[_children.Count - 1].LastStatus;
    }
}
