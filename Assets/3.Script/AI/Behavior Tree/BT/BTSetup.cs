using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BehaviorTree))]
public class BTSetup : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };

        public string Name;


    }


    //랜덤값 거리, Wander 상태일 때 이만큼 이동함
    [Header("랜덤 이동 거리")]
    [SerializeField] private float _wanderRange = 10f;
    [SerializeField] private float _newDestination = 30f;


    [Header("쫓아가는 기준")]
    //추적을 시작할 수 있는 최소 기준
    [SerializeField] private float _minAwarenessToChase = 1f;
    //추적을 멈추는 기준
    [SerializeField] private float _awarenessToStopChase = 2f;

    protected BehaviorTree _tree;
    protected CharacterAgent _agent;
    protected AwarenessSystem _sensors;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<CharacterAgent>();
        _sensors = GetComponent<AwarenessSystem>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작"); //셀렉터

        var service = BTRoot.AddService<BTServiceBase>("목표 찾는 Service", (float deltaTime) => //매프레임 계속 실행
        {
            if (_sensors.ActiveTargets == null || _sensors.ActiveTargets.Count == 0)
            {
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
                return;
            }

            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

            if (currentTarget != null)
            {
                foreach (var candidate in _sensors.ActiveTargets.Values)
                {
                    if (candidate.Detectable == currentTarget //처음에 감지한 애랑 따라가고 있는 상대랑 같고
                        &&candidate.Awareness >= _awarenessToStopChase) //인식 수준이 2보다 크거나 같으면 돌아가기
                    {
                        return;
                    }
                }

                //인식 수준이 1보다 낮다면 없앰
                currentTarget = null;
            }

            // 타겟이 없다면 새로운 타겟 찾기
            float highestAwareness = _minAwarenessToChase;
            foreach (var candidate in _sensors.ActiveTargets.Values)
            {
                // 새로운 타겟에 할당하기
                if (candidate.Awareness >= highestAwareness)
                {
                    currentTarget = candidate.Detectable;
                    highestAwareness = candidate.Awareness;
                }
            }


            //여기서 Set해주기
            _localMemory.SetGeneric(BlackBoardKey.CurrentTarget, currentTarget);
        });

        var canChaseSel = BTRoot.Add<BTNode_Sequence>("SEQ. 목표가 있나요?");
        var canChaseDeco = canChaseSel.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {
            //타겟이 있으면 true 없으면 false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var mainSeq = canChaseDeco.Add<BTNode_Sequence>("Seq1 : 목표로 이동 시도");

        mainSeq.Add<BTNode_Action>("A 목표 찾음 : 찾아서 출발",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            _agent.MoveTo(currentTarget.transform.position);

            if (Vector3.Distance(_agent.transform.position, currentTarget.transform.position) < 1)
            {
                _agent.CancelCurrentCommand();
                return BehaviorTree.ENodeStatus.Succeeded;

            }

            else
                return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                /*var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                  _agent.MoveTo(currentTarget.transform.position);*/

                //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                return BehaviorTree.ENodeStatus.Succeeded;
            });

        var stealRoot = mainSeq.Add<BTNode_Sequence>("Seq2 : 훔치기 시도");
        var dd = stealRoot.AddDecorator<BTDecoratorBase>("훔칠 타겟이 존재하는지 확인하는 Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;

        });

        var stealRootAction = dd.Add<BTNode_Action>("A 타겟 존재 : 훔치기 실행",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             currentTarget.transform.SetParent(transform);
             currentTarget.transform.localPosition = Vector3.zero;
             _agent.CancelCurrentCommand();
             return BehaviorTree.ENodeStatus.Succeeded;


            /* if (currentTarget != null)
             {
                 //다가가서 주울 수 있는 상태가 되어야함
                 if (Vector3.Distance(_agent.transform.position, currentTarget.transform.position) < 1)
                 {
                     currentTarget.transform.SetParent(transform);
                     currentTarget.transform.localPosition = Vector3.zero;
                     _agent.CancelCurrentCommand();
                     //_agent.StopNav();
                     return BehaviorTree.ENodeStatus.Succeeded;

                 }
             }

             else Debug.Log("없어졌어용");
             return BehaviorTree.ENodeStatus.Failed;*/
             //못 주우면 처음으로 돌아가기
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : 도망 시도");
        runRoot.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {
            Vector3 pos = new Vector3(-20, 0, -2);
            _localMemory.SetGeneric(BlackBoardKey.NewDestination, pos);
            var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            return newDestination != null;
        });
        var runAction = runRoot.Add<BTNode_Action>("목적지 있음 : 도망 실행",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.MoveTo(newDestination);

                if (Vector3.Distance(_agent.transform.position, newDestination) < 2)
                {
                    Debug.Log("다왔어용");
                    //currentTarget.transform.parent = null;
                    _agent.CancelCurrentCommand();
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                {
                    Debug.Log("가는 중이에용");
                    return BehaviorTree.ENodeStatus.InProgress;
                }


                //return BehaviorTree.ENodeStatus.Succeeded;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;

            });

        //내려놓기

        var discardRoot = mainSeq.Add<BTNode_Sequence>("Seq4 : 내려놓기 시도");
        discardRoot.AddDecorator<BTDecoratorBase>("내려놓을 수 있나요?", () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             return currentTarget != null;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("버리기 실행",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

                currentTarget.transform.parent = null;
                _agent.CancelCurrentCommand();
                Debug.Log("버렸어용");
                return BehaviorTree.ENodeStatus.Succeeded;
            },
            () =>
            {

                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );



        var wanderRoot = canChaseSel.Add<BTNode_Sequence>("목표 못 찾음 : 무작위 이동");
        wanderRoot.Add<BTNode_Action>("무작위 이동중",
        () =>
        {
            //이상하게 움직이는거 고치기
            Vector3 location = _agent.PickLocationInRange(_wanderRange);
            _agent.MoveTo(location);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });




    }

}
