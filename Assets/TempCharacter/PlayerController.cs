using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;

    [Header("Object")]
    [SerializeField] private Transform _railPreview;
    [SerializeField] private GameObject _balloonObject;
    [SerializeField] private GameObject _runParticle;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bridgePrefab;

    [Header("UI")]
    [SerializeField] private WaterGauge _waterGauge;

    [Header("Particle")]
    [SerializeField] private ParticleSystem _fireeffect;
    [SerializeField] private ParticleSystem _dasheffect;
    // 상태  => 이건 상태패턴??
    private bool _isDash = false;
    private bool _isInteractive = false;
    private bool _isRespawn = false;
    private bool _isCharge = false;

    // 컴포넌트
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;
    private Animator _animator;

    // 물건
    // 종류가 다르면 안됨
    private Stack<MyItem> _handItem = new Stack<MyItem>();
    private Stack<MyItem> _detectedItem = new Stack<MyItem>();

    // 플레이어 수치
    private float _currentSpeed;
    // 현재 상호작용 쿨타임
    private float _currentInteractCoolTime;

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
    public MyItem CurrentHandItem => _handItem.Count == 0 ? null : _handItem.Peek();  // 현재 들고 있는 아이템

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStat = GetComponent<PlayerStat>();
        _runParticle.SetActive(false);
        InitPlayer();
    }

    private void InitPlayer()
    {
        _isDash = false;
        _isInteractive = false;
        _currentSpeed = _playerStat.moveSpeed;
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
        CheckPutDownRail();

        // 아이템 상호작용
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // 환경 상호작용
        DetectWater();  // 물 뜨기
        DigUp();        // 캐기
        Attack();       // 공격

        // 기차 상호작용
        InteractiveTrain();
        OffFire();      //기차 불끄기
    }

    private void Move()
    {
        if (_isRespawn)
            return;

        // 움직임, 회전, 대시까지
        if (_playerInput.IsShift && !_isDash)
        {
            _isDash = true;
            _runParticle.SetActive(true);
            _currentSpeed = _playerStat.dashSpeed;
            Invoke("DashOff", _playerStat.dashDuration);
            _dasheffect.Play();
        }

        transform.position += _playerInput.Dir * _currentSpeed * Time.deltaTime;
        transform.LookAt(_playerInput.Dir + transform.position);
    }

    private void DashOff()
    {
        _currentSpeed = _playerStat.moveSpeed;
        _runParticle.SetActive(false);
        _isDash = false;
        _dasheffect.Stop();

    }

    // spacebar 누를 때
    private void InteractiveItemSpace()
    {
        // 나무 들고 있고 앞에 물이 있을 때는 하지마

        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // 줍기
        {
            Debug.Log("줍기");
            if (_currentFrontObject != null && _currentFrontObject.gameObject.layer == LayerMask.NameToLayer("WorkBench"))
            {
                TrainWorkBench bench = _currentFrontObject.GetComponent<TrainWorkBench>();

                if (_currentFrontObject.GetComponentInChildren<RailController>() != null)
                {
                    RailController[] rail = _currentFrontObject.GetComponentsInChildren<RailController>();

                    for (int i = 0; i < rail.Length; i++)
                    {
                        Debug.Log("빼내기");
                        //레일의 부모를 다시 정적상태인 풀링으로 이동
                        rail[i].transform.parent = bench.railPool.transform;
                    }
                }
            }
            else
            {
                Pair<Stack<MyItem>, Stack<MyItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
                _handItem = p.first;
                _detectedItem = p.second;
            }
        }

        else if (_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {
            if (_currentFrontObject != null && _currentFrontObject.gameObject.layer == LayerMask.NameToLayer("Box"))
            {
                TrainBox box = _currentFrontObject.GetComponent<TrainBox>();

                if (CurrentHandItem.ItemType == EItemType.wood)
                {
                    if (box.woods.Count < box.maxItem)
                    {
                        for (int i = 0; i < _handItem.Count; i++)
                        {
                            Debug.Log("납품");
                            box.GiveMeItem(CurrentHandItem.ItemType, _handItem);
                        }
                        _handItem.Clear();

                    }
                    else return;
                }
                else if (CurrentHandItem.ItemType == EItemType.steel)
                {
                    if (box.steels.Count < box.maxItem)
                    {
                        for (int i = 0; i < _handItem.Count; i++)
                        {
                            Debug.Log("납품");
                            box.GiveMeItem(CurrentHandItem.ItemType, _handItem);
                        }
                        _handItem.Clear();
                    }
                    else return;
                }
            }
            else
            {
                Debug.Log("버리기");
                Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
                _handItem = p.first;
                _detectedItem = p.second;
            }
        }
        else if (_handItem.Count != 0 && _detectedItem.Count != 0) // 교체
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
        if (_handItem.Count != 0 && _detectedItem.Count != 0)
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
            for (int i = 0; i < _currentblock.childCount; i++)
            {
                MyItem item = _currentblock.GetChild(i).GetComponent<MyItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }

            if (_balloonObject.activeSelf)
            {
                _balloonObject.SetActive(false);
                _isRespawn = false;
            }
        }
        else
        {
            _currentblock = null;
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
        if (Physics.Raycast(_rayStartTransform.position, transform.forward, out RaycastHit hit, _playerStat.detectRange, _playerStat.detectableLayer))
        {
            if (!_isInteractive)
            {
                _currentInteractCoolTime += Time.deltaTime;

                if (_currentInteractCoolTime > _playerStat.interactiveCoolTime)
                {
                    _currentInteractCoolTime = 0;
                    _isInteractive = true;
                }
            }

            if (hit.transform.GetComponent<MyItem>() != null)
                return;

            // 캐싱
            if (_currentFrontObject == hit.transform)
                return;
            _currentFrontObject = hit.transform;

        }
        else
        {
            _currentInteractCoolTime = 0;
            _isInteractive = false;
            _currentFrontObject = null;
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

        while (queue.Count != 0)
        {
            Transform currentBlock = queue.Dequeue();

            if (currentBlock.childCount == 0)
                return currentBlock;

            for (int i = 0; i < 8; i++)
                if (Physics.Raycast(currentBlock.position, dir[i], out RaycastHit hit, 1f, _playerStat.blockLayer))
                    if (hashSet.Add(hit.transform))
                        queue.Enqueue(hit.transform);
        }

        return null;
    }

    // 손에 있는 물건을 떨구는 메소드
    private void PutDownItem()
    {
        Debug.Log(_handItem.Count);
        if (_handItem.Count != 0)
        {
            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    #region 민경이 형
    private void DetectWater()  // 물 감지
    {
        if (_currentFrontObject == null)
        {
            return;
        }

        if (_currentFrontObject.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if (CurrentHandItem != null && CurrentHandItem.ItemType == EItemType.bucket)
            {
                if (!_waterGauge.IsFillWater())
                {

                    if (!_isCharge)
                    {
                        SoundManager.Instance.PlaySoundEffect("Player_WaterImport");
                        _isCharge = true;
                    }
                    // 물 채우기
                    _waterGauge.gameObject.SetActive(true);

                    _waterGauge.FillGauge();
                }
                else
                {
                    CurrentHandItem.ActiveWater(true);
                    _isCharge = false;
                }
            }
        }
        else
        {
            if (!_waterGauge.IsFillWater())
                _waterGauge.ResetWater();
        }
    }

    private void OffFire()
    {
        if(CurrentHandItem.ItemType == EItemType.bucket && 
            _currentFrontObject.gameObject.name == "Train_Water_Tank(1)")
        {
            if(_waterGauge.IsFillWater())
            {
                // 불꺼주기 넣어야 해
                _fireeffect.Stop();

                CurrentHandItem.ActiveWater(false);
                _waterGauge.ResetWater();
            }
        }
    }

    public bool SetBridge() // 다리 놓기
    {
        if (_currentFrontObject == null)
            return false;

        if (_currentFrontObject.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if (CurrentHandItem != null && CurrentHandItem.ItemType == EItemType.wood)
            {
                Transform parent = _currentFrontObject.parent;
                Transform bridge = Instantiate(_bridgePrefab, parent).transform;
                bridge.localPosition = Vector3.up * -0.375f;
                bridge.localRotation = Quaternion.identity;

                Destroy(_currentFrontObject.gameObject);
                Destroy(_handItem.Pop().gameObject);

                return true;
            }
        }

        return false;
    }

    private void DigUp() // 캐기
    {
        if (_currentFrontObject == null)
            return;
        if (!_isInteractive)
            return;
        // 여기 애니메이션
        if (_currentFrontObject.gameObject.layer == LayerMask.NameToLayer("diggable"))
        {
            if (CurrentHandItem == null)
                return;
            ReSource resource = _currentFrontObject.GetComponent<ReSource>();
            if (resource == null)
                return;

            if (CurrentHandItem.ItemType == EItemType.axe && resource.ResourceType == EResource.tree)
            {
                resource.Dig();
                _isInteractive = false;
            }
            else if (CurrentHandItem.ItemType == EItemType.pick && resource.ResourceType == EResource.steel)
            {
                resource.Dig();
                _isInteractive = false;
            }
        }
    }

    private void Attack()  // 공격
    {
        if (_currentFrontObject == null)
            return;
        if (!_isInteractive)
            return;
        if (_currentFrontObject.gameObject.layer == LayerMask.NameToLayer("attackable"))
        {
            if (CurrentHandItem != null)
            {
                if (CurrentHandItem.ItemType == EItemType.pick || CurrentHandItem.ItemType == EItemType.axe)
                {
                    AnimalHealth animal = _currentFrontObject.GetComponent<AnimalHealth>();
                    if (animal != null)
                    {
                        animal.Hit();
                        _isInteractive = false;
                    }
                }
            }
        }
    }

    private void AddBolt(GameObject bolt) // 코일 먹기
    {
        Destroy(bolt);
    }

    public void Respawn()
    {
        _isRespawn = true;

        InitPlayer();

        // 죽으면 들고 있는거 다 놓자
        PutDownItem();

        transform.position += Vector3.up * 10f;
        _balloonObject.SetActive(true);
    }
    #endregion
}
