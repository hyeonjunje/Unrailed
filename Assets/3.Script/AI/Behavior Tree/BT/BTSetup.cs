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
        public static readonly BlackBoardKey RandomDestination = new BlackBoardKey() { Name = "RandomDestination" };

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
    DetectableTargetManager detectableTarget;


    private void Awake()
    {
        detectableTarget = FindObjectOfType<DetectableTargetManager>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<CharacterAgent>();
        _sensors = GetComponent<AwarenessSystem>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));


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

        var canChaseSel = BTRoot.Add<BTNode_Sequence>("SEQ. 목표가 있나요?");
        var canChaseDeco = canChaseSel.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {
            //타겟이 있으면 true 없으면 false
            //여기의 문제는 타겟이 생기는 순간 밑에거가 스킵되고 갑자기 실행이 된다는 점
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var oo = canChaseSel.Add<BTNode_Sequence>("d아악");

        var mainSeq = oo.Add<BTNode_Sequence>("Seq1 : 목표로 이동 시도");
        var ff = mainSeq.Add<BTNode_Action>("A 목표 찾음 : 찾아서 출발",
        () =>
        {
            //InProgress 상태에서 currentTarget이 바뀌면 버그나는듯
            Debug.Log("몇번하나요?");
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            //도착하지 않았다면

                _agent.MoveTo(currentTarget.transform.position);
                return BehaviorTree.ENodeStatus.InProgress;


/*            if(_agent.AtDestination)
            {

            }
            return BehaviorTree.ENodeStatus.Succeeded;*/
/*            if (!_agent.AtDestination)
            {
                _agent.MoveTo(currentTarget.transform.position);
                Debug.Log("이동중");
                //타겟으로 이동
                //이거 타겟 좌표가 두번째 타겟때 업데이트가 안됨
                //이름은 맞는데 좌표만 첫번째 타겟 좌표로 나옴........................................................뭐지?
                //return BehaviorTree.ENodeStatus.Succeeded;
            }
                return BehaviorTree.ENodeStatus.Succeeded;*/
/*            else
            {
                //도착했다면 성공 반환
                //!!두번째 타겟부터 AI가 이동하기도 전에 납치하는 버그 고쳐야함
                if(!_agent.isMoving)
                {
                Debug.Log("다왔어용");
                return BehaviorTree.ENodeStatus.Succeeded;

                }
                return BehaviorTree.ENodeStatus.InProgress;
            }
*/

        },
            () =>
            {
/*                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                if (!_agent.AtDestination)
                {
                    _agent.MoveTo(currentTarget.transform.position);
                    Debug.Log("이동중");
                }*/
                //return BehaviorTree.ENodeStatus.Succeeded;


                //return BehaviorTree.ENodeStatus.Succeeded;  
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
             if (_agent.AtDestination)
             {
                 var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                 currentTarget.transform.SetParent(transform);
                 currentTarget.transform.localPosition = Vector3.zero;
                 var newDestination = _agent.PickLocationInRange(10);
                 _localMemory.SetGeneric<Vector3>(BlackBoardKey.NewDestination, newDestination);

             }

             
                 return BehaviorTree.ENodeStatus.InProgress;

         },
         () =>
         {
             //_agent.CancelCurrentCommand();
                 var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //return BehaviorTree.ENodeStatus.Succeeded;
             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : 도망 시도");
        var runDeco = runRoot.AddDecorator<BTDecoratorBase>("목표로 이동할 수 있나요?", () =>
        {


            return true;
            //Vector3 pos = new Vector3(-20, 0, -2);
            //_localMemory.SetGeneric(BlackBoardKey.NewDestination, pos);

/*            var newwDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            return newwDestination != null;*/
        });
        var runAction = runRoot.Add<BTNode_Action>("목적지 있음 : 도망 실행",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                    var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);

                //전 목적지에 도착한 상태라면
                if(_agent.AtDestination)
                {
                    //도망치기

                }
                    _agent.MoveTo(newDestination);
                    return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                //return BehaviorTree.ENodeStatus.Succeeded;

            });

        //내려놓기

        var discardRoot = runRoot.Add<BTNode_Sequence>("Seq4 : 내려놓기 시도");
        discardRoot.AddDecorator<BTDecoratorBase>("내려놓을 수 있나요?", () =>
         {
            return true;

             //var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
             //var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //return currentTarget != null;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("버리기 실행",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
/*                if(currentTarget!=null&_agent.AtDestination)
                {
                    //버리기
                    currentTarget.transform.parent = null;
                    _agent.CancelCurrentCommand();
                    Debug.Log("버렸어용");
                    Destroy(currentTarget);
                    _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

                    //다음 목적지 설정
                    var randomDestination = _agent.PickLocationInRange(_wanderRange);
                    _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, randomDestination);
                    return BehaviorTree.ENodeStatus.Succeeded;

                }
                else
                return BehaviorTree.ENodeStatus.InProgress;*/


                currentTarget.transform.parent = null;
                //_agent.CancelCurrentCommand();
                Debug.Log("버렸어용");
                Destroy(currentTarget);
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

                //다음 목적지 설정
                var randomDestination = _agent.PickLocationInRange(_wanderRange);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, randomDestination);
                return BehaviorTree.ENodeStatus.Succeeded;



                //도착할 때까지 이동
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );






        var wanderRoot = BTRoot.Add<BTNode_Sequence>("목표 못 찾음 : 무작위 이동");
        /*        var wanderDeco = wanderRoot.AddDecorator<BTDecoratorBase>("무작위 위치 지정하기", () =>
                {
                    return true;
                });*/
        wanderRoot.Add<BTNode_Action>("무작위 이동중",
        () =>
        {
            var randomDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.RandomDestination);
            //var randomDestination = _agent.PickLocationInRange(_wanderRange);
            //_agent.MoveTo(randomDestination);
            //return BehaviorTree.ENodeStatus.InProgress;
                _agent.MoveTo(randomDestination);
            Debug.Log(randomDestination);
            Debug.Log(_agent.AtDestination);
                return BehaviorTree.ENodeStatus.InProgress;
/*            if(!_agent.AtDestination)
            {

            }

            else
            {
                _agent.CancelCurrentCommand();
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));
                return BehaviorTree.ENodeStatus.Succeeded;

            }*/

        },
        () =>
        {

            if(_agent.AtDestination)
            {
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));
            }
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });




    }

}
