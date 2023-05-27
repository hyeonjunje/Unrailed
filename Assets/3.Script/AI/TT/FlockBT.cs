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

        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);

/*        Vector3 location = _agent.MoveToRandomPosition();
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, location);*/

        var BTRoot = _tree.RootNode.Add<BTNode_Sequence>("BT START");
        var wanderRoot = BTRoot.Add<BTNode_Action>("돌아다니기,,",
            () =>
            {
                Debug.Log("여기");
                //var destinaton = _localMemory.GetGeneric<Vector3>(BlackBoardKey.Destination);
                _agent.MoveToRandomPosition();

                //Vector3 location = _agent.PickLocationInRange(_wanderRange);
                //_localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, location);
                return BehaviorTree.ENodeStatus.InProgress;
            }, () =>
            {

                return _agent.AtDestination ?
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

            }
            );

    /*    var idleRoot = BTRoot.Add<BTNode_Action>("가운데로 모이기?",
        () =>
        {
            Debug.Log("2");
            //_agent.MoveTo(_flock.Center);
            _agent.MoveTo(_flock.Center);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {


            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });*/




    }


}


