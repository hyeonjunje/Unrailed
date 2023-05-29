using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(SmartObject_Tree))]
public class TreeInteraction : SimpleInteraction
{
    protected SmartObject_Tree _tree;

    protected void Awake()
    {
        _tree = GetComponent<SmartObject_Tree>();
    }

    public override bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        _tree.Cutting();

        return base.Perform(performer, onCompleted);
    }
    //나무 벨 수 있는 조건 : 해당 AI가 도끼를 갖고 있어야함

/*    public override bool CanPerform()
    {
    }*/

}
