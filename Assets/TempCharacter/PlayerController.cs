using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;
    [SerializeField] private Transform _railPreview;

    // 상태  => 이건 상태패턴??
    private bool _isDash = false;

    // 컴포넌트
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;

    // 물건
    // 종류가 다르면 안됨
    private Stack<MyItem> _handItem = new Stack<MyItem>();
    private Stack<MyItem> _detectedItem = new Stack<MyItem>();

    // 플레이어 수치
    private float _currentSpeed;

    // 현재 서 있는 블럭
    private Transform _currentblock;
    // 전방에 있는 오브젝트
    private Transform _currentFrontObject;

    Vector3[] dir = new Vector3[8] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
        new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1)};

    // 프로퍼티
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform AroundEmptyBlockTranform => BFS();


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

        _handItem = new Stack<MyItem>();
        _detectedItem = new Stack<MyItem>();
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

        // 레일 미리보기 확인
       // CheckPutDownRail();

        // 아이템 상호작용
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // 환경 상호작용
        InteractiveEnvironment();

        // 기차 상호작용
        InteractiveTrain();
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
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // 줍기
        {
            Debug.Log("줍기");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {
            Debug.Log("버리기");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count != 0) // 교체
        {
            Debug.Log("교체");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().Change(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    // 안 누를 때
    private void InteractivItem()
    {
        if(_handItem.Count != 0 && _detectedItem.Count != 0)
        {
            Debug.Log("와다닥 줍기");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().AutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
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
            _detectedItem = new Stack<MyItem>();
            for(int i = 0; i < _currentblock.childCount; i++)
            {
                MyItem item = _currentblock.GetChild(i).GetComponent<MyItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }
        }
    }

    // 레일을 이을 수 있는 상황이라면 preview 레일을 활성화시킨다.
    private void CheckPutDownRail()
    {
        if (_handItem.Count != 0 && _handItem.Peek().ItemType == EItemType.rail)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(_currentblock.position, dir[i], out RaycastHit hit, _playerStat.detectRange, _playerStat.blockLayer))
                {
                    // 현재 땅 위에 아무것도 없고 주위에 마지막 레일이 있다면
                    if (_currentblock.childCount == 0 && hit.transform.childCount != 0 && hit.transform.GetChild(0).GetComponent<RailController>() == FindObjectOfType<GoalManager>().lastRail)
                    {
                        _railPreview.gameObject.SetActive(true);
                        _railPreview.SetParent(null);
                        _railPreview.position = _currentblock.position + Vector3.up * 0.5f;
                        _railPreview.rotation = Quaternion.identity;
                        return;
                    }
                }
            }
        }
        if (_railPreview.gameObject.activeSelf)
            _railPreview.gameObject.SetActive(false);
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

    private Transform BFS()
    {
        // CurrentBlockTransform => _currentblock
        // 현재 블럭에서 가장 가까운 자식이 없는 블럭을 반환하자

        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(_currentblock);

        HashSet<Transform> hashSet = new HashSet<Transform>();
        hashSet.Add(_currentblock);

        while(queue.Count != 0)
        {
            Transform currentBlock = queue.Dequeue();

            if (currentBlock.childCount == 0)
                return currentBlock;

            for (int i = 0; i < 8; i++)
                if(Physics.Raycast(currentBlock.position, dir[i], out RaycastHit hit, 1f, _playerStat.blockLayer))
                    if(hashSet.Add(hit.transform))
                        queue.Enqueue(hit.transform);
        }

        return null;
    }
}
