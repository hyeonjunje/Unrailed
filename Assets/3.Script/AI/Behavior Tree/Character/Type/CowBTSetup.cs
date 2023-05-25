using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BehaviorTree))]
public class CowBTSetup : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };

        public string Name;
    }

    [Header("랜덤 이동 거리")]
    [SerializeField] private float _wanderRange = 3f;


    protected BehaviorTree _tree;
    protected CharacterAgent _agent;

    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<CharacterAgent>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);

        Vector3 location = _agent.PickLocationInRange(_wanderRange);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, location);

        var BTRoot = _tree.RootNode.Add<BTNode_Sequence>("BT START");
        var wanderRoot = BTRoot.Add<BTNode_Action>("돌아다니기,,",
            ()=>
            {
                var destinaton = _localMemory.GetGeneric<Vector3>(BlackBoardKey.Destination);
                _agent.MoveTo(destinaton);

                Vector3 location = _agent.PickLocationInRange(_wanderRange);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, location);
                return BehaviorTree.ENodeStatus.InProgress;
            },()=>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        
            }
            );

/*        var idleRoot = BTRoot.Add<BTNode_Action>("가만히 있기,,",
            () =>
            {
                var destinaton = _localMemory.GetGeneric<Vector3>(BlackBoardKey.Destination);
                _agent.StopNav();
                return BehaviorTree.ENodeStatus.InProgress;
            }, () =>
             {

                 
                 return _agent.isMoving ? BehaviorTree.ENodeStatus.Succeeded :
                                          BehaviorTree.ENodeStatus.InProgress;
             
             });
*/



    }


}
