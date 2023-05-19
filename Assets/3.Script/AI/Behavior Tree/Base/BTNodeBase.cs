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

    //진입
    protected System.Func<BehaviorTree.ENodeStatus> onEnterFn;

    //업데이트
    protected System.Func<BehaviorTree.ENodeStatus> onTickFn;


    public BehaviorTree.ENodeStatus LastStatus { get; protected set; } = BehaviorTree.ENodeStatus.Unknown;
    public bool DecoratorsPermitRunning { get; protected set; } = true;


    //생성자
    public BTNodeBase(string _Name = "", System.Func<BehaviorTree.ENodeStatus> _OnEnterFn = null,
    System.Func<BehaviorTree.ENodeStatus> _OnTickFn = null)
    {
        Name = _Name;
        onEnterFn = _OnEnterFn;
        onTickFn = _OnTickFn;
    }



    //Node 추가하기
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

    void TickServices(float deltaTime)
    {
        foreach (var service in _services)
            service.OnTick(deltaTime);
    }
    bool EvaluateDecorators()
    {
        bool canRun = true;

        foreach (var decorator in _decorators)
        {
            canRun = decorator.Evaluate();
            //Evaluate는 BTSetup의 데코레이터 부분의 결과를
            //AND 연산한 결과

            //하나라도 false일시 중단
            if (!canRun)
                break;
        }

        //DecoratorsPermitRunning의 기본값은 true
        //값이 다르려면 DecoratorsPermitRunning을 외부에서 false로 바꿔줬어야..
        if (canRun != DecoratorsPermitRunning)
        {
            DecoratorsPermitRunning = canRun;

            //데코레이터의 실행 가능 여부가 변경되었다면 초기화
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

        //더 이상 없으면 처음으로 돌리기
        if (!tickedAnyNodes)
            Reset();
    }

    protected virtual void OnEnter()
    {
        //진입하기
        //뭔가 하나라도 Add 했으면 null이 아님
        if (onEnterFn != null)
            LastStatus = onEnterFn.Invoke();
        else
            LastStatus = _children.Count > 0 ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Succeeded;
    }

    protected virtual bool OnTick(float deltaTime)
    {
        bool tickedAnyNodes = false;

        if (!DecoratorsPermitRunning)
        {
            LastStatus = BehaviorTree.ENodeStatus.Failed;
            tickedAnyNodes = true;
            return tickedAnyNodes;
        }

        TickServices(deltaTime);
        //처음 진입시
        if (LastStatus == BehaviorTree.ENodeStatus.Unknown) //기본값
        {
            OnEnter();
            //진입
            tickedAnyNodes = true;

            if (LastStatus == BehaviorTree.ENodeStatus.Failed)
                return tickedAnyNodes;
        }

        // 진행 단계
        if (onEnterFn != null)
        {
            LastStatus = onEnterFn.Invoke(); //상태 바꾸기
            tickedAnyNodes = true; // 진행 중이면 true

            //끝에 닿았으면 false (성공/실패 상태 일때)
            if (LastStatus != BehaviorTree.ENodeStatus.InProgress)
                return tickedAnyNodes;
        }

        // 자녀 노드가 더 이상 없을 때
        if (_children.Count == 0)
        {
            if (onEnterFn == null)
                LastStatus = BehaviorTree.ENodeStatus.Succeeded; //성공 반환

            return tickedAnyNodes; //false 반환
        }

        for (int childIndex = 0; childIndex < _children.Count; ++childIndex)
        {

            //자식 노드 가져오기
            var child = _children[childIndex];

            //이전 데코레이터에 의해 활성화 되었나?
            bool childPreviouslyEnabledByDecorators = child.DecoratorsPermitRunning;
            //현재 데코레이터에 의해 활성화 되는가?
            bool childCurrentlyEnabledByDecorators = child.EvaluateDecorators();


            //자식 노드가 "InProgress" 상태일 때 
            if (child.LastStatus == BehaviorTree.ENodeStatus.InProgress)
            {
                //자식 노드가 실행되어있다면 true / 아니라면 false
                tickedAnyNodes |= child.OnTick(deltaTime);
                return tickedAnyNodes;
            }

            //자식 노드가 이미 기록되어 있다면 다음 자식으로 넘어가기
            if (child.LastStatus != BehaviorTree.ENodeStatus.Unknown)
                continue;

            tickedAnyNodes |= child.OnTick(deltaTime);


            LastStatus = child.LastStatus;
            //이전 데코레이터에서는 비활성화, 현재 데코레이터에서는 활성화인 경우
            if (!childPreviouslyEnabledByDecorators && childCurrentlyEnabledByDecorators)
            {
                for (int futureIndex = childIndex + 1; futureIndex < _children.Count; ++futureIndex)
                {
                    var futureChild = _children[futureIndex];
                    if (futureChild.LastStatus == BehaviorTree.ENodeStatus.InProgress)
                        futureChild.OnAbort();
                    else
                        futureChild.Reset();
                }
            }

            if (child.LastStatus == BehaviorTree.ENodeStatus.InProgress)
                return tickedAnyNodes;
            else if (child.LastStatus == BehaviorTree.ENodeStatus.Failed &&
                !ContinueEvaluatingIfChildFailed())
            {
                return tickedAnyNodes;
            }
            else if (child.LastStatus == BehaviorTree.ENodeStatus.Succeeded &&
                !ContinueEvaluatingIfChildSucceeded())
            {
                return tickedAnyNodes;
            }
        }
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
