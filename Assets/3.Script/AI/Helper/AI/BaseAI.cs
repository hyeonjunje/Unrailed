using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] protected Transform _rayStartTransform;
    [SerializeField] protected Transform _rightHandTransform;
    [SerializeField] protected Transform _twoHandTransform;


    public Transform RayStartTransfrom => _rayStartTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;

    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Transform _currentblock;

    protected AI_Stack _stack;
    protected WorldResource _target;

    protected Animator _animator;
    protected int isMove = Animator.StringToHash("isMove");

    public Resource Home;

}
