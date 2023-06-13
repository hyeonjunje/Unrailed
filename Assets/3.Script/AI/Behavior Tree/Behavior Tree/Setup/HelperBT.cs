using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HelperBT : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Order = new BlackBoardKey() { Name = "Order" };
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };
        public static readonly BlackBoardKey Shopping = new BlackBoardKey() { Name = "Shopping" };
        public static readonly BlackBoardKey ResourceType = new BlackBoardKey() { Name = "ResourceType" };
        public static readonly BlackBoardKey Arrive = new BlackBoardKey() { Name = "Arrive" };
        public static readonly BlackBoardKey Home = new BlackBoardKey() { Name = "Home" };
        public static readonly BlackBoardKey Item = new BlackBoardKey() { Name = "Item" };

        public string Name;

    }


    private Blackboard<BlackBoardKey> _localMemory;


    private Helper _helper;
    //����
    private AI_Item _item;
    //���
    private WorldResource.EType _order;

    public bool _arrive = false;

    public float currentTime = 0;

    //�̸�Ƽ��
    private EmoteManager _emoteManager;
    [SerializeField] private Image _emoteImage;
    private float _defaultSpeed;


    private readonly int isDig = Animator.StringToHash("isDig");

    private void Awake()
    {
        _emoteManager = FindObjectOfType<EmoteManager>();
        _stack = GetComponent<AI_Stack>();
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _defaultSpeed = _agent.moveSpeed;
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, WorldResource.EType.Wood);
        _localMemory.SetGeneric<bool>(BlackBoardKey.Arrive, _arrive);

        Vector3 home = _agent.FindCloestAroundEndPosition(GoalManager.Instance.lastRail.transform.position);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Home, home);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");
        BTRoot.AddService<BTServiceBase>("����� ��ٸ��� Service", (float deltaTime) =>
        {
            // ��
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            _order = _helper.TargetResource;

        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("����� �ֳ���?");
        var CheckOrder = OrderRoot.AddDecorator<BTDecoratorBase>("����� �ٲ������ Ȯ��", () =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            var arrive = _localMemory.GetGeneric<bool>(BlackBoardKey.Arrive);
            //����� �ٲ���ų� �����ߴٸ�
            return order == _order && arrive == _helper.arrive;
        });

        var MainSequence = OrderRoot.Add<BTNode_Sequence>("����� �ִ� ���");
        #region ���� ĳ��, �� ĳ��, �� ������ ���


        #region ��� ������
        var FindTools = MainSequence.Add<BTNode_Sequence>("1. ���� ã��");

        var MoveToItem = FindTools.Add<BTNode_Action>("���� ���ϱ�, �̵��ϱ�", () =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);

            if (_item == null)
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
                                _localMemory.SetGeneric<AI_Item>(BlackBoardKey.Item, item);
                                _emoteImage.sprite = _emoteManager.GetEmote(item.ID);
                                _agent.MoveTo(item.InteractionPoint);
                                _animator.SetBool(isMove, true);
                            }
                            else
                            {
                                //������ ������̰ų�
                                //���� �����ִµ� ���� �� �������ϰų� ���
                                _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.WarningEmote);
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
            //����
            var item = _localMemory.GetGeneric<AI_Item>(BlackBoardKey.Item);
            _item = item;

            if (_item != null)
            {
                foreach (var interaction in item.Interactions)
                {
                    //�÷��̾ ��� �ִ��� Ȯ���ϱ�
                    if (interaction.CanPerform())
                    {
                        item.PickUp();
                        SoundManager.Instance.PlaySoundEffect("Player_ToolsUp");
                        _emoteImage.sprite = _emoteManager.GetEmote(item.ID);
                        break;
                    }
                }

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


        var WorkRoot = MainSequence.Add<BTNode_Sequence>("2. ���ϱ�", () =>
        {
            //Ÿ�� �ڿ� ����
            return BehaviorTree.ENodeStatus.InProgress;

        }, () =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
        });

        var Target = WorkRoot.Add<BTNode_Action>("Ÿ�� ���ϱ�", () =>
        {
            if (_target == null)
            {
                //��ǥ �ڿ�
                _target = Home.GetGatherTarget(_helper);

                if (_target != null)
                {
                    _emoteImage.sprite = _emoteManager.GetEmote((int)_target.Type + 10);
                    Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                    _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);
                    return BehaviorTree.ENodeStatus.Succeeded;

                }

            }
            return BehaviorTree.ENodeStatus.InProgress;

        });

        var PossibleToWork = WorkRoot.Add<BTNode_Selector>("���ϱ� ������");
        var PossibleSequence = PossibleToWork.Add<BTNode_Sequence>("����", () =>
        {
            if (_target != null)
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
            if (_target != null)
            {
                Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
                return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;
            }
            else return BehaviorTree.ENodeStatus.InProgress;
        });

        ImpossibleToWork.Add<BTNode_Action>("Ÿ���� �� �� ���� ���� �־��", () =>
        {
            _target = Home.ResearchTarget(_helper);
            return BehaviorTree.ENodeStatus.Succeeded;
        });


        ImpossibleToWork.Add<BTNode_Action>("Ÿ���� �� �� ���� ���� �־��", () =>
        {
            if (_target != null)
            {
                Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                _agent.MoveTo(position);

                return BehaviorTree.ENodeStatus.InProgress;
            }

            return BehaviorTree.ENodeStatus.Failed;
        },
         () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });



        #endregion

        #region ����, ��
        var CheckTargetType = WorkRoot.Add<BTNode_Selector>("�ڿ� ������ ���� �ٸ� �ൿ�ϱ�");

        var WoodOrStone = CheckTargetType.Add<BTNode_Sequence>("[����, ��]", () =>
        {

            if (_target != null)
            {
                return _target.Type == WorldResource.EType.Wood || _target.Type == WorldResource.EType.Stone
                   ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
            }

            else return BehaviorTree.ENodeStatus.Failed;

        });

        var CollectResource = WoodOrStone.Add<BTNode_Action>("��� ä���ϱ�", () =>
        {
            _animator.SetBool(isDig, true);
            StartCoroutine(_target.isDigCo());

            return BehaviorTree.ENodeStatus.InProgress;

        }, () =>

        {
            if (_target == null)
            {
                return BehaviorTree.ENodeStatus.Failed;
            }
            else
            {
                Vector3 dir = _target.transform.position - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);
            }

            if (!_target.isDig())
            {
                if (_target.Type == WorldResource.EType.Wood) SoundManager.Instance.PlaySoundEffect("Wood_Broken");
                if (_target.Type == WorldResource.EType.Stone) SoundManager.Instance.PlaySoundEffect("Steel_Broken");
                Destroy(_target.gameObject);
                _animator.SetBool(isDig, false);
                return BehaviorTree.ENodeStatus.Succeeded;
            }

            return BehaviorTree.ENodeStatus.InProgress;
        }
        );
        #endregion

        #region ��, �ڿ�
        // �� ==========================================================================
        var WaterOrResource = CheckTargetType.Add<BTNode_Sequence>("[��, �ڿ�]", () =>
        {
            if (_target != null)
            {
                return _target.Type == WorldResource.EType.Water || _target.Type == WorldResource.EType.Resource
                ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
            }

            else return BehaviorTree.ENodeStatus.Failed;
        });

        var Interaction = WaterOrResource.Add<BTNode_Action>("�� ä��� / �ڿ� ���", () =>
        {
            switch (_target.Type)
            {
                case WorldResource.EType.Water:
                    SoundManager.Instance.PlaySoundEffect("Player_WaterImport");
                    break;

                case WorldResource.EType.Resource:
                    _stack.DetectGroundBlock(_target);
                    if (_stack.HandItem.Count == 0)
                    {
                        SoundManager.Instance.PlaySoundEffect("Item_Up");
                        _stack.InteractiveItem();
                    }
                    //�� �� �ױ�
                    else
                    {
                        if (!_stack.HandItem.Peek().HelperCheckItemType)
                        {
                            SoundManager.Instance.PlaySoundEffect("Item_Up");
                            _stack.InteractiveItemAuto();
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

        var MoveToHome = WaterOrResource.Add<BTNode_Action>("�� ���ٳ��� / ���� �ڿ����� �̵��ϱ�", () =>
        {
            switch (_target.Type)
            {
                case WorldResource.EType.Water:
                    _animator.SetBool(isMove, true);
                    Vector3 home = _agent.FindCloestAroundEndPosition(GoalManager.Instance.lastRail.transform.position);
                    _agent.MoveTo(home);
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

        var SleepRoot = WaterOrResource.Add<BTNode_Action>("�絿�� ��������, �� �� ������", () =>
        {
            switch (_target.Type)
            {
                case WorldResource.EType.Water:
                    PutDown();
                    _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.SleepEmote);
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
                    if (Home.NonethisResourceTypeHelper || _stack.HandItem.Peek().HelperCheckItemType)
                    {
                        return BehaviorTree.ENodeStatus.Succeeded;
                    }
                    else
                    {
                        return _stack.HandItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

                    }
                //�� �� ������� �ű��



                case WorldResource.EType.Water:
                    return BehaviorTree.ENodeStatus.InProgress;
            }
            return BehaviorTree.ENodeStatus.InProgress;

        });

        var CarryingResource = WaterOrResource.Add<BTNode_Action>("�ڿ� ����ϱ�", () =>
        {
            Vector3 home = _agent.FindCloestAroundEndPosition(GoalManager.Instance.lastRail.transform.position);
            _agent.MoveTo(home);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        }
        );

        var PutDownResource = WaterOrResource.Add<BTNode_Action>("�ڿ� ��������", () =>
        {
            _currentblock = _stack.BFS(this);
            _stack.PutDown();

            _target = null;
            _currentblock = null;
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            Home.GetGatherTarget(_helper);
            //�� �̻� ä���� �ڿ��� ���°��
            if (Home.NonethisResourceTypeHelper)
            {
                _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.SadEmote);
                _animator.SetBool(isMove, false);
                return BehaviorTree.ENodeStatus.InProgress;
            }

            else
            {
                var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
                if (order == _order)
                {
                    return _stack.HandItem.Count == 0 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                }
                else
                    _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
                return BehaviorTree.ENodeStatus.Succeeded;
            }
        }
        );

        #endregion
        #endregion


        #region ������ �̵��ϱ�

        var GotoStation = BTRoot.Add<BTNode_Sequence>("�����Ѱ��");
        GotoStation.Add<BTNode_Action>("������ �̵��ϱ�", () =>
        {
            PutDown();
            if (_helper.arrive)
            {
                //Home.ResetResources();
                _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.HeartEmote);
                SoundManager.Instance.PlaySoundEffect("Player_Dash");
                Vector3 position = ShopManager.Instance.nextGame.position;
                _agent.MoveTo(position);
                _agent.moveSpeed = 10;
                _animator.SetBool(isMove, true);
                return BehaviorTree.ENodeStatus.InProgress;
            }

            return BehaviorTree.ENodeStatus.Failed;
        }, () =>
        {
            if (_helper.arrive)
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            return BehaviorTree.ENodeStatus.Failed;
        });

        GotoStation.Add<BTNode_Action>("��� �ִٸ� ������ �ֱ�", () =>
        {
            Reset();
            _animator.SetBool(isMove, false);
            _agent.moveSpeed = _defaultSpeed;
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {

            return !_helper.GotoPlayer ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });


        var NeedToMove = GotoStation.Add<BTNode_Sequence>("�÷��̾ ��Ҵ��� Ȯ���ϱ�");
        NeedToMove.AddDecorator<BTDecoratorBase>("�÷��̾ ������ Ȯ���ϴ� ���ڷ�����", () =>
        {
            return !_helper.GotoPlayer;

        });

        var Shopping = NeedToMove.Add<BTNode_Sequence>("�� ��Ҵٸ�");
        Shopping.Add<BTNode_Action>("������ �ֱ�", () =>
        {
            _animator.SetBool(isMove, false);
            _agent.moveSpeed = _defaultSpeed;
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.moveSpeed == _defaultSpeed ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        Shopping.Add<BTNode_Action>("���� �ϱ�", () =>
        {
            _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.HmmEmote);
            Transform shopping = ShopManager.Instance.shopUpgradeTrainPos
                                 [UnityEngine.Random.Range(0, ShopManager.Instance.shopUpgradeTrainPos.Length)];
            _localMemory.SetGeneric<Transform>(BlackBoardKey.Shopping, shopping);

            Vector3 position = _agent.FindCloestAroundEndPosition(shopping.position);
            _animator.SetBool(isMove, true);
            _agent.MoveTo(position);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        Shopping.Add<BTNode_Action>("����ϱ�", () =>
        {
            _animator.SetBool(isMove, false);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            Transform shopping = _localMemory.GetGeneric<Transform>(BlackBoardKey.Shopping);
            Vector3 dir = shopping.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

            currentTime += Time.deltaTime;
            if (currentTime > 3)
            {
                currentTime = 0;

                return BehaviorTree.ENodeStatus.Succeeded;
            }
            return BehaviorTree.ENodeStatus.InProgress;
        });

        Shopping.Add<BTNode_Action>("���� �ϱ�", () =>
        {
            Transform shopping = ShopManager.Instance.shopNewTrainPos[UnityEngine.Random.Range(0, ShopManager.Instance.shopNewTrainPos.Length)];
            _localMemory.SetGeneric<Transform>(BlackBoardKey.Shopping, shopping);
            Vector3 position = _agent.FindCloestAroundEndPosition(shopping.position);
            _agent.MoveTo(position);
            _animator.SetBool(isMove, true);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        Shopping.Add<BTNode_Action>("����ϱ�", () =>
        {
            _animator.SetBool(isMove, false);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            Transform shopping = _localMemory.GetGeneric<Transform>(BlackBoardKey.Shopping);
            Vector3 dir = shopping.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);
            return BehaviorTree.ENodeStatus.InProgress;
        });

        #endregion


        #region ����� �ٲ���
        var OrderChange = BTRoot.Add<BTNode_Sequence>("����� �ٲ� ���");
        OrderChange.Add<BTNode_Action>("���� �������� �ʱ�ȭ", () =>
        {
            PutDown();
            var reset = Reset();
            //Ÿ���� ���ٸ� �ڱ�
            //�ִٸ� ���� ��� �����ϱ�
            return reset ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
        });

        var ErrorRoot = BTRoot.Add<BTNode_Selector>("����� ������ �� ���� ���");
        var MoveToPutDown = ErrorRoot.Add<BTNode_Sequence>("�տ� ���� ���� ��");
        MoveToPutDown.Add<BTNode_Action>("���������� ����ϱ�", () =>
        {
            if (_stack.HandItem.Count != 0)
            {
                Vector3 home = _agent.FindCloestAroundEndPosition(GoalManager.Instance.lastRail.transform.position);
                _agent.MoveTo(home);
                return BehaviorTree.ENodeStatus.InProgress;
            }

            return BehaviorTree.ENodeStatus.Failed;

        }, () =>
        {

            if (_stack.HandItem.Count != 0)
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            return BehaviorTree.ENodeStatus.Failed;
        });
        MoveToPutDown.Add<BTNode_Action>("�������� �ڱ�", () =>
        {
            if (_stack.HandItem.Count != 0)
            {
                _currentblock = _stack.BFS(this);
                _stack.PutDown();
                _target = null;
                _currentblock = null;
            }

            _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.SadEmote);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            if (order == _order)
            {
                return _stack.HandItem.Count == 0 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            else
                _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
            return BehaviorTree.ENodeStatus.Succeeded;

        });

        var CantDoAnything = ErrorRoot.Add<BTNode_Sequence>("�տ� ���� ���� ��");
        CantDoAnything.Add<BTNode_Action>("�ƹ��͵� ���ؿ�", () =>
        {
            _animator.SetBool(isMove, false);
            _emoteImage.sprite = _emoteManager.GetEmote(_emoteManager.SadEmote);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            if (order != _order)
            {
                _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            if (_helper.GotoPlayer)
            {
                return BehaviorTree.ENodeStatus.Failed;
            }
            return BehaviorTree.ENodeStatus.InProgress;
        });



    }


    private void PutDown()
    {
        if (_item != null)
        {
            Debug.Log("��������");
            _item.PickUp();
            SoundManager.Instance.StopSoundEffect("Player_ToolsDown");
            SoundManager.Instance.PlaySoundEffect("Player_ToolsDown");

            _item.transform.parent = _stack.BFS(this);
            _item.transform.rotation = Quaternion.identity;
            _item.transform.localPosition = (Vector3.up * 0.5f);
            _localMemory.SetGeneric<AI_Item>(BlackBoardKey.Item, null);
            _animator.SetBool(isDig, false);
            _animator.SetBool(isMove, false);
            _item = null;

        }
    }

    private bool Reset()
    {
        if (_target != null)
        {
            _item = null;
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);
            _target = null;
            return true;
        }

        return false;
    }

    #endregion

}
