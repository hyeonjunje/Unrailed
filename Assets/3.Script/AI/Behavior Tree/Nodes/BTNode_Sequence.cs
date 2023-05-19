using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode_Sequence : BTNodeBase
{
    //실패 시 리턴
    protected override bool ContinueEvaluatingIfChildFailed()
    {
        return false;
    }

    //성공 시 계속 진행
    protected override bool ContinueEvaluatingIfChildSucceeded()
    {
        return true;
    }

    //상태 업데이트
    protected override void OnTickedAllChildren()
    {
        LastStatus = _children[_children.Count - 1].LastStatus;
    }
}
