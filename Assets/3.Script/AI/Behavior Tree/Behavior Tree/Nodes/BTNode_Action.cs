using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode_Action : BTNodeBase
{
    protected override void OnTickedAllChildren()
    {
        LastStatus = _children[_children.Count - 1].LastStatus;
    }

}
