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

    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;


    private Transform _currentblock;
    public Transform RayStartTransfrom => _rayStartTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;

    public Text DebugTarget;

    private Vector3 _home;
    private Animator _animator;
    private float _rotateSpeed = 10;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;
    protected AI_Stack _stack;


    protected WorldResource _target;
    protected WorldResource.EType _order;

    private void Awake()
    {
        _stack = GetComponent<AI_Stack>();
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
            // 전 명령 
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            // 현재 명령
            _order = _helper.TargetResource;

        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("명령이 있나요?");
        var CheckOrder = OrderRoot.AddDecorator<BTDecoratorBase>("명령이 바뀌었는지 확인", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var MainSequence = OrderRoot.Add<BTNode_Sequence>("명령이 있는 경우");


        #region 나무 캐기, 돌 캐기, 물 떠오기 명령
        var FindTools = MainSequence.Add<BTNode_Sequence>("1. 도구 찾기");

        var MoveToItem = FindTools.Add<BTNode_Action>("도구 정하기, 이동하기", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             foreach (var item in ItemManager.Instance.RegisteredObjects)
             {
                //아이템이 명령이랑 호환이 된다면
                if (item.Type == order)
                 {
                     foreach (var interaction in item.Interactions)
                     {
                        //플레이어가 들고 있는지 확인하기
                        if (interaction.CanPerform())
                         {
                             interaction.Perform();
                             _item = item;
                             _agent.MoveTo(_item.InteractionPoint);
                             DebugTarget.text = _item.name;
                         }
                         else
                         {
                            //누군가 사용중일때
                        }
                         break;
                     }

                 }

             }

            //도구로 이동하기

            _animator.SetBool("isMove", true);


             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });

        var PickUpTool = FindTools.Add<BTNode_Action>("도구 들기", () =>
         {
             if (_item != null)
             {

                 switch (_item.Type)
                 {
                     //양동이면 양손
                     case WorldResource.EType.Water:
                         _item.transform.SetParent(TwoHandTransform);
                         break;
                     //곡괭이, 도끼면 한 손
                     default:
                         _item.transform.SetParent(RightHandTransform);
                         break;
                 }

                 _item.transform.localPosition = Vector3.zero;
                 _item.transform.localRotation = Quaternion.identity;

                 return BehaviorTree.ENodeStatus.InProgress;
             }

             else
                 return BehaviorTree.ENodeStatus.Succeeded;
         },
        () =>
        {
            return BehaviorTree.ENodeStatus.Succeeded;
        });


        var workRoot = MainSequence.Add<BTNode_Sequence>("2. 일하기", () =>
         {
            //타겟 자원 설정
            return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         });

        var target = workRoot.Add<BTNode_Action>("타겟 정하기", () =>
         {
             if (_target == null)
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

        ImpossibleToWork.Add<BTNode_Action>("그거 못해요", () =>
         {
             DebugTarget.text = "자는 중";
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
          {
              return BehaviorTree.ENodeStatus.InProgress;
          });



        var sel = workRoot.Add<BTNode_Selector>("자원 종류에 따라 다른 행동하기");
        var wood = sel.Add<BTNode_Sequence>("[나무, 돌]", () =>
         {
             return _target.Type == WorldResource.EType.Wood || _target.Type == WorldResource.EType.Stone
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
        var water = sel.Add<BTNode_Sequence>("[물, 자원]", () =>
         {
             return _target.Type == WorldResource.EType.Water || _target.Type == WorldResource.EType.Resource
             ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;

         });

        var Filling = water.Add<BTNode_Action>("물 채우기", () =>
       {
           return BehaviorTree.ENodeStatus.InProgress;

       }
        , () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                    //아이템 상호작용 중 가능한 상호작용 찾기
                    //물이라면 물 뜨기
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
                     break;

                 case WorldResource.EType.Resource:
                    //자원이라면 세개 쌓기
                    _currentblock = _target.transform;
                     _stack.DetectGroundBlock(_target);
                     if (_stack._handItem.Count == 0)
                     {
                         _stack.InteractiveItemSpace();
                     }

                     else
                     {
                         _stack.InteractiveItem();
                     }
                     ResourceTracker.Instance.DeRegisterResource(_target);
                     Destroy(_target);
                     return BehaviorTree.ENodeStatus.Succeeded;

             }


             return BehaviorTree.ENodeStatus.InProgress;

         }
        );

        var MoveToHome = water.Add<BTNode_Action>("물 갖다놓기 / 다음 자원으로 이동하기", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                     _agent.MoveTo(_home);
                     break;

                 case WorldResource.EType.Resource:
                     break;



             }
             return BehaviorTree.ENodeStatus.InProgress;
             //나중에 기차 좌표로 바꾸기

         }, () =>
          {
              return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
          }
         );

        var Sleep = water.Add<BTNode_Action>("자기", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                         //PutDown();
                         DebugTarget.text = "자는 중";
                     break;

                 case WorldResource.EType.Resource:
                     break;


             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

             switch(_target.Type)
             {
                 case WorldResource.EType.Resource:
                     return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
                 case WorldResource.EType.Water:
                     return BehaviorTree.ENodeStatus.InProgress;



             }

             return BehaviorTree.ENodeStatus.InProgress;
             //자원


             //물
             /*             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
                          return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;*/

         });

        var Carrying = water.Add<BTNode_Action>("자원 운반하기", () =>
        {
            if(_target!=null)
            {
                _agent.MoveTo(_home);

            }
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            if (_target != null)
            {

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.InProgress;
            }
            else
                return BehaviorTree.ENodeStatus.InProgress;
            //나중에 바꾸기
        }
        );




        #endregion




        var OrderChange = BTRoot.Add<BTNode_Sequence>("명령이 바뀐 경우");
        OrderChange.Add<BTNode_Action>("도구 내려놓고 자기", () =>
         {
             //_target = null;
             PutDown();
             /*             if (_item != null)
                          {
                              foreach (var interaction in _item.Interactions)
                              {
                                  //내려놓기
                                  if (!interaction.CanPerform())
                                  {
                                      interaction.Perform();
                                  }
                                  break;
                              }
                              PutDown();

                          }*/

             //블랙보드 업데이트
             //_item = null;


             return BehaviorTree.ENodeStatus.Succeeded;
         });





    }


    private void PutDown()
    {
        if (_item != null)
        {
            foreach (var interaction in _item.Interactions)
            {
                //내려놓기
                if (!interaction.CanPerform())
                {
                    interaction.Perform();
                }
                break;
            }
            _item.transform.position = transform.position + Vector3.left;
            _item.transform.rotation = Quaternion.identity;
            _item.transform.parent = null;

            _item = null;
            _target = null;
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

            _animator.SetBool("isDig", false);
            _animator.SetBool("isMove", false);

        }





}
}
