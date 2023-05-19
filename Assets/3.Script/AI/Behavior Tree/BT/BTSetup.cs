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

        BTRoot.AddService<BTServiceBase>("목표 찾는 Service", (float deltaTime) => //한 번 실행, 타겟이 없을경우 더 이상 실행하지않음
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

        var chaseRoot = BTRoot.Add<BTNode_Sequence>("1. 목표가 있나요?");
        chaseRoot.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {

            //타겟이 있으면 true 없으면 false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;


        });

        chaseRoot.Add<BTNode_Action>("목표 찾음 : 찾아서 출발",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //_agent.SetDestination(currentTarget.transform.position);
            _agent.MoveTo(currentTarget.transform.position);

            return BehaviorTree.ENodeStatus.InProgress;
        },
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                //_agent.MoveTo(currentTarget.transform.position);

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                //return BehaviorTree.ENodeStatus.Succeeded;
            });

        //여기도 진입 안됨
        var StealRoot = BTRoot.Add<BTNode_Sequence>("2. 훔칠 수 있나요?");
        //목표에 닿았는지 확인하는 Service
        StealRoot.AddDecorator<BTDecoratorBase>("목표에 닿았는지 확인하는 Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            if (_agent.AtDestination)
            {
                //이거 기준도 이상함
                Debug.Log("다왔어용");
                return true;
                
            }
            return false;
            //return currentTarget != null;

        });
        StealRoot.Add<BTNode_Action>("목표 닿음 : 훔치기 실행",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             if(currentTarget!=null)
             {
                 currentTarget.transform.SetParent(transform);
             }

             else Debug.Log("타겟이 없어용");
             

             //여기가 문제 타겟이 왜 없어졌을까용
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             currentTarget.transform.SetParent(gameObject.transform);
             return BehaviorTree.ENodeStatus.Succeeded;

         });

        var runRoot = BTRoot.Add<BTNode_Sequence>("3. 목적지가 있나요?");
/*        runRoot.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {
            //타겟이 있으면 true 없으면 false
            //var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            //return newDestination != null;
        });
        runRoot.Add<BTNode_Action>("목적지 있음 : 도망 실행",
            () =>
            {
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.SetDestination(newDestination);
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.SetDestination(newDestination);
                return BehaviorTree.ENodeStatus.InProgress;
            });
*/













        var wanderRoot = BTRoot.Add<BTNode_Sequence>("목표 못 찾음 : 무작위 이동");
        wanderRoot.Add<BTNode_Action>("무작위 이동중",
        () =>
        {
            Vector3 location = _agent.PickLocationInRange(_wanderRange);
            _agent.MoveTo(location);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });




    }

}
