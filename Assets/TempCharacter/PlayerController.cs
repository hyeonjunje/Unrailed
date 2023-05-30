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

    // ����  => �̰� ��������??
    private bool _isDash = false;

    // ������Ʈ
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;

    // ����
    // ������ �ٸ��� �ȵ�
    private Stack<IMyItem> _handItem = new Stack<IMyItem>();
    private Stack<IMyItem> _detectedItem = new Stack<IMyItem>();

    // �÷��̾� ��ġ
    private float _currentSpeed;

    // ���� �� �ִ� ��
    private Transform _currentblock;
    // ���濡 �ִ� ������Ʈ
    private Transform _currentFrontObject;

    public List<IMyItem> handItemList = new List<IMyItem>();
    public List<IMyItem> detectedItemList = new List<IMyItem>();

    public List<string> handItemGameObjectList = new List<string>();
    public List<string> detectedItemGameObjectList = new List<string>();


    // ������Ƽ
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
        // �÷��̾� ������
        Move();
    }

    private void Update()
    {
        // ���� ��, ���� ��ü ����
        DetectGroundBlock();
        DetectFrontObject();

        // ������ ��ȣ�ۿ�
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // ȯ�� ��ȣ�ۿ�
        InteractiveEnvironment();

        // ���� ��ȣ�ۿ�
        InteractiveTrain();

        // ��������
        handItemList = _handItem.ToList();
        detectedItemList = _detectedItem.ToList();

        handItemGameObjectList = new List<string>();
        detectedItemGameObjectList = new List<string>();

        foreach (var go in handItemList)
            handItemGameObjectList.Add(go.ToString());

        foreach (var go in detectedItemList)
            detectedItemGameObjectList.Add(go.ToString());
        // ���� Ŭ�� ����
    }

    private void Move()
    {
        // ������, ȸ��, ��ñ���
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

    // spacebar ���� ��
    private void InteractiveItemSpace()
    {
        if(_handItem.Count != 0 && _detectedItem.Count == 0) // ������
        {
            Debug.Log("������");

            int count = 0;
            // �״�� ��������
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
        else if(_handItem.Count == 0 && _detectedItem.Count != 0) // �ݱ�
        {
            Debug.Log("�ݱ�");

            // �� ���� 3���� ������ ��
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
        else if(_handItem.Count != 0 && _detectedItem.Count != 0) // ��ü
        {
            Debug.Log("��ü");

            // ������ ������ �״´�.
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
            // ������ �ٸ��� �Ѱ���� �ٲ۴�.
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

    // �� ���� ��
    private void InteractivItem()
    {
        if(_handItem.Count != 0 && _detectedItem.Count != 0)
        {
            IMyItem item = _detectedItem.Peek();
            // ������ ������ 3���� �� ������ �״´�.
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
            // ĳ��
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
            // ĳ��
            if (_currentFrontObject == hit.transform)
                return;

            _currentFrontObject = hit.transform;
        }
    }
}
