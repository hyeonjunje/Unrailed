using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey HP = new BlackBoardKey() { Name = "HP" };

        public string Name;
    }

    private Blackboard<BlackBoardKey> _localMemory;
    private AnimalHealth _health;
    protected readonly int isRoot = Animator.StringToHash("isRoot");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _stack = GetComponent<AI_Stack>();
        _health = GetComponent<AnimalHealth>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
        _animator.SetBool(isMove, true);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");

        BTRoot.AddService<BTServiceBase>("��ĥ �� ã�� Service", (float deltaTime) =>
        {
            Home.TargettoSteal(_agent);
        });

        var HaveTarget = BTRoot.Add<BTNode_Sequence>("��ĥ �� �ֳ���?");
        HaveTarget.AddDecorator<BTDecoratorBase>("��ĥ �� �ִ��� �Ÿ��� DECO", () =>
        {
            //Ÿ���� ������ true ������ false
            var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
            return hp == _health.CurrentHp && !Home.NonethisResourceTypeEnemy;
        });

        #region �ִ� ���

        var MainSequence = HaveTarget.Add<BTNode_Sequence>("�ִ� ���");

        MainSequence.Add<BTNode_Action>("Ÿ�� ���ϱ�", () =>
         {
             return BehaviorTree.ENodeStatus.InProgress;
        }
         , () =>
         {
            _target = Home.TargettoSteal(_agent);
             return _target == null ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Succeeded;

         });


        MainSequence.Add<BTNode_Action>("�̵��ϱ�",
        () =>
        {
            return _agent.MoveTo(_target.transform.position) ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;

        },
            () =>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        MainSequence.Add<BTNode_Action>("��ġ��",
        () =>
        {
            if(_target!=null)
            {
                _animator.SetBool(isMove, false);
                _animator.SetBool(isRoot, true);
                _stack.EnemyDetectGroundBlock(_target);
                //ó�� ��� �� 
                if (_stack.HandItem.Count == 0)
                {
                    _stack.EnemyInteractiveItem();
                }
                //�� �� �ױ�
                else
                {
                    if (!_stack.HandItem.Peek().EnemyCheckItemType)
                    {
                        _stack.EnemyInteractiveAuto();

                    }
                }
                return BehaviorTree.ENodeStatus.InProgress;

            }
            return BehaviorTree.ENodeStatus.Failed;

        },
         () =>
         {
             if (_target != null)
             {
                 return BehaviorTree.ENodeStatus.Succeeded;

             }
             else
                 return BehaviorTree.ENodeStatus.InProgress;
         });

        MainSequence.Add<BTNode_Action>("�ױ�",
            ()=>
            {
                return BehaviorTree.ENodeStatus.InProgress;

            },()=>
            {
                //�ڿ��� �� �̻� ���ٸ� 
                if (Home.NonethisResourceTypeEnemy || _stack.HandItem.Peek().EnemyCheckItemType)
                {
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                //�� �� ������� �ű��
                {
                    return _stack.HandItem.Count == 3? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
                }

            });

        MainSequence.Add<BTNode_Action>("����",
        () =>
        {
            Vector3 pos = _agent.MoveToClosestEndPosition();
            _animator.SetBool(isMove, true);
            return BehaviorTree.ENodeStatus.InProgress;

        },
        () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });


        //��������

        MainSequence.Add<BTNode_Action>("������",
            () =>
            {
                SoundManager.Instance.PlaySoundEffect("Enemy_Laugh");
                _animator.SetBool(isRoot, false);
                _target = null;
                _stack.EnemyThrowResource();
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return _stack.HandItem.Count ==0 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            );
        #endregion

        // ���°��
        var Attacked = BTRoot.Add<BTNode_Sequence>("����");
        Attacked.Add<BTNode_Action>("���� ����", () =>
         {
             var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
             //�տ� ���� �ִµ� ���� ���
             if (_stack.HandItem.Count!=0 && hp !=_health.CurrentHp)
             {
                _currentblock = _stack.BFS(this);

                _animator.SetBool(isMove, true);
                _animator.SetBool(isRoot, false);
                _target = null;
                _stack.EnemyPutDown();
                return BehaviorTree.ENodeStatus.InProgress;
             }
             //�տ� ���� ���µ� ���� ���
             else if (_stack.HandItem.Count == 0 && hp != _health.CurrentHp)
             {
                 _animator.SetBool(isMove, true);
                 _animator.SetBool(isRoot, false);
                 _target = null;
                 return BehaviorTree.ENodeStatus.InProgress;
             }

             return BehaviorTree.ENodeStatus.Failed;

         }, () =>
         {
             var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
             Debug.Log(hp);
             return _stack.HandItem.Count ==0 ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;
         });


        var ThrowResource = BTRoot.Add<BTNode_Sequence>("������ �ƴѵ� �� �̻� �ڿ��� ���� ���");
        ThrowResource.Add<BTNode_Action>("������ �̵�", () =>
        {
            var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
            if (_stack.HandItem.Count != 0 && hp == _health.CurrentHp)
            {
                Vector3 pos = _agent.MoveToClosestEndPosition();
                _animator.SetBool(isMove, true);
                return BehaviorTree.ENodeStatus.InProgress;
            }
            return BehaviorTree.ENodeStatus.Failed;

        }, () =>
        {
            return  _agent.AtDestination? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        ThrowResource.Add<BTNode_Action>("������", () =>
        {
            SoundManager.Instance.PlaySoundEffect("Enemy_Laugh");
            _animator.SetBool(isRoot, false); 
            _animator.SetBool(isMove, false);
            _target = null;
            _stack.EnemyThrowResource();
            return BehaviorTree.ENodeStatus.InProgress;

        }, () =>
        {
            return _stack.HandItem.Count == 0 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });




        //���°��

        var WanderRoot = BTRoot.Add<BTNode_Sequence>("��ǥ �� ã��");
        WanderRoot.Add<BTNode_Action>("������ �̵���",
        () =>
        {
            _animator.SetBool(isRoot, false);
            _animator.SetBool(isMove, true);
            _agent.MoveToRandomPosition();
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            if(_agent.AtDestination)
            {
                _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            else
            {
                return BehaviorTree.ENodeStatus.InProgress;
            }
        });

        var IdleRoot = MainSequence.Add<BTNode_Sequence>("������ �� ���� ���� ���");
        IdleRoot.Add<BTNode_Action>("������ �ֱ�", () =>
        {
            _animator.SetBool(isMove, false);
            return BehaviorTree.ENodeStatus.InProgress;
        });
    }
}
