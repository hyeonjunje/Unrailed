using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(BehaviorTree))]
public class HelperBT : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Order = new BlackBoardKey() { Name = "Order" };
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };
        public static readonly BlackBoardKey ResourceType = new BlackBoardKey() { Name = "ResourceType" };

        public string Name;

    }

    protected Blackboard<BlackBoardKey> _localMemory;

    //���� ��ġ�� ���߿� �ٲٱ�
    private Vector3 _home;
    private float _rotateSpeed = 10;

    protected Helper _helper;
    //����
    protected AI_Item _item;

    //���
    protected WorldResource.EType _order;

    //�̸�Ƽ��
    public Image Emote;
    protected EmoteManager _emote;



    private int isDig = Animator.StringToHash("isDig");

    private void Awake()
    {
        _home = transform.position;
        _emote = FindObjectOfType<EmoteManager>();

        _stack = GetComponent<AI_Stack>();
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, WorldResource.EType.Wood);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");
        BTRoot.AddService<BTServiceBase>("����� ��ٸ��� Service", (float deltaTime) =>
        {
            // �� ��� 
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            // ���� ���
            _order = _helper.TargetResource;

        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("����� �ֳ���?");
        var CheckOrder = OrderRoot.AddDecorator<BTDecoratorBase>("����� �ٲ������ Ȯ��", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var MainSequence = OrderRoot.Add<BTNode_Sequence>("����� �ִ� ���");


        #region ���� ĳ��, �� ĳ��, �� ������ ���
        var FindTools = MainSequence.Add<BTNode_Sequence>("1. ���� ã��");

        var MoveToItem = FindTools.Add<BTNode_Action>("���� ���ϱ�, �̵��ϱ�", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);

             if(_item==null)
             {

                 foreach (var item in ItemManager.Instance.RegisteredObjects)
                 {
                     //�������� ����̶� ȣȯ�� �ȴٸ�
                     if (item.Type == order)
                     {
                         foreach (var interaction in item.Interactions)
                         {
                             //�÷��̾ ��� �ִ��� Ȯ���ϱ�
                             if (interaction.CanPerform())
                             {
                                 interaction.Perform();
                                 _item = item;
                                 Emote.sprite = _emote.GetEmote(_item.Id());
                                 _agent.MoveTo(_item.InteractionPoint);
                                 _animator.SetBool(isMove, true);
                             }
                             else
                             {
                                 //������ ������̰ų�
                                 //���� �����ִµ� ���� �� �������ϰų� ���
                                 Emote.sprite = _emote.GetEmote(_emote.WarningEmote);
                                 Debug.Log($"{_item} : ����� �� ���� ��Ȳ�̿���.");
                                 return BehaviorTree.ENodeStatus.Failed;
                             }
                             break;
                         }

                     }
                 }


             }


            //������ �̵��ϱ�




             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });

        var PickUpTool = FindTools.Add<BTNode_Action>("���� ���", () =>
         {
             if (_item != null)
             {

                 switch (_item.Type)
                 {
                     //�絿�̸� ���
                     case WorldResource.EType.Water:
                         _item.transform.SetParent(TwoHandTransform);
                         break;
                     //���, ������ �� ��
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


        var workRoot = MainSequence.Add<BTNode_Sequence>("2. ���ϱ�", () =>
         {
            //Ÿ�� �ڿ� ����
            return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         });

        var target = workRoot.Add<BTNode_Action>("Ÿ�� ���ϱ�", () =>
         {
             if (_target == null)
             {
                 //��ǥ �ڿ�
                 _target = Home.GetGatherTarget(_helper);

                 if (_target != null)       
                 {
                     Emote.sprite = _emote.GetEmote((int)_target.Type+10);
                     Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                     _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);
                     return BehaviorTree.ENodeStatus.Succeeded;

                 }

             }
             return BehaviorTree.ENodeStatus.InProgress;

         });

        var PossibleToWork = workRoot.Add<BTNode_Selector>("���ϱ� ������");
        var PossibleSequence = PossibleToWork.Add<BTNode_Sequence>("����", () =>
         {
             if(_target!=null)
             {
                Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
                 return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
             }
             return BehaviorTree.ENodeStatus.InProgress;
         }
        );
        var MoveToResource = PossibleSequence.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             //�ڿ����� �̵��ϱ�
             _animator.SetBool(isMove, true);
             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });

        var ImpossibleToWork = PossibleToWork.Add<BTNode_Sequence>("�Ұ���", () =>
        {
            //�ڿ��� �� �ǳʿ� ���� ��

            Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
            return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;

        });

        ImpossibleToWork.Add<BTNode_Action>("Ÿ���� �� �� ���� ���� �־��", () =>
         {
             Emote.sprite = _emote.GetEmote(_emote.WarningEmote);
             _animator.SetBool(isMove, false);
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
          {
              return BehaviorTree.ENodeStatus.InProgress;
          });



        var sel = workRoot.Add<BTNode_Selector>("�ڿ� ������ ���� �ٸ� �ൿ�ϱ�");

        var wood = sel.Add<BTNode_Sequence>("[����, ��]", () =>
         {

             if (_target != null)
             {
                 return _target.Type == WorldResource.EType.Wood || _target.Type == WorldResource.EType.Stone
                    ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
             }

             else return BehaviorTree.ENodeStatus.Failed;

         });

        var CollectResource = wood.Add<BTNode_Action>("��� ä���ϱ�", () =>
         {
             _animator.SetBool(isDig, true);
             StartCoroutine(_target.isDigCo());

             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>

         {
             Vector3 dir = _target.transform.position - transform.position;
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

             if (!_target.isDig())
             {
                 Destroy(_target.gameObject);
                 _animator.SetBool(isDig, false);
                 return BehaviorTree.ENodeStatus.Succeeded;
             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );

        // �� ==========================================================================
        var water = sel.Add<BTNode_Sequence>("[��, �ڿ�]", () =>
         {
             return _target.Type == WorldResource.EType.Water || _target.Type == WorldResource.EType.Resource
             ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         });

        var Filling = water.Add<BTNode_Action>("�� ä��� / �ڿ� ���", () =>
         {
           switch (_target.Type)
           {

               case WorldResource.EType.Water:
                   break;

               case WorldResource.EType.Resource:
                   _stack.DetectGroundBlock(_target);


                         if (_stack._handItem.Count == 0)
                         {
                             _stack.InteractiveItemSpace();
                         }
                         //�� �� �ױ�
                         else
                         {
                            if (Home.dd(_agent))
                            {
                              _stack.InteractiveItem();
                            }

                         }
                   //ó�� ��� �� 

                   break;
           }


           return BehaviorTree.ENodeStatus.InProgress;

       }
        , () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                    //������ ��ȣ�ۿ� �� ������ ��ȣ�ۿ� ã��
                    //���̶�� �� �߱�
                    foreach (var interaction in _item.Interactions)
                     {
                         if (interaction.CanPerform())
                         {
                             if (interaction.Perform())
                             {
                                 _animator.SetBool(isMove, false);
                                 return BehaviorTree.ENodeStatus.InProgress;
                             }
                             else
                                 
                                 return BehaviorTree.ENodeStatus.Succeeded;
                         }
                     }
                     break;

                 case WorldResource.EType.Resource:
                     return BehaviorTree.ENodeStatus.Succeeded;

             }


             return BehaviorTree.ENodeStatus.InProgress;

         }
        );

        var MoveToHome = water.Add<BTNode_Action>("�� ���ٳ��� / ���� �ڿ����� �̵��ϱ�", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                     _animator.SetBool(isMove, true);
                     _agent.MoveTo(_home);
                     break;

                 case WorldResource.EType.Resource:
                     break;
             }
             return BehaviorTree.ENodeStatus.InProgress;
             //���߿� ���� ��ǥ�� �ٲٱ�

         }, () =>
          {
              return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
          }
         );

        var Sleep = water.Add<BTNode_Action>("�ڱ�", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                     PutDown();
                     Emote.sprite = _emote.GetEmote(_emote.SleepEmote);
                     break;

                 case WorldResource.EType.Resource:
                     break;
             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Resource:

                     Home.GetGatherTarget(_helper);
                     //�ڿ��� �� �̻� ���ٸ� 
                     if (!Home.dd(_agent))
                     {
                         return BehaviorTree.ENodeStatus.Succeeded;
                     }
                     else
                     {
                         return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

                     }
                         //�� �� ������� �ű��



                 case WorldResource.EType.Water:
                     return BehaviorTree.ENodeStatus.InProgress;
             }
             return BehaviorTree.ENodeStatus.InProgress;

         });

        var Carrying = water.Add<BTNode_Action>("�ڿ� ����ϱ�", () =>
        {
            //���߿� ���� ��ǥ�� �ٲٱ�
            _agent.MoveTo(_home);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        }
        );

        var PutDownResource = water.Add<BTNode_Action>("�ڿ� ��������", () =>
         {
             _currentblock = _stack.AroundEmptyBlockTranform;
             _stack.PutDown();

             _target = null;
             _currentblock = null;
            return BehaviorTree.ENodeStatus.InProgress;
         },
        () =>
        {
            Home.GetGatherTarget(_helper);
            //�� �̻� ä���� �ڿ��� ���°��
            if (Home.NonethisResourceType)
            {
                Emote.sprite = _emote.GetEmote(_emote.WarningEmote);
                _animator.SetBool(isMove, false);
                return BehaviorTree.ENodeStatus.InProgress;
            }

            else
            {
                var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
                if (order == _order)
                {
                    return _stack._handItem.Count == 0 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                }
                else
                    _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
                    return BehaviorTree.ENodeStatus.Succeeded;

            
            }
        }
        );

        #endregion


        var OrderChange = BTRoot.Add<BTNode_Sequence>("����� �ٲ� ���");
        OrderChange.Add<BTNode_Action>("���� �������� �ʱ�ȭ", () =>
         {
             PutDown();
             var reset = Reset();
             //Ÿ���� ���ٸ� �ڱ�
             //�ִٸ� ���� ��� �����ϱ�
             return reset ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
         });

        var CantDoAnything = BTRoot.Add<BTNode_Sequence>("����� ������ �� ���� ���");
        CantDoAnything.Add<BTNode_Action>("�ڱ�", () =>
        {
            Emote.sprite = _emote.GetEmote(_emote.WarningEmote);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             if(order!=_order)
             {
                _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
                 return BehaviorTree.ENodeStatus.Succeeded;
             }
             return BehaviorTree.ENodeStatus.InProgress;
         });


    }

  

    private void PutDown()
    {
        if (_item != null)
        {
            foreach (var interaction in _item.Interactions)
            {
                //��������
                if (!interaction.CanPerform())
                {
                    interaction.Perform();
                }
                break;
            }

            _item.transform.rotation = Quaternion.identity;
            _item.transform.parent = _stack.AroundEmptyBlockTranform;
            _item.transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * 0.15f);

            _animator.SetBool(isDig, false);
            _animator.SetBool(isMove, false);

        }
    }

    private bool Reset()
    {
        if(_target!=null)
        {
            _item = null;
            _target = null;
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
            return true;
        }

        return false;
    }


}
