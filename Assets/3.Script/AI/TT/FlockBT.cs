using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BehaviorTree))]
public class FlockBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };

        public string Name;
    }

    [Header("랜덤 이동 거리")]
    [SerializeField] private float _wanderRange = 3f;


    protected BehaviorTree _tree;
    //protected CharacterAgent _agent;

    protected PathFindingAgent _agent;
    protected Transform _transform;
    protected Flock _flock;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _transform = GetComponent<Transform>();
        _flock = GetComponent<Flock>();
    }

    private void Start()
    {
        //Boids

        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, Vector3.zero);

        var BTRoot = _tree.RootNode.Add<BTNode_Sequence>("BT START");
        BTRoot.AddService<BTServiceBase>("충돌 가능성 감지", (float deltaTime) =>
        {
            //충돌 가능성이 있다면
            if(_flock.IsHeadingForCollision())
            {
                var separation = _flock.ObstacleRays();
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, separation);
                Debug.Log("위험해요");
            }
        });

        var isHeadingCollison = BTRoot.Add<BTNode_Sequence>("충돌 가능성 검사");
        isHeadingCollison.AddDecorator<BTDecoratorBase>("충돌 가능성이 있나요?", () =>
         {
             return !_flock.IsHeadingForCollision();
         });

        var wanderRoot = BTRoot.Add <BTNode_Sequence>("충돌 가능성 없음");


        var wander = wanderRoot.Add<BTNode_Action>("돌아다니기",
            () =>
            {
                _agent.MoveToRandomPosition();
                return BehaviorTree.ENodeStatus.InProgress;
            }, () =>
            {
                return _agent.AtDestination ?
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            );
        //1. 무리 가운데로 모이기(Cohesion)
        var cohesion = wanderRoot.Add<BTNode_Sequence>("Cohesion");
        var cohesionRoot = cohesion.Add<BTNode_Action>("무리 가운데로 모이기",
        () =>
        {
            //Cohesion
            Debug.Log("Cohesion");
            _agent.MoveTo(_flock.Center);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });

        //충돌 가능성이 있다면

        //2.자기들끼리는 피하기(Separation)
        var SeparationRoot = BTRoot.Add<BTNode_Sequence>("Separation");
        SeparationRoot.Add<BTNode_Action>("무리와 충돌하지 않기",
        () =>
        {
            //Separation
            Debug.Log("Separation");
            var ff = _localMemory.GetGeneric<Vector3>(BlackBoardKey.Destination);
            _agent.MoveTo(ff);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });

        //3. 무리의 방향을 따라가기(Alignment)





    }


}


