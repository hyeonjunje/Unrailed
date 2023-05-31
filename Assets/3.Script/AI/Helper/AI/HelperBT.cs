using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(BehaviorTree))]
public class HelperBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Order = new BlackBoardKey() { Name = "Order" };
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };

        public string Name;

    }


    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    WorldResource Target;

    private void Awake()
    {
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작");
        BTRoot.AddService<BTServiceBase>("명령을 기다리는 Service", (float deltaTime) =>
        {
            var order = _helper.TargetResource;
            _localMemory.SetGeneric(BlackBoardKey.Order, order);
            
        });

        var dd = BTRoot.Add<BTNode_Sequence>("명령이 있나요?");
        var deco = dd.AddDecorator<BTDecoratorBase>("명령 확인", () =>
         {
             return _helper.Home != null;
         });

        var mainSelector = dd.Add<BTNode_Sequence>("명령이 있는 경우");
        var woodRoot = mainSelector.Add<BTNode_Sequence>("명령 : 나무/돌 캐기");
        //도끼 들기
        //나무로 이동하기
        //나무 베기
        var MoveToItem = woodRoot.Add<BTNode_Action>("도구로 이동",()=>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            Vector3 itemPosition;

            switch(order)
            {
                case WorldResource.EType.Stone:
                    //itemPosition = 곡괭이위치;
                    Debug.Log("곡괭이");
                    break;
                case WorldResource.EType.Wood:
                    Debug.Log("도끼");
                    break;
                case WorldResource.EType.Water:
                    Debug.Log("양동이");
                    break;

            }

            //타겟 설정
            Target = _helper.Home.GetGatherTarget(_helper);
            //도구로 이동하기
            Debug.Log(Target);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {
            return BehaviorTree.ENodeStatus.Succeeded;
            //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var MoveToResource = mainSelector.Add<BTNode_Action>("자원으로 이동", () =>
         {
             Vector3 position = Target.transform.position;
             //자원으로 이동하기
             Vector3 dd = _agent.FindCloestAroundEndPosition(position);
             _agent.MoveTo(dd);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
             //return BehaviorTree.ENodeStatus.InProgress;

         });














        var ss = BTRoot.Add<BTNode_Action>("명령이 없는 경우", () =>
         {
             if(_helper.Home!=null)
             {
             Target = _helper.Home.GetGatherTarget(_helper);
             _agent.MoveTo(Target.transform.position);

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

            return BehaviorTree.ENodeStatus.InProgress;
        });





    }
}
