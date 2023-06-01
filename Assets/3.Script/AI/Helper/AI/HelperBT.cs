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
        var CheckOrder = OrderRoot.AddDecorator<BTDecoratorBase>("명령이 바뀌었는지 확인", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var mainSequence = OrderRoot.Add<BTNode_Sequence>("명령이 있는 경우");


        #region 나무 캐기, 돌 캐기, 물 떠오기 명령
        var FindTools = mainSequence.Add<BTNode_Sequence>("1. 도구 찾기");

        //도끼 들기
        //나무로 이동하기
        //나무 베기
        var MoveToItem = FindTools.Add<BTNode_Action>("도구로 이동",()=>
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
                            Debug.Log($"{item.name}는 누가 쓰고 있어요");
                        }
                        else
                        {
                            interaction.Perform();
                            _item = item;
                        }
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

        var PickUpTool = FindTools.Add<BTNode_Action>("도구 들기", () =>
         {
             switch(_item.Type)
             {
                 //양동이면 양손
                 case WorldResource.EType.Water:
                    _item.transform.SetParent(BetweenTwoHands);
                     break;
                 //다른거면 한 손
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
             if(_target == null)
             {
                 //목표 자원
                 _target = _helper.Home.GetGatherTarget(_helper);
                Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);

             }
             return BehaviorTree.ENodeStatus.Succeeded;

         });
        var PossibleToWork = workRoot.Add<BTNode_Selector>("일하기 셀렉터");

        var PossibleSequence = PossibleToWork.Add<BTNode_Sequence>("가능", () =>
         {
             Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
             return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         }
        );
        var MoveToResource = PossibleSequence.Add<BTNode_Action>("자원으로 이동", () =>
         {
             //자원으로 이동하기
             DebugTarget.text = _target.name;
             _animator.SetBool("isMove", true);
             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });

        var ImpossibleToWork = PossibleToWork.Add<BTNode_Sequence>("불가능", () =>
        {
            //자원이 물 건너에 있을 때
            Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
            return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;

        });

        ImpossibleToWork.Add<BTNode_Action>("그거 못해요ㅠ", () =>
         {
             DebugTarget.text = "자는 중";
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
          {
              return BehaviorTree.ENodeStatus.InProgress;
          });



        var sel = workRoot.Add<BTNode_Selector>("자원 종류에 따라 다른 행동하기");
        var wood = sel.Add<BTNode_Sequence>("[나무] !",()=>
        {
            return _item.Type == WorldResource.EType.Wood || _item.Type ==WorldResource.EType.Stone
            ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
        
        });

        var CollectResource = wood.Add<BTNode_Action>("계속 채집하기", () =>
         {
             _animator.SetBool("isDig", true);
             StartCoroutine(_target.isDigCo());
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>

         {
             Vector3 dir = _target.transform.position - transform.position;
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

             if (!_target.isDig())
             {
                 Destroy(_target.gameObject);
                 _animator.SetBool("isDig", false);
                 return BehaviorTree.ENodeStatus.Succeeded;
             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );

        // 물 ==========================================================================
        var water = sel.Add<BTNode_Sequence>("[물] !",()=>
        {
            return _item.Type == WorldResource.EType.Water ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;

        });

        var Filling = water.Add <BTNode_Action>("물 채우기", ()=>
        {
            return BehaviorTree.ENodeStatus.InProgress;

        }
        ,()=>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);

            //아이템 상호작용 중 가능한 상호작용 찾기
            foreach (var interaction in _item.Interactions)
            {
                if (interaction.CanPerform())
                {
                    if (interaction.Perform())
                    {
                        return BehaviorTree.ENodeStatus.InProgress;
                    }
                    else
                        return BehaviorTree.ENodeStatus.Succeeded;
                }
            }
            return BehaviorTree.ENodeStatus.InProgress;

        }
        );

        var MoveToHome = water.Add<BTNode_Action>("물 갖다놓기", () =>
         {
             //나중에 기차 좌표로 바꾸기
            _agent.MoveTo(_home);
             return BehaviorTree.ENodeStatus.InProgress;

         },()=>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         }
         );

        var Sleep = water.Add<BTNode_Action>("자기", () =>
         {

             _item.transform.position = transform.position + Vector3.left;
             PutDown();
             DebugTarget.text = "자는 중";
             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         { 
         
             return BehaviorTree.ENodeStatus.InProgress;
         
         });

        #endregion




        var OrderChange = BTRoot.Add<BTNode_Sequence>("명령이 바뀐 경우");
        OrderChange.Add<BTNode_Action>("도구 내려놓고 자기", () =>
         {
             _item.transform.position = transform.position;
             PutDown();

             //블랙보드 업데이트
             _item = null;
             _target = null;
             _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

             return BehaviorTree.ENodeStatus.Succeeded;
         });

        



    }


    private void PutDown()
    {
        //_item.transform.position = transform.position;
        _item.transform.rotation = Quaternion.identity;
        _item.transform.parent = null;
        _animator.SetBool("isDig", false);
        _animator.SetBool("isMove", false);
    }
}
