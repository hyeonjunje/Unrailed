using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BTDecoratorBase : BTElementBase
{
    protected System.Func<bool> onEvaluateFn;
    public bool LastEvaluationResult { get; protected set; }

    public void Init(string _Name, System.Func<bool> _OnEvaluateFn)
    {
        Name = _Name;
        onEvaluateFn = _OnEvaluateFn;
    }

    public virtual bool Evaluate()
    {
        //null�� �ƴ϶�� OnEvaluateFn�� ȣ���Ͽ� �� ��� ���
        //null�̶�� false ��ȯ
        LastEvaluationResult = onEvaluateFn != null ? onEvaluateFn() : false;

        return LastEvaluationResult;
    }

    protected override void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0)
    {
        for (int index = 0; index < indentLevel; ++index)
            debugTextBuilder.Append(' ');

        debugTextBuilder.Append($"D: {Name} [{(LastEvaluationResult ? "PASS" : "FAIL")}]");
    }
}
