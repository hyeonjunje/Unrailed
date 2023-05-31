using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;

    // 상태  => 이건 상태패턴??
    private bool _isDash = false;

    // 컴포넌트
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;

    // 물건
    // 종류가 다르면 안됨
    private Stack<IMyItem> _handItem = new Stack<IMyItem>();
    private Stack<IMyItem> _detectedItem = new Stack<IMyItem>();

    // 플레이어 수치
    private float _currentSpeed;

    // 현재 서 있는 블럭
    private Transform _currentblock;
    // 전방에 있는 오브젝트
    private Transform _currentFrontObject;

    public List<IMyItem> handItemList = new List<IMyItem>();
    public List<IMyItem> detectedItemList = new List<IMyItem>();

    public List<string> handItemGameObjectList = new List<string>();
    public List<string> detectedItemGameObjectList = new List<string>();


    // 프로퍼티
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;
    public Transform CurrentBlockTransform => _currentblock;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();


        _playerStat = GetComponent<PlayerStat>();

        InitPlayer();
    }

    private void InitPlayer()
    {
        _isDash = false;
        _currentSpeed = _playerStat.moveSpeed;

        _handItem = new Stack<IMyItem>();
        _detectedItem = new Stack<IMyItem>();
    }

    private void FixedUpdate()
    {
        // 플레이어 움직임
        Move();
    }

    private void Update()
    {
        // 현재 땅, 전방 물체 감지
        DetectGroundBlock();
        DetectFrontObject();

        // 아이템 상호작용
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // 환경 상호작용
        InteractiveEnvironment();

        // 기차 상호작용
        InteractiveTrain();

        // 보지마슈
        handItemList = _handItem.ToList();
        detectedItemList = _detectedItem.ToList();

        handItemGameObjectList = new List<string>();
        detectedItemGameObjectList = new List<string>();

        foreach (var go in handItemList)
            handItemGameObjectList.Add(go.ToString());

        foreach (var go in detectedItemList)
            detectedItemGameObjectList.Add(go.ToString());
        // 보면 클남 ㅋㅋ
    }

    private void Move()
    {
        // 움직임, 회전, 대시까지
        if(_playerInput.IsShift && !_isDash)
        {
            _isDash = true;
            _currentSpeed = _playerStat.dashSpeed;
            Invoke("DashOff", _playerStat.dashDuration);
        }

        transform.position += _playerInput.Dir * _currentSpeed * Time.deltaTime;
        transform.LookAt(_playerInput.Dir + transform.position);
    }

    private void DashOff()
    {
        _currentSpeed = _playerStat.moveSpeed;
        _isDash = false;
    }

    // spacebar 누를 때
    private void InteractiveItemSpace()
    {
        if(_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {
            Debug.Log("버리기");

            int count = 0;
            // 그대로 내려놓음
            while(_handItem.Count != 0)
            {
                IMyItem item = _handItem.Peek();
                if (item.PutDown(count++))
                {
                    _handItem.Pop();
                    _detectedItem.Push(item);
                }
                else
                {
                    break;
                }
            }
        }
        else if(_handItem.Count == 0 && _detectedItem.Count != 0) // 줍기
        {
            Debug.Log("줍기");

            // 내 손이 3개일 때까지 듬
            int count = 0;
            while(_handItem.Count != 3)
            {
                if (_detectedItem.Count == 0)
                    break;

                IMyItem item = _detectedItem.Peek();
                if (item.PickUp(count++))
                {
                    _detectedItem.Pop();
                    _handItem.Push(item);
                }
                else
                {
                    break;
                }
            }
        }
        else if(_handItem.Count != 0 && _detectedItem.Count != 0) // 교체
        {
            Debug.Log("교체");

            // 종류가 같으면 쌓는다.
            if(_handItem.Peek().CheckItemType(_detectedItem.Peek()))
            {
                int count = _detectedItem.Count;
                while (_handItem.Count != 0)
                {
                    if(_handItem.Peek().PutDown(count++))
                    {
                        _detectedItem.Push(_handItem.Pop());
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // 종류가 다르면 한계까지 바꾼다.
            else
            {
                if (_handItem.Count <= 3 && _detectedItem.Count <= 3)
                {
                    int count = 0;

                    Stack<IMyItem> tempHandItem = new Stack<IMyItem>(_detectedItem);
                    _detectedItem.Clear();

                    while (_handItem.Count != 0)
                    {
                        if (_handItem.Peek().PutDown(count++))
                        {
                            _detectedItem.Push(_handItem.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    count = 0;

                    while (tempHandItem.Count != 0)
                    {
                        if(tempHandItem.Peek().PickUp(count++))
                        {
                            _handItem.Push(tempHandItem.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    // 안 누를 때
    private void InteractivItem()
    {
        if(_handItem.Count != 0 && _detectedItem.Count != 0)
        {
            Debug.Log("와다닥 줍기");

            IMyItem item = _detectedItem.Peek();
            // 종류가 같으면 3개가 될 때까지 쌓는다.
            if(_handItem.Peek().CheckItemType(item))
            {
                while(true)
                {
                    if (_handItem.Count >= 3 || _detectedItem.Count == 0)
                        break;

                    if(item.PickUp(_handItem.Count))
                    {
                        _detectedItem.Pop();
                        _handItem.Push(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void InteractiveEnvironment()
    {

    }

    private void InteractiveTrain()
    {

    }

    private void DetectGroundBlock()
    {
        if (Physics.Raycast(_rayStartTransform.position, Vector3.down, out RaycastHit hit, _playerStat.detectRange, _playerStat.blockLayer))
        {
            // 캐싱
            if (_currentblock == hit.transform)
                return;

            _currentblock = hit.transform;
            _detectedItem = new Stack<IMyItem>();
            for(int i = 0; i < _currentblock.childCount; i++)
            {
                IMyItem item = _currentblock.GetChild(i).GetComponent<IMyItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }
        }
    }

    private void DetectFrontObject()
    {
        if (Physics.Raycast(_rayStartTransform.position, transform.forward, out RaycastHit hit, _playerStat.detectRange))
        {
            // 캐싱
            if (_currentFrontObject == hit.transform)
                return;

            _currentFrontObject = hit.transform;
        }
    }
}
