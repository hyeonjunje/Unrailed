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

    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected Helper_Action _helper;

    System.Action action;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _helper = GetComponent<Helper_Action>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric(BlackBoardKey.Order, Helper_Action.EState.Sleep);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작");
        BTRoot.AddService<BTServiceBase>("명령을 기다리는 Service", (float deltaTime) =>
        {
            var order = _helper.HelperState;
            _localMemory.SetGeneric(BlackBoardKey.Order, order);
            
        });

        var dd = BTRoot.Add<BTNode_Sequence>("명령이 있나요?");
        dd.AddDecorator<BTDecoratorBase>("명령 확인", () =>
         {
             var order = _localMemory.GetGeneric<Helper_Action.EState>(BlackBoardKey.Order);
             return order != Helper_Action.EState.Sleep;
         });

        var mainSelector = dd.Add<BTNode_Selector>("명령이 있는 경우");
        var woodDecorator = mainSelector.AddDecorator<BTDecoratorBase>("명령 종류 확인", () =>
         {
             var order = _localMemory.GetGeneric<Helper_Action.EState>(BlackBoardKey.Order);
             return order == Helper_Action.EState.Wood;
         });

        var woodRoot = woodDecorator.Add<BTNode_Sequence>("명령 : 나무 캐기");
        //도끼 들기
        //나무로 이동하기
        //나무 베기
        woodRoot.Add<BTNode_Action>("도끼로 이동",()=>
        {
            //1. 레이어로 감지하기
            //2. 스크립트 넣기
            return BehaviorTree.ENodeStatus.InProgress;
        },()=>
        { 
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        woodRoot.Add<BTNode_Action>("나무로 이동하기",()=>
        {

            return BehaviorTree.ENodeStatus.InProgress;
        }
        ,()=>
        {

            return BehaviorTree.ENodeStatus.InProgress;
        });




    }
}
