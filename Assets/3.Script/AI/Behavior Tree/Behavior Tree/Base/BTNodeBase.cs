using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BTNodeBase : BTElementBase
{
    protected List<BTNodeBase> _children = new List<BTNodeBase>();
    protected List<BTDecoratorBase> _decorators = new List<BTDecoratorBase>();
    protected List<BTServiceBase> _services = new List<BTServiceBase>();

    //����
    protected System.Func<BehaviorTree.ENodeStatus> onEnterFn;

    //������Ʈ
    protected System.Func<BehaviorTree.ENodeStatus> onTickFn;


    public BehaviorTree.ENodeStatus LastStatus { get; protected set; } = BehaviorTree.ENodeStatus.Unknown;
    public bool DecoratorsPermitRunning { get; protected set; } = true;

    //������
    public BTNodeBase(string _Name = "", System.Func<BehaviorTree.ENodeStatus> _OnEnterFn = null,
    System.Func<BehaviorTree.ENodeStatus> _OnTickFn = null)
    {
        Name = _Name;
        onEnterFn = _OnEnterFn;
        onTickFn = _OnTickFn;
    }



    //Node �߰��ϱ�
    public BTNodeBase Add<T>(string _Name,
        System.Func<BehaviorTree.ENodeStatus> _OnEnterFn = null,
        System.Func<BehaviorTree.ENodeStatus> _OnTickFn = null) where T : BTNodeBase, new()
    {
        T newNode = new T();
        newNode.Name = _Name;
        newNode.onEnterFn = _OnEnterFn;
        newNode.onTickFn = _OnTickFn;

        return Add(newNode);
    }

    public BTNodeBase Add<T>(T newNode) where T : BTNodeBase
    {
        _children.Add(newNode);
        return newNode;
    }


    public BTNodeBase AddService<T>(string _Name, System.Action<float> _OnTickFn) where T : BTServiceBase, new()
    {
        T newService = new T();
        newService.Init(_Name, _OnTickFn);

        _services.Add(newService);

        return this;
    }

    public BTNodeBase AddService<T>(T newService) where T : BTServiceBase
    {
        _services.Add(newService);

        return this;
    }

    public BTNodeBase AddDecorator<T>(string _Name, System.Func<bool> _OnEvaluateFn) where T : BTDecoratorBase, new()
    {
        T newDecorator = new T();
        newDecorator.Init(_Name, _OnEvaluateFn);

        _decorators.Add(newDecorator);

        return this;
    }

    public BTNodeBase AddDecorator<T>(T newDecorator) where T : BTDecoratorBase
    {
        _decorators.Add(newDecorator);

        return this;
    }

    private void TickServices(float deltaTime)
    {
        foreach (var service in _services)
            service.OnTick(deltaTime);
    }
    private bool EvaluateDecorators()
    {
        bool canRun = true;

        foreach (var decorator in _decorators)
        {
            canRun = decorator.Evaluate();
            //Evaluate�� BTSetup�� ���ڷ����� �κ��� �����
            //AND ������ ���

            //�ϳ��� false�Ͻ� �ߴ�
            if (!canRun)
                break;
        }

        if (canRun != DecoratorsPermitRunning)
        {
            DecoratorsPermitRunning = canRun;

            //���ڷ������� ���� ���� ���ΰ� ����Ǿ��ٸ� �ʱ�ȭ
            if (canRun)
                Reset();
        }

        return canRun;
    }
    protected virtual void OnAbort()
    {
        Reset();
    }

    public virtual void Reset()
    {
        LastStatus = BehaviorTree.ENodeStatus.Unknown;

        foreach (var child in _children)
            child.Reset();
    }

    public void Tick(float deltaTime)
    {
        bool tickedAnyNodes = OnTick(deltaTime);

        //�� �̻� ������ ó������ ������
        if (!tickedAnyNodes)
            Reset();
    }

    protected virtual void OnEnter()
    {
        //�����ϱ�
        //���� �ϳ��� Add ������ null�� �ƴ�
        if (onEnterFn != null)
            LastStatus = onEnterFn.Invoke();
        else
            LastStatus = _children.Count > 0 ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Succeeded;
    }

    protected virtual bool OnTick(float deltaTime)
    {
        //��ȯ���� false��� ��� ����
        //��ȯ���� true��� ��� ����


        bool tickedAnyNodes = false;

        //���ڷ����� ������ �����ϴ��� �˻�
        //DecoratorsPermitRunning�� �⺻���� true
        //�̰� false���
        if (!DecoratorsPermitRunning)
        {
            //���з� �����ϰ� �� ��带 �����ߴٰ� ǥ��
            LastStatus = BehaviorTree.ENodeStatus.Failed;
            tickedAnyNodes = true;
            return tickedAnyNodes;
        }

        //���� ������Ʈ�ϱ�
        TickServices(deltaTime);


        //ó�� ���Խ�
        if (LastStatus == BehaviorTree.ENodeStatus.Unknown) //�⺻��
        {
            //����
            OnEnter();
            tickedAnyNodes = true;

            if (LastStatus == BehaviorTree.ENodeStatus.Failed)
                return tickedAnyNodes;
        }

        // ���� �ܰ�
        if (onTickFn != null)
        {
            //onTick �Լ��� �����Ͽ� ��ȯ�� ���·� ������Ʈ
            LastStatus = onTickFn.Invoke(); 
            tickedAnyNodes = true;

            //��ȯ�Ѱ� �������� �ƴ� ����/���� ������ ���� Tick�� ��� ��带 ��ȯ�ϰ�
            // 
            if (LastStatus != BehaviorTree.ENodeStatus.InProgress)
                return tickedAnyNodes;
        }

        // �ڳ� ��尡 ���� ��
        if (_children.Count == 0)
        {
            if (onTickFn == null)
                LastStatus = BehaviorTree.ENodeStatus.Succeeded; //���� ���·� ������Ʈ

            return tickedAnyNodes; // ���� 
        }

        //����
        for (int childIndex = 0; childIndex < _children.Count; ++childIndex)
        {

            //�ڽ� ��� ��������
            var child = _children[childIndex];

            //���� ���ڷ����Ϳ��� ���� ����
            bool childPreviouslyEnabledByDecorators = child.DecoratorsPermitRunning;


            //canRun ��ȯ��
            bool childCurrentlyEnabledByDecorators = child.EvaluateDecorators();

            //���࿡ ������ childPreviouslyEnabledByDecorators��
            //1. false�� ���
            //�׷��� ���⼭ EvaluateDecorators�� �� �� canRun�� true��
            //DecoratorsPermitRunning�� false�� ��Ȳ�� �߻��ؼ� ���⼭ �ڽĵ��� �� �ʱ�ȭ�� ��
            //true

            //2. true�� ���
            //�ڽĵ� �ʱ�ȭ ���ϰ� true


            //�ڽ� ��尡 "InProgress" ������ �� 
            if (child.LastStatus == BehaviorTree.ENodeStatus.InProgress)
            {
                //�ڽ� ��尡 �����Ű��
                tickedAnyNodes |= child.OnTick(deltaTime);
                //����, ���� ���� ��ȯ
                return tickedAnyNodes;
            }

            //�ڽ� ��尡 �������̶�� ���� ���� �Ѿ��
            if (child.LastStatus != BehaviorTree.ENodeStatus.Unknown)
                continue;

            //�ڽ� ��� �����Ű��
            tickedAnyNodes |= child.OnTick(deltaTime);


            LastStatus = child.LastStatus;
            //�� �ʱ�ȭ ���� ��Ȳ
            if (!childPreviouslyEnabledByDecorators && childCurrentlyEnabledByDecorators)
            {
                //���� �ڽĵ鵵 �� �ʱ�ȭ
                for (int futureIndex = childIndex + 1; futureIndex < _children.Count; ++futureIndex)
                {
                    var futureChild = _children[futureIndex];
                    if (futureChild.LastStatus == BehaviorTree.ENodeStatus.InProgress)
                        futureChild.OnAbort();
                    else
                        //����/���н� �ڽĵ� ���¸� Unknown���� �ٲ��ֱ�
                        futureChild.Reset();
                }
            }


            if (child.LastStatus == BehaviorTree.ENodeStatus.InProgress)
                return tickedAnyNodes;

            //�߰����� �򰡰� �ʿ� ���� ��Ȳ
            //Sequence�� ���, ���� �� ó������ ���ư���
            else if (child.LastStatus == BehaviorTree.ENodeStatus.Failed &&
                !ContinueEvaluatingIfChildFailed())
            {
                return tickedAnyNodes;
            }
            //Selector�� ���, ���� �� ó������ ���ư���
            else if (child.LastStatus == BehaviorTree.ENodeStatus.Succeeded &&
                !ContinueEvaluatingIfChildSucceeded())
            {
                return tickedAnyNodes;
            }
        }
        //���� ������Ʈ
        OnTickedAllChildren();

        return tickedAnyNodes;
    }

    protected virtual bool ContinueEvaluatingIfChildFailed()
    {
        return true;
    }

    protected virtual bool ContinueEvaluatingIfChildSucceeded()
    {
        return true;
    }

    protected virtual void OnTickedAllChildren()
    {

    }

    public string GetDebugText()
    {

        StringBuilder debugTextBuilder = new StringBuilder();
        GetDebugTextInternal(debugTextBuilder);
        return debugTextBuilder.ToString();
    }

    protected override void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0)
    {
        for (int index = 0; index < indentLevel; ++index)
            debugTextBuilder.Append(' ');

        debugTextBuilder.Append($"{Name} [{LastStatus.ToString()}]");

        foreach (var service in _services)
        {
            debugTextBuilder.AppendLine();
            debugTextBuilder.Append(service.GetDebugText(indentLevel + 1));
        }

        foreach (var decorator in _decorators)
        {
            debugTextBuilder.AppendLine();
            debugTextBuilder.Append(decorator.GetDebugText(indentLevel + 1));
        }

        foreach (var child in _children)
        {
            debugTextBuilder.AppendLine();
            child.GetDebugTextInternal(debugTextBuilder, indentLevel + 2);
        }
    }

}
