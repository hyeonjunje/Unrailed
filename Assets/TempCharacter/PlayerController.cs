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

    [Header("Prefabs")]
    [SerializeField] private GameObject _bridgePrefab;

    [Header("UI")]
    [SerializeField] private WaterGauge _waterGauge;

    [Header("Particle")]
    [SerializeField] private ParticleSystem _fireeffect;
    [SerializeField] private ParticleSystem _dasheffect;
    // ����  => �̰� ��������??
    private bool _isDash = false;
    private bool _isInteractive = false;
    private bool _isRespawn = false;

    // ������Ʈ
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;
    private Animator _animator;

    // ����
    // ������ �ٸ��� �ȵ�
    private Stack<MyItem> _handItem = new Stack<MyItem>();
    private Stack<MyItem> _detectedItem = new Stack<MyItem>();

    // �÷��̾� ��ġ
    private float _currentSpeed;
    // ���� ��ȣ�ۿ� ��Ÿ��
    private float _currentInteractCoolTime;

    // ���� �� �ִ� ��
    private Transform _currentblock;
    // ���濡 �ִ� ������Ʈ
    private Transform _currentFrontObject;

    Vector3[] dir = new Vector3[8] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
        new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1)};

    // ������Ƽ
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform AroundEmptyBlockTranform => BFS();
    public MyItem CurrentHandItem => _handItem.Count == 0 ? null : _handItem.Peek();  // ���� ��� �ִ� ������

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
        _isInteractive = false;
        _currentSpeed = _playerStat.moveSpeed;
    }

    private void FixedUpdate()
    {
        // �÷��̾� ������
        Move();
    }

    private void Update()
    {
        // ���� ��, ���� ��ü ����
        DetectGroundBlock();
        DetectFrontObject();

        // ���� �̸����� Ȯ��
        CheckPutDownRail();

        // ������ ��ȣ�ۿ�
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // ȯ�� ��ȣ�ۿ�
        DetectWater();  // �� �߱�
        DigUp();        // ĳ��
        Attack();       // ����

        // ���� ��ȣ�ۿ�
        InteractiveTrain();
        OffFire();      //���� �Ҳ���
    }

    private void Move()
    {
        if (_isRespawn)
            return;

        // ������, ȸ��, ��ñ���
        if(_playerInput.IsShift && !_isDash)
        {
            _isDash = true;
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
        _isDash = false;
        _dasheffect.Stop();

    }

    // spacebar ���� ��
    private void InteractiveItemSpace()
    {
        // ���� ��� �ְ� �տ� ���� ���� ���� ������

        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // �ݱ�
        {
            Debug.Log("�ݱ�");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count == 0) // ������
        {
            Debug.Log("������");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count != 0) // ��ü
        {
            Debug.Log("��ü");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().Change(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    // �� ���� ��
    private void InteractivItem()
    {
        if(_handItem.Count != 0 && _detectedItem.Count != 0)
        {
            Debug.Log("�ʹٴ� �ݱ�");

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
            // ĳ��
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

            if(_balloonObject.activeSelf)
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

    // ������ ���� �� �ִ� ��Ȳ�̶�� preview ������ Ȱ��ȭ��Ų��.
    private void CheckPutDownRail()
    {
        if (_handItem.Count != 0 && _handItem.Peek().ItemType == EItemType.rail)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(_currentblock.position, dir[i], out RaycastHit hit, _playerStat.detectRange, _playerStat.blockLayer))
                {
                    // ���� �� ���� �ƹ��͵� ���� ������ ������ ������ �ִٸ�
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
            if(!_isInteractive)
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

            // ĳ��
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
        // ���� ������ ���� ����� �ڽ��� ���� ���� ��ȯ����

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

    // �տ� �ִ� ������ ������ �޼ҵ�
    private void PutDownItem()
    {
        Debug.Log(_handItem.Count);
        if(_handItem.Count != 0)
        {
            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    #region �ΰ��� ��
    private void DetectWater()  // �� ����
    {
        if (_currentFrontObject == null)
        {
            return;
        }

        if(_currentFrontObject.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if(CurrentHandItem != null && CurrentHandItem.ItemType == EItemType.bucket)
            {
                if(!_waterGauge.IsFillWater())
                {
                    // �� ä���
                    _waterGauge.gameObject.SetActive(true);
                    _waterGauge.FillGauge();
                }
                else
                {
                    CurrentHandItem.ActiveWater(true);
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
                // �Ҳ��ֱ� �־�� ��
                _fireeffect.Stop();

                CurrentHandItem.ActiveWater(false);
                _waterGauge.ResetWater();
            }
        }
    }

    public bool SetBridge() // �ٸ� ����
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

    private void DigUp() // ĳ��
    {
        if (_currentFrontObject == null)
            return;
        if (!_isInteractive)
            return;
        // ���� �ִϸ��̼�
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
            else if(CurrentHandItem.ItemType == EItemType.pick && resource.ResourceType == EResource.steel)
            {
                resource.Dig();
                _isInteractive = false;
            }
        }
    }

    private void Attack()  // ����
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
                    if(animal != null)
                    {
                        animal.Hit();
                        _isInteractive = false;
                    }
                }
            }
        }
    }

    private void AddBolt(GameObject bolt) // ���� �Ա�
    {
        Destroy(bolt);
    }

    public void Respawn()
    {
        _isRespawn = true;

        InitPlayer();

        // ������ ��� �ִ°� �� ����
        PutDownItem();

        transform.position += Vector3.up * 10f;
        _balloonObject.SetActive(true);
    }
    #endregion
}
