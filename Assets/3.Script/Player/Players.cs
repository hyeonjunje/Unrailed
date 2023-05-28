using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour
{

    #region 아이템 주웠을 때 위치
    [SerializeField] public Transform rightHand;//[질문] : 트랜스폼은 되고 게임오브젝트는 왜 안돼?..
    [SerializeField] public Transform betweenTwoHands;
    [SerializeField] public Transform rayStart;
    [SerializeField] public float DropDistance = 1.5f;
    #endregion



    #region 이동 관련 변수
    public float speed = 5f;
    float xAxis;
    float zAxis;
    bool isDash;
    Vector3 moveVec;
    [SerializeField] float dashCool = 0.3f;
    #endregion
    #region 아이템 픽업 관련 변수
    [SerializeField] float GapOfItems = 0.25f;
    [SerializeField] float pickUpDistance;
    [SerializeField] LayerMask BlockLayer;
    [SerializeField] LayerMask StackItemLayer;
    [SerializeField] LayerMask NonStackItemLayer;
    public bool isHaveItem;
    #endregion


    #region 버튼을 눌렀는가?
    bool isDashKeyDown;
    public bool isGetItemKeyDown;
    #endregion


    #region 컴포넌트 변수
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Item item;
    #endregion

    public List<GameObject> stackItem = new List<GameObject>();//현재 스택 아이템
    public GameObject nonStackItem;// 현재 논스택 아이템
    //public GameObject currentItem = null;
    public GameObject detectedItem = null;
    //public GameObject detectedItem;

    //private Item item;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator.SetBool("isWalk", false);
        nonStackItem = null;
        //item = GetComponent<Item>();
        isGetItemKeyDown = false;
    }

    void Update()
    {
        GetInput();
        Walk();
        Turn();
        Dash();
        FirstPickUpItem();
        DropItem();
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
            Debug.Log("");
            RaycastHit hit;
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f))
            {
                Debug.Log(hit.transform.gameObject.name);
                //item = hit.collider.GetComponent<Item>();
                GameObject hitItem = hit.transform.gameObject;
                //Item hitItem = hit.collider.GetComponent<Item>();
                if (hitItem != null)
                {
                    if (hitItem.tag == "StackItem" && stackItem.Count == 0)
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
            Debug.Log("교체하고 있어요~");
            //경우의 수
            /*
             * 소유 아이템 / 바닥 아이템
             * Stack       / Stack    -> 교체, 바닥 아이템 오른손(rightHand.position)으로, 소유 아이템 바닥(transform.position)으로
             * NonStack    / Stack    -> 교체, 바닥 아이템 오른손으로, 소유 아이템 바닥으로 [완료]
             * Stack       / NonStack -> 교체, 바닥 아이템 양손 사이로, 소유 아이템 바닥으로
             * NonStack    / NonStack -> 교체, 바닥 아이템 양손 사이로, 소유 아이템 바닥으로 [완료]
             * Stack       / Block    -> 내려놓기, 오른손에서 바닥으로
             * NonStack    / Block    -> 내려놓기, 양손에서 바닥으로
             * 
             */


            // 바닥 아이템 기준으로 if문 만들어보자.
            RaycastHit hit;
            // 바닥에 있는 아이템의 레이어가 NonStack인 경우 -> 교체
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, NonStackItemLayer))
            {
                Debug.Log("NonStackItem 감지");
                // 소유 아이템 : Stack / NonStack
                Debug.Log(hit.transform.gameObject.name + "-> 레이캐스트에 맞은 애");
                if (hit.collider)
                {
                    if (nonStackItem.tag == "StackItem")
                    {
                        /*Debug.Log($"손 : {nonStackItem.name} - 땅 : {detectedItem.name} 교체 전!!");
                        SetItemOnBlock(nonStackItem);
                        //nonStackItem = hit.transform.gameObject;
                        SetItemInRightHand(detectedItem);
                        Debug.Log($"손 : {nonStackItem.name} - 땅 : {detectedItem.name} 교체 후!!");*/

                    }
                    else if (nonStackItem.tag == "NonStackItem")
                    {
                        SetItemOnBlock(nonStackItem);
                        SetItemInRightHand(hit.transform.gameObject);
                    }
                }
            }


            // 바닥에 있는 아이템의 레이어가 Stack인 경우 -> 교체
            RaycastHit[] hits = Physics.RaycastAll(rayStart.position, Vector3.down, 10f, StackItemLayer);
            if (hits.Length > 0)
            {
                Debug.Log("StackItem 감지");
                // 소유 아이템 : Stack / NonStack

                if (nonStackItem.tag == "StackItem")
                {
                    // 소유 아이템 : 스택 / 바닥 아이템 : 스택
                    // 소유 아이템 바닥으로 보내
                    //SetItemOnBlock(nonStackItem);
                    // 바닥 아이템 양손 사이로 보내
                    //SetItemBetweenTwoHands();



                }
                if (nonStackItem.tag == "NonStackItem")
                {
                    Debug.Log("도끼와 철을 교환하고 싶어요.");
                    SetItemOnBlock(nonStackItem);
                    SetItemBetweenTwoHands(hits);
                    
                }
            }


            // 바닥에 있는 아이템의 레이어가 Block인 경우 -> 내려놓기
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
            {
                Debug.Log("바닥에 아이템이 없다");
                if (nonStackItem == null && stackItem.Count == 0)
                {
                    Debug.Log("소지한 아이템이 없어서 return 할게요.");
                    return;
                }
                else if (nonStackItem != null)
                {
                    Debug.Log("NonStackItem 버릴게요.");
                    SetItemOnBlock(nonStackItem);
                    isHaveItem = false;
                }
                else if (stackItem.Count != 0)
                {
                    Debug.Log("StackItem 버릴게요.");
                    SetItemOnBlock(stackItem);
                    
                    // 나무 - 철, 예외처리
                    isHaveItem = false;
                }
            }
            
       
        }
    }


    void SetItemInRightHand(GameObject Item)
    {
        Item.transform.SetParent(rightHand);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        nonStackItem = Item;
    }


    void SetItemBetweenTwoHands(GameObject Item)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        stackItem.Add(Item);
    }


    void SetItemBetweenTwoHands(GameObject Item, float GapOfItems)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = new Vector3(0, GapOfItems * (stackItem.Count), 0);
        Item.transform.localRotation = Quaternion.identity;
        stackItem.Add(Item);
    }


    // 땅에 스택 아이템
    void SetItemBetweenTwoHands(RaycastHit[] Item)
    {
        for (int i = 0; i < Item.Length; i++)
        {
            GameObject itemObject = Item[i].transform.gameObject;
            itemObject.transform.SetParent(betweenTwoHands);
            //itemObject.transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * GapOfItems * i);
            itemObject.transform.localPosition = new Vector3(0, GapOfItems * i, 0);
            itemObject.transform.localRotation = Quaternion.identity;
            stackItem.Add(itemObject.transform.gameObject);
        }
        //Item = new List<GameObject>(); //[질문]
    }


    void SetItemOnBlock(GameObject Item)
    {
        RaycastHit hit;
        if(Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            Item.transform.SetParent(hit.transform);
            Item.transform.localPosition = new Vector3(0, 0.5f, 0);
            Item.transform.localRotation = Quaternion.identity;
            nonStackItem = null;
        }
    }
    void SetItemOnBlock(List<GameObject> Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            for(int i=0; i< Item.Count; i++)
            {
                Debug.Log("모두 내려 놓을게");
                Item[i].transform.SetParent(hit.transform);
                Item[i].transform.localPosition = new Vector3(0, (GapOfItems*i) + 0.5f, 0);
                Item[i].transform.localRotation = Quaternion.identity;
            }
            stackItem = new List<GameObject>(); //[질문]
        }
    }

 

    private void OnTriggerEnter(Collider other)
    {
        //const string STACK_ITEM_TAG = "StackItem"; 
        if (other.tag == "StackItem")
        {
            if (stackItem.Count > 0 && stackItem.Count < 3)
            {
                SetItemBetweenTwoHands(other.gameObject, GapOfItems);
            }
        }
    }








    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StacKItem" || other.tag == "NonStackItem")
        {
            detectedItem = other.gameObject;
        }

        item = other.GetComponent<Item>();
        //Item item = other.GetComponent<Item>();
        if (item != null)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 10.0f, StackItemLayer))
            {
                if (stackItem.Count > 0 && stackItem.Count < 3)
                {
                    stackItem.Add(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "StacKItem" || other.tag == "NonStackItem")
        {
            if (detectedItem == other.gameObject)
                detectedItem = null;
        }
    }*/




    /*void DigUp()
    {
        RaycastHit hit;
        Debug.DrawRay(rayStart.position, transform.forward * pickUpDistance, Color.red);
        if (Physics.Raycast(rayStart.position, transform.forward, out hit, pickUpDistance))
        {
            IDig target = hit.collider.GetComponent<IDig>();
            if (target != null)
            {
                // item이 null이 될 경우를 생각해야 함 
                if (hit.transform.name == "Tree01(Clone)" && item.name == "ItemAxe")
                {
                    target.OnDig(hit.point);
                }
            }
        }
    }*/


}