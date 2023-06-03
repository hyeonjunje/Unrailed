using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT2 : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };
        public static readonly BlackBoardKey RandomDestination = new BlackBoardKey() { Name = "RandomDestination" };

        public string Name;

    }

    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _stack = GetComponent<AI_Stack>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, null);



        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작");

        BTRoot.AddService<BTServiceBase>("훔칠 거 찾는 Service", (float deltaTime) =>
        {
            if (!Home.NonethisResourceType)
            {
                _target = Home.TargettoSteal(_agent);
                var target = Home.TargettoSteal(_agent);
                _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, target);

            }
            else
                _target = null;
                return;
        });

        var HaveTarget = BTRoot.Add<BTNode_Sequence>("훔칠 거 있나요?");
        var CheckTarget = HaveTarget.AddDecorator<BTDecoratorBase>("훔칠 거 있는지 거르는 DECO", () =>
        {
            //타겟이 있으면 true 없으면 false
            return _target != null;
        });

        var StartRoot = HaveTarget.Add<BTNode_Sequence>("1. 있는 경우");

        var MainSeq = StartRoot.Add<BTNode_Sequence>("1");
        MainSeq.Add<BTNode_Action>("이동하기",
        () =>
        {
            Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
            _agent.MoveTo(_target.transform.position);

            return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        var stealRoot = MainSeq.Add<BTNode_Sequence>("2");
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("훔칠 타겟이 존재하는지 확인하는 Decorator", () =>
        {
            return _target != null;

        });

        stealRoot.Add<BTNode_Action>("타겟 존재 : 훔치기 실행",
        () =>
        {
            _animator.SetBool("Lifting", true);
            if(_target!=null)
            {
                _stack.DetectGroundBlock(_target);
                //처음 드는 거 
                if (_stack._handItem.Count == 0)
                {
                    _stack.InteractiveItemSpace();
                }
                //그 후 쌓기
                else
                {
                    _stack.InteractiveItem();
                }
                return BehaviorTree.ENodeStatus.InProgress;

            }
            return BehaviorTree.ENodeStatus.Failed;



        },
         () =>
         {
             return _target != null ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
         });


        var dd = MainSeq.Add<BTNode_Sequence>("3");
        dd.Add<BTNode_Action>("쌓기",
            ()=>
            {
                return BehaviorTree.ENodeStatus.InProgress;

            },()=>
            {
                Home.TargettoSteal(_agent);
                //자원이 더 이상 없다면 
                if (Home.NonethisResourceType)
                {
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                    //세 개 들었으면 옮기기
                    return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

            });



        var runRoot = MainSeq.Add<BTNode_Sequence>("4");
        runRoot.Add<BTNode_Action>("도망",
            () =>
            {
                //구석으로 가서 버리기
                Vector3 position = _agent.MoveToClosestEndPosition();
                //_currentblock.position = position;
                return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {
                return _agent.AtDestination ?
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        //내려놓기

        var discardRoot = runRoot.Add<BTNode_Sequence>("4");
        discardRoot.Add<BTNode_Action>("버리기",
            () =>
            {
                _animator.SetBool("Lifting", false);
                _stack.ThrowResource();
                _target = null;


                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );



        var wanderRoot = BTRoot.Add<BTNode_Sequence>("목표 못 찾음 : 무작위 이동");
        wanderRoot.Add<BTNode_Action>("무작위 이동중",
        () =>
        {
            _agent.MoveToRandomPosition();
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

    }
}
