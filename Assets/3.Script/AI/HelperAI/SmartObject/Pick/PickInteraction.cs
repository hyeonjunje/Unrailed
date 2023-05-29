using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickInteraction : SimpleInteraction
{
    protected SmartObject_Pick _pick;

    private void Awake()
    {
        _pick = GetComponent<SmartObject_Pick>();
    }

    //�ٴڿ� �ִ��� Ȯ���ϱ�
    public override bool CanPerform()
    {
        return base.CanPerform() && _pick.IsOn;
    }

    public override bool Perform(CommonAIBase performer, UnityAction<BaseInteraction> onCompleted)
    {
        _pick.Pick();
        return base.Perform(performer, onCompleted);
    }


}
