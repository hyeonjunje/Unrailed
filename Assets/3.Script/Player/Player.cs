using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ������ �ֿ��� �� ��ġ
    [SerializeField] public Transform rightHand;
    [SerializeField] public Transform betweenTwoHands;
    [SerializeField] public Transform rayStart;
    [SerializeField] WaterGauge waterGauge;
    [SerializeField] public float DropDistance = 1.5f;
    #endregion
    #region �̵� ���� ����
    public float speed = 5f;
    float xAxis;
    float zAxis;
    bool isDash;
    Vector3 moveVec;
    [SerializeField] float dashCool = 0.3f;
    #endregion
    #region ������ �Ⱦ� ���� ����
    private Item item;
    public GameObject WaterMesh;
    [SerializeField] float GapOfItems = 0.25f;
    [SerializeField] float pickUpDistance;
    [SerializeField] LayerMask BlockLayer;
    [SerializeField] LayerMask waterlayer;
    [SerializeField] Transform ItemPrefab;
    [SerializeField] LayerMask StackItemLayer;
    [SerializeField] LayerMask NonStackItemLayer;
    public List<GameObject> currentStackItem = new List<GameObject>();
    public GameObject currentNonStackItem;
    #endregion
    #region ��ư�� �����°�?
    bool isDashKeyDown;
    public bool isGetItemKeyDown;
    #endregion
    #region �޼��尡 ���� �������ΰ�?
    public bool isHaveItem;
    #endregion
    #region ������Ʈ ����
    private Rigidbody _rigidbody;
    private Animator _animator;
    #endregion
    Rigidbody rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentNonStackItem = null;
        isGetItemKeyDown = false;
        _animator.SetBool("isWalk", false);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        DropItem();
        DigUp();
        DetectWater();
        FirstPickUpItem();
        SetBridge();
    }


    private void FixedUpdate()
    {
        Walk();
        Turn();
        Dash();
    }


    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        zAxis = Input.GetAxisRaw("Vertical");
        isDashKeyDown = Input.GetButtonDown("Dash");
        isGetItemKeyDown = Input.GetButtonDown("getItem");
    }

    void Walk()
    {
        if (!isDash) moveVec = new Vector3(xAxis, 0, zAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
        _animator.SetBool("isWalk", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(moveVec + transform.position);
    }

    void Dash()
    {
        if (isDashKeyDown && !isDash)
        {
            isDash = true;
            speed *= 2;
            Invoke("DashOff", dashCool);
        }
    }

    void DashOff()
    {
        speed *= 0.5f;
        isDash = false;
    }

   
    public void FirstPickUpItem()
    {

        if (isGetItemKeyDown && !isHaveItem)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f))
            {
                GameObject hitItem = hit.transform.gameObject;
                if (hitItem != null)
                {
                    if (hitItem.tag == "StackItem" && currentStackItem.Count == 0)
                    {
                        SetItemBetweenTwoHands(hitItem);
                        isHaveItem = true;
                    }
                    else if (hitItem.tag == "NonStackItem")
                    {
                        SetItemInRightHand(hitItem);
                        isHaveItem = true;
                    }
                }
            }
            isGetItemKeyDown = false;
        }
    }
    public void DropItem()
    {
        if (isGetItemKeyDown && isHaveItem)
        {
            #region DropItem ����� ��
            /*
             * ���� ������ / �ٴ� ������
             * Stack       / Stack    -> ��ü, �ٴ� ������ ������(rightHand.position)����, ���� ������ �ٴ�(transform.position)����
             * NonStack    / Stack    -> ��ü, �ٴ� ������ ����������, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����
             * NonStack    / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / Block    -> ��������, �����տ��� �ٴ�����
             * NonStack    / Block    -> ��������, ��տ��� �ٴ�����
             */
            #endregion

            RaycastHit hit;
            RaycastHit[] hits = Physics.RaycastAll(rayStart.position, Vector3.down, 10f, StackItemLayer);

            // �ٴ� ������ ���̾� : NonStack
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, NonStackItemLayer))
            {
                GameObject rayHitNonStackItem = hit.transform.gameObject;
                //NonStack / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����[�Ϸ�]
                if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    SetItemInRightHand(rayHitNonStackItem);
                }
                //Stack / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����
                else if (currentStackItem.Count != 0)
                {
                    SetItemOnBlock(currentStackItem);
                    SetItemInRightHand(rayHitNonStackItem);
                }
            }

            // �ٴڿ� �ִ� �������� ���̾ Stack�� ��� -> ��ü
            else if (hits.Length > 0)
            {
                if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    SetItemBetweenTwoHands(hits);
                }
                else if (currentStackItem.Count != 0)
                {
                    SetItemOnBlock(currentStackItem);
                    SetItemBetweenTwoHands(hits);
                }

            }

            // �ٴڿ� �ִ� �������� ���̾ Block�� ��� -> ��������
            else if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
            {
                if (currentNonStackItem == null && currentStackItem.Count == 0)
                {
                    Debug.Log("������ �������� ��� return �ҰԿ�.");
                    return;
                }
                else if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    isHaveItem = false;
                }
                else if (currentStackItem.Count != 0)
                {
                    SetItemOnBlock(currentStackItem);
                    isHaveItem = false;
                }
            }

        }
    }
    #region ������ ��ġ -> ������
    void SetItemInRightHand(GameObject Item)
    {
        Item.transform.SetParent(rightHand);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        currentNonStackItem = Item;
    }
    #endregion
    #region ������ ��ġ -> ��� ����
    void SetItemBetweenTwoHands(GameObject Item)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        currentStackItem.Add(Item);
    }

    void SetItemBetweenTwoHands(GameObject Item, float GapOfItems)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = new Vector3(0, GapOfItems * (currentStackItem.Count), 0);
        Item.transform.localRotation = Quaternion.identity;
        currentStackItem.Add(Item);
    }

    void SetItemBetweenTwoHands(RaycastHit[] Item)
    {
        for (int i = 0; i < Item.Length; i++)
        {
            GameObject itemObject = Item[i].transform.gameObject;
            itemObject.transform.SetParent(betweenTwoHands);
            //itemObject.transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * GapOfItems * i);
            itemObject.transform.localPosition = new Vector3(0, GapOfItems * i, 0);
            itemObject.transform.localRotation = Quaternion.identity;
            currentStackItem.Add(itemObject.transform.gameObject);
        }
    }
    #endregion
    #region ������ ��ġ -> �ٴ�
    void SetItemOnBlock(GameObject Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            Item.transform.SetParent(hit.transform);
            Item.transform.localPosition = new Vector3(0, 0.5f, 0);
            Item.transform.localRotation = Quaternion.identity;
            currentNonStackItem = null;
        }
    }
    void SetItemOnBlock(List<GameObject> Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer ))
        {
            for (int i = 0; i < Item.Count; i++)
            {
                Item[i].transform.SetParent(hit.transform);
                Item[i].transform.localPosition = new Vector3(0, (GapOfItems * i) + 0.5f, 0);
                Item[i].transform.localRotation = Quaternion.identity;
            }
            currentStackItem = new List<GameObject>(); //[����]
        }
    }
    #endregion
    #region ���� ������ �ڵ� ����
    private void OnTriggerEnter(Collider other)
    {
        //const string STACK_ITEM_TAG = "StackItem"; 
        /*if (other.tag == "StackItem")
        {*/
        if (currentStackItem.Count > 0 && currentStackItem.Count < 3)
        {
            if (currentStackItem[0].gameObject.name == other.gameObject.name)
            {
                SetItemBetweenTwoHands(other.gameObject, GapOfItems);
            }
        }
        /*}*/
    }
    #endregion

    void DetectWater()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * pickUpDistance, Color.red);
        //if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpDistance, waterlayer))
        {
                Debug.Log("�� ����");
                waterGauge.gameObject.SetActive(true);
                waterGauge.FillGauge();
            if (WaterMesh.activeSelf)
            {
                waterGauge.watergauge.gameObject.SetActive(false);
            }

        }
        else
        {
            waterGauge.gameObject.SetActive(false);
            waterGauge.StopFilling();

        }
    }
    void DigUp()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.TransformPoint(0, 0.1f, 0), transform.forward * pickUpDistance * 0.5f, Color.red);
        if (Physics.Raycast(transform.TransformPoint(0, 0.1f, 0), transform.forward, out hit, pickUpDistance))
        {
            Debug.Log("�߻�" + hit.transform.name);
            IDig target = hit.collider.GetComponent<IDig>();

            if (target != null)
            {
                if (hit.transform.name == "Tree01(Clone)" &&  currentNonStackItem.name == "ItemAxe(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "Tree02(Clone)" && currentNonStackItem.name == "ItemAxe(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "ItemIron00(Clone)" && currentNonStackItem.name == "ItemPick(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "ItemIron01(Clone)" && currentNonStackItem.name == "ItemPick(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "ItemIron02(Clone)" && currentNonStackItem.name == "ItemPick(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "ItemIron03(Clone)" && currentNonStackItem.name == "ItemPick(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else if (hit.transform.name == "Iron(Clone)" && currentNonStackItem.name == "ItemPick(Clone)")
                {
                    _animator.SetBool("isDig", true);
                    StartCoroutine(DigCoroutine(target, hit.point));
                }
                else
                {
                    _animator.SetBool("isMove", true);
                    _animator.SetBool("isDig", false);
                }
            }
            else
            {
                _animator.SetBool("isMove", true);
                _animator.SetBool("isDig", false);
            }
        }
        else
        {
            _animator.SetBool("isMove", true);
            _animator.SetBool("isDig", false);
        }
    }

    private IEnumerator DigCoroutine(IDig target, Vector3 hitPoint)
    {
        float digTime = 1.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < digTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            if (!IsPlayerLookingAtTarget(hitPoint))
            {
                _animator.SetBool("isMove", true);
                _animator.SetBool("isDig", false);
                yield break; 
            }
        }

        target.OnDig(hitPoint);
        _animator.SetBool("isDig", false);
    }

    private bool IsPlayerLookingAtTarget(Vector3 targetPosition)
    {
        Vector3 playerToTarget = targetPosition - transform.position;
        float angle = Vector3.Angle(transform.forward, playerToTarget);
        return angle <= 30f; 
    }

    void SetBridge()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            Debug.DrawRay(transform.TransformPoint(0, 0.1f, 0), transform.forward * pickUpDistance, Color.red);
            for (int i = 0; i < currentStackItem.Count; i++)
            {
                if (Physics.Raycast(transform.TransformPoint(0, 0.1f, 0), transform.forward, out hit, pickUpDistance))
                {
                    if (hit.transform.name == "Water(Clone)" && currentStackItem[i].name == "ItemWood(Clone)")
                    {
                        SpawnBridge(hit.transform);
                    }

                }
            }
        }
    }
    void SpawnBridge(Transform WhiteBlock)
    {
        Transform parent = WhiteBlock.parent;

        Transform NewRiver = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NewRiver.parent = WhiteBlock.parent;
        NewRiver.localPosition = (new Vector3(0, -0.5f, 0));
        NewRiver.localRotation = Quaternion.identity;
        Destroy(WhiteBlock.gameObject);

    }
}

      
    
