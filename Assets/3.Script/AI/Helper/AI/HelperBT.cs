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
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };

        public string Name;

    }
    public Transform RightHand;
    public Transform BetweenTwoHands;

    public Text DebugTarget;

    private Vector3 _itemPosition;
    private Vector3 _home;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;

    private float _rotateSpeed = 10;

    protected WorldResource _target;
    protected WorldResource.EType _order;

    private void Awake()
    {
        _home = transform.position;
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
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
        var woodRoot = mainSequence.Add<BTNode_Sequence>("1. 도구 찾기");

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


            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = woodRoot.Add<BTNode_Action>("도구 들기", () =>
         {
             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                _item.transform.SetParent(BetweenTwoHands);
                     break;
                 default:
                     _item.transform.SetParent(RightHand);
                     break;
             }

             _item.transform.localPosition = Vector3.zero;
             _item.transform.localRotation = Quaternion.identity;

             return BehaviorTree.ENodeStatus.InProgress;
         },
        () =>
        { 
       
             return _item.transform.parent == RightHand || BetweenTwoHands ? 
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });






        var workRoot = mainSequence.Add<BTNode_Sequence>("2. 일하기",()=>
        {
            //타겟 자원 설정

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {
       
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
        });
        var target = workRoot.Add<BTNode_Action>("타겟 정하기", () =>
         {
             if(_target==null)
             {
                _target = _helper.Home.GetGatherTarget(_helper);
                Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);

             }
             return BehaviorTree.ENodeStatus.Succeeded;

         });

        var MoveToResource = workRoot.Add<BTNode_Action>("자원으로 이동", () =>
         {
             //자원으로 이동하기
             DebugTarget.text = _target.name;
             Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(pos);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = workRoot.Add<BTNode_Action>("자원 채집하기", () =>
         {
             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                     _item.GetComponent<Item_Bucket_Water>().StartFilling();

                     break;

                 default:
                    _animator.SetBool("isDig", true);
                    StartCoroutine(_target.isDigCo());
                     break;

             }
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>

         {
             Vector3 dir = _target.transform.position - transform.position;
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                     if (!_item.GetComponent<Item_Bucket_Water>().isFilling)
                     { 
                         return BehaviorTree.ENodeStatus.Succeeded;
                     }
                     else
                     _item.GetComponent<Item_Bucket_Water>().FillGauge();

                     break;

                 default:
                     //나무, 돌
                     //명령이 바뀌기 전까지 무한반복
                     if (!_target.isDig())
                     {
                         Destroy(_target.gameObject);
                         return BehaviorTree.ENodeStatus.Failed;
                     }
                     break;

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );


        var MoveToHome = workRoot.Add <BTNode_Action>("물 갖고 이동하기", ()=>
        {
            _agent.MoveTo(_home);
            return BehaviorTree.ENodeStatus.InProgress;

        }
        ,()=>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        
        }
        );

        var Sleep = workRoot.Add<BTNode_Action>("내려놓고 자기", () =>
         {
             _animator.SetBool("isMove", false);
             _item.transform.position = transform.position + Vector3.forward;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             return BehaviorTree.ENodeStatus.InProgress;

         },()=>
         {
        
             return BehaviorTree.ENodeStatus.InProgress;
        
         }
         );










        var OrderChange = BTRoot.Add<BTNode_Sequence>("명령이 바뀐 경우");
        OrderChange.Add<BTNode_Action>("도구 내려놓기", () =>
         {

             //내려놓기
             //PutDown();
             _item.transform.position = transform.position;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             _animator.SetBool("isDig", false);

             //블랙보드 업데이트
             _item = null;
             _target = null;
             _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

             return BehaviorTree.ENodeStatus.Succeeded;
         });

        



    }


    private void PutDown()
    {
        _item.transform.position = transform.position;
        _item.transform.rotation = Quaternion.identity;
        _item.transform.parent = null;
        _animator.SetBool("isDig", false);
    }
}
