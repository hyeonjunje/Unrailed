using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey HP = new BlackBoardKey() { Name = "HP" };

        public string Name;
    }

    private Blackboard<BlackBoardKey> _localMemory;
    private AnimalHealth _health;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _stack = GetComponent<AI_Stack>();
        _health = GetComponent<AnimalHealth>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
        _animator.SetBool(isMove, true);

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

        var HaveTarget = BTRoot.Add<BTNode_Selector>("훔칠 거 있나요?");
        var CheckTarget = HaveTarget.AddDecorator<BTDecoratorBase>("훔칠 거 있는지 거르는 DECO", () =>
        {
            //타겟이 있으면 true 없으면 false
            var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
            return _target != null && hp == _health.CurrentHp;
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
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("훔칠 타겟이 아직 존재하는지 확인하는 Decorator", () =>
        {
            return _target != null;

        });

        stealRoot.Add<BTNode_Action>("타겟 존재 : 훔치기 실행",
        () =>
        {
            if(_target!=null)
            {
                _animator.SetBool(isMove, false);
                _stack.EnemyDetectGroundBlock(_target);
                //처음 드는 거 
                if (_stack._handItem.Count == 0)
                {
                    _stack.EnemyInteractiveItem();
                }
                //그 후 쌓기
                else
                {
                    _stack.EnemyInteractiveAuto();
                }
                return BehaviorTree.ENodeStatus.InProgress;

            }
            return BehaviorTree.ENodeStatus.Failed;



        },
         () =>
         {
             return _target != null ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
         });


        var StackResource = MainSeq.Add<BTNode_Sequence>("3");
        StackResource.Add<BTNode_Action>("쌓기",
            ()=>
            {
                return BehaviorTree.ENodeStatus.InProgress;

            },()=>
            {
                Home.TargettoSteal(_agent);
                //자원이 더 이상 없다면 
                if (Home.NonethisResourceType || _stack._handItem.Peek().EnemyCheckItemType)
                {
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                    //세 개 들었으면 옮기기
                    return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

            });


        var RunRoot = MainSeq.Add<BTNode_Sequence>("4");
        RunRoot.Add<BTNode_Action>("도망",
        () =>
        {
            Vector3 position = _agent.MoveToClosestEndPosition();
            return BehaviorTree.ENodeStatus.InProgress;

        },
        () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;
        });


        //내려놓기

        var DiscardRoot = RunRoot.Add<BTNode_Sequence>("5");
        DiscardRoot.Add<BTNode_Action>("버리기",
            () =>
            {
                SoundManager.Instance.PlaySoundEffect("Enemy_Laugh");
                _stack.EnemyThrowResource();
                _target = null;
                _animator.SetBool(isMove, true);
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );


        var Attacked = BTRoot.Add<BTNode_Sequence>("맞음");
        Attacked.Add<BTNode_Action>("공격 당함", () =>
         {
             if(_stack._handItem.Count!=0)
             {
                _currentblock = _stack.BFS(this);
                _stack.EnemyPutDown();
                _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
                 _animator.SetBool(isMove, true);
                 _target = null;
             }
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _target==null ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;

         });



        var WanderRoot = BTRoot.Add<BTNode_Sequence>("목표 못 찾음");
        WanderRoot.Add<BTNode_Action>("무작위 이동중",
        () =>
        {
            _animator.SetBool(isMove, true);
            _agent.MoveToRandomPosition();
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

    }
}
