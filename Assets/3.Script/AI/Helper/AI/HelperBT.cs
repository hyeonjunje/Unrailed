using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Text DebugTarget;

    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;

    protected WorldResource _target;
    protected WorldResource.EType _order;

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
        _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, WorldResource.EType.Wood);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT 시작");
        BTRoot.AddService<BTServiceBase>("명령을 기다리는 Service", (float deltaTime) =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            _order = _helper.TargetResource;
            
        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("명령이 있나요?");
        var deco = OrderRoot.AddDecorator<BTDecoratorBase>("명령이 바뀌었는지 확인", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var mainSequence = OrderRoot.Add<BTNode_Sequence>("명령이 있는 경우");
        var woodRoot = mainSequence.Add<BTNode_Sequence>("명령 : 나무/돌 캐기");
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
            DebugTarget.text = _item.name;
            _agent.MoveTo(_item.InteractionPoint);
            _animator.SetBool("isMove", true);
            //타겟 자원 설정
            _target = _helper.Home.GetGatherTarget(_helper);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = mainSequence.Add<BTNode_Action>("도구 들기", () =>
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


        var MoveToResource = mainSequence.Add<BTNode_Action>("자원으로 이동", () =>
         {
             //자원으로 이동하기
             DebugTarget.text = _target.name;
             Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(position);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = mainSequence.Add<BTNode_Action>("자원 채집하기", () =>
         {
             _animator.SetBool("isDig", true);
             _target.isDig();
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _target.isDig() ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Succeeded;
         }
        );




        var OrderChange = BTRoot.Add<BTNode_Sequence>("명령이 바뀐 경우");
        OrderChange.Add<BTNode_Action>("도구 내려놓기", () =>
         {
             //내려놓기
             _item.transform.position = transform.position;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             _animator.SetBool("isDig", false);

             //블랙보드 업데이트
             _item = null;
             _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

             return BehaviorTree.ENodeStatus.Succeeded;
         });




    }
}
