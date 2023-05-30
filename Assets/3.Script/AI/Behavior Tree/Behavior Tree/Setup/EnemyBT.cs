using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BehaviorTree))]
public class EnemyBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };
        public static readonly BlackBoardKey RandomDestination = new BlackBoardKey() { Name = "RandomDestination" };

        public string Name;


    }


    [Header("쫓아가는 기준")]
    //추적을 시작할 수 있는 최소 기준
    [SerializeField] private float _minAwarenessToChase = 1f;
    //추적을 멈추는 기준
    [SerializeField] private float _awarenessToStopChase = 2f;



    private Vector3 _itemPosition;
    private Animator _animator;

    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected AwarenessSystem _sensors;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _sensors = GetComponent<AwarenessSystem>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
;    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
   


        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작"); //셀렉터

        BTRoot.AddService<BTServiceBase>("목표 찾는 Service", (float deltaTime) => //매프레임 계속 실행
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

        var canChaseSeq = BTRoot.Add<BTNode_Sequence>("SEQ. 목표가 있나요?");
        var canChaseDeco = canChaseSeq.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {
            //타겟이 있으면 true 없으면 false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var startRoot = canChaseSeq.Add<BTNode_Sequence>("시작");
        var mainSeq = startRoot.Add<BTNode_Sequence>("Seq1 : 목표로 이동 시도");
        mainSeq.Add<BTNode_Action>("A 목표 찾음 : 찾아서 출발",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            _agent.MoveTo(currentTarget.transform.position);
            
            return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        var stealRoot = mainSeq.Add<BTNode_Sequence>("Seq2 : 훔치기 시도");
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("훔칠 타겟이 존재하는지 확인하는 Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;

        });

        var stealRootAction = stealDeco.Add<BTNode_Action>("A 타겟 존재 : 훔치기 실행",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             _animator.SetBool("Lifting", true);

             currentTarget.transform.SetParent(transform);
             currentTarget.transform.localPosition = _itemPosition;
             currentTarget.transform.localRotation = Quaternion.identity;
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : 도망 시도");
        runRoot.Add<BTNode_Action>("목적지 있음 : 도망 실행",
            () =>
            {
                //구석으로 가서 버리기
                _agent.MoveToClosestEndPosition();
                return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {
                return _agent.AtDestination ? 
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        //내려놓기

        var discardRoot = runRoot.Add<BTNode_Sequence>("Seq4 : 내려놓기 시도");
        discardRoot.AddDecorator<BTDecoratorBase>("내려놓을 수 있나요?", () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             return currentTarget.transform.parent = this.transform;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("버리기 실행",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                _animator.SetBool("Lifting", false);

                currentTarget.gameObject.AddComponent<Rigidbody>();
                currentTarget.transform.parent = null;
                Destroy(currentTarget.gameObject, 1);
                Destroy(currentTarget);
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
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
