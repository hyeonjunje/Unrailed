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
    public Transform RightHand;

    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;
    protected WorldResource _target;

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
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, order);
            
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


            foreach(var item in ItemManager.Instance.RegisteredObjects)
            {
                //아이템이 명령이랑 호환이 된다면
                if(item.Type==order)
                {
                    foreach (var interaction in item.Interactions)
                    {
                        //플레이어가 들고 있는지 확인하기
                        if (!interaction.CanPerform())
                        {
                            Debug.Log("누가 쓰고 있나봐요");
                        }
                        _item = item;
                        Debug.Log($"{item.name} 주울 수 있어요");
                        break;
                    }

                }
            }

            //도구로 이동하기
            _agent.MoveTo(_item.InteractionPoint);
            _animator.SetBool("isMove", true);
            //타겟 자원 설정
            _target = _helper.Home.GetGatherTarget(_helper);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = mainSelector.Add<BTNode_Action>("도구 들기", () =>
         {

             _item.transform.SetParent(RightHand);
             _item.transform.localPosition = Vector3.zero;
             _item.transform.localRotation = Quaternion.identity;

             return BehaviorTree.ENodeStatus.InProgress;
         },
        () =>
        { 
       
             return _item.transform.parent == RightHand ? 
            BehaviorTree.ENodeStatus.Succeeded:BehaviorTree.ENodeStatus.InProgress;
        });


        var MoveToResource = mainSelector.Add<BTNode_Action>("자원으로 이동", () =>
         {
             //자원으로 이동하기
             Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(position);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = mainSelector.Add<BTNode_Action>("자원 채집하기", () =>
         {
             _animator.SetBool("isDig", true);
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );














        var ss = BTRoot.Add<BTNode_Action>("명령이 없는 경우", () =>
         {
             if(_helper.Home!=null)
             {
             _target = _helper.Home.GetGatherTarget(_helper);
             _agent.MoveTo(_target.transform.position);

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

            return BehaviorTree.ENodeStatus.InProgress;
        });





    }
}
