using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    [SerializeField] private GameObject[] railPrefabs;
    [SerializeField] private RailController neighborRail;
    [SerializeField] private GoalManager trainManager;

    private Transform[] railChild;
    public TrainMovement[] trainComponents;
    public RailLine railLine;

    public int dirCount;
    int childCount;

    public bool isInstance;

    public bool isGoal;
    public bool isFront;
    public bool isBack;
    public bool isLeft;
    public bool isRight;
    public bool isStartRail;
    public bool isEndRail;

    private Vector3 _frontPos;
    private Vector3 _backPos;
    private Vector3 _rightPos;
    private Vector3 _leftPos;

    public float poolingTime;
    public float lifeTime = 0;
    private void Awake()
    {
        trainManager = FindObjectOfType<GoalManager>();
        railChild = this.GetComponentsInChildren<Transform>();


        childCount = gameObject.transform.childCount;
        railPrefabs = new GameObject[childCount];

        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i] = gameObject.transform.GetChild(i).gameObject;
        }

    }
    private void OnEnable()
    {
        trainManager.railCon.Add(gameObject.GetComponent<RailController>());
        if (!isEndRail && !isStartRail)
        {
            //기차이동 위치값 초기화
            //단, 골이 아닐때에만 활성화된다.
            //마지막 레일이 골에 닿았을 경우 모든 레일의 골을 해제한다.
            EnqueueRail();
        }

        if (!isGoal)
        {
            //레일을 이동시켰을 때 인식할 레이의 위치
            _frontPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
            _backPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
            _rightPos = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
            _leftPos = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
            
            //노란 레일선  초기화
            railLine = null;
            
            //위치 방향 초기화
            isFront = false;
            isBack = false;
            isLeft = false;
            isRight = false;
            
            dirCount = 0;
            
            //오브젝트 레이어 초기화
            foreach (Transform child in railChild)
            {
                child.gameObject.layer = 23;
            }
            //인식불가 bool 초기화
            
            isInstance = false;
        }

        //철로 연결
        RaycastOn();

        if (!isGoal && !isEndRail && !isStartRail)
        {
            railLine.Line.SetActive(false);
        }
    }
    //todo 05 18 앞 철로가 없으면 철로를 해제 할 수 있도록 만들어 놓을것 그리고 가능하면 - 박상연
    public void RailSwitch()
    {
        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i].SetActive(true);
            if (dirCount != i)
            {
                railPrefabs[i].SetActive(false);

            }
            else
            {
                railLine = railPrefabs[i].GetComponentInChildren<RailLine>();
            }
        }
    }
    public void RaycastOn()
    {
        //실 게임 내에서는 isStartRail 철로 2개정도 깔아두고 2개는 기본 철로. 그 후에 붙이면 정상가동
        //isEndRail은 두개만 붙여놓을것 

        RaycastHit raycastHit = new RaycastHit();

        RailDir();


        if ((Physics.Raycast(_frontPos, transform.forward, out raycastHit, 0.3f) && (!raycastHit.collider.GetComponentInParent<RailController>().isInstance))
            || (Physics.Raycast(_rightPos, transform.right, out raycastHit, 0.3f )&& !raycastHit.collider.GetComponentInParent<RailController>().isInstance) 
            || (Physics.Raycast(_backPos, -transform.forward, out raycastHit, 0.3f) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(_leftPos, -transform.right, out raycastHit, 0.3f) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance))
        {
            neighborRail = raycastHit.collider.GetComponentInParent<RailController>();

            //북 동 남 서 확인하여 isIntance를 확인
            if (neighborRail != null && 
                !neighborRail.isInstance && !neighborRail.isStartRail)
            {
                if (isFront) neighborRail.isBack = true;
                if (isRight) neighborRail.isLeft = true;
                if (isBack) neighborRail.isFront = true;
                if (isLeft) neighborRail.isRight = true;

                if(!neighborRail.isEndRail) neighborRail.isInstance = true;

                neighborRail.railDirSelet();
                neighborRail.RailSwitch();
                neighborRail.railLine.Line.SetActive(true);

                foreach (Transform child in neighborRail.railChild)
                {
                   child.gameObject.layer = 0;
                }
            }
            //북 동 남 서 확인하여 isGoal을 확인
            if (neighborRail != null && neighborRail.isEndRail && !isEndRail)
            {
                trainManager.TrainGoal();
                neighborRail.isEndRail = false;
                neighborRail.enabled = false;
                neighborRail.enabled = true;
            }
        }

        railDirSelet();
        RailSwitch();

        if (isInstance)
            railLine.Line.SetActive(true);
    }
    void RailDir()
    {
        isFront = Physics.Raycast(_frontPos, transform.forward, 0.3f, LayerMask.GetMask("Rail"));
        if (isFront) return;
        isRight = Physics.Raycast(_rightPos, transform.right, 0.3f, LayerMask.GetMask("Rail"));
        if (isRight) return;
        isBack = Physics.Raycast(_backPos, -transform.forward, 0.3f, LayerMask.GetMask("Rail"));
        if (isBack) return;
        isLeft = Physics.Raycast(_leftPos, -transform.right, 0.3f, LayerMask.GetMask("Rail"));
        if (isLeft) return;

    }
    private void railDirSelet()
    {
        if (isFront) dirCount = 5;
        if (isRight) dirCount = 0;
        if (isBack) dirCount = 6;
        if (isLeft) dirCount = 7;

        if (isFront && isRight) dirCount = 3;
        else if (isFront && isLeft) dirCount = 1;
        else if (isBack && isRight) dirCount = 4;
        else if (isBack && isLeft) dirCount = 2;
    }
    public void ResetLine()
    {
        railLine.Line.SetActive(true);
    }
    void Update()
    {
        // Debug.DrawRay(_frontPos, transform.forward * 0.3f, Color.red);
        // Debug.DrawRay(_backPos, -transform.forward * 0.3f, Color.green);
        // Debug.DrawRay(_rightPos, transform.right * 0.3f, Color.yellow);
        // Debug.DrawRay(_leftPos, -transform.right * 0.3f, Color.blue);



        if (isGoal)
        {
            lifeTime += Time.deltaTime;

            if(lifeTime >= poolingTime)
            {
                lifeTime = 0;
                transform.position = Vector3.zero;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        trainManager.railCon.Remove(gameObject.GetComponent<RailController>());
        

        if(neighborRail != null)
        {
            if (!isStartRail && !neighborRail.isStartRail && !isGoal)
            {
                neighborRail.isInstance = false;
                neighborRail.railLine.Line.SetActive(false);
            }
        }

        //혹시 몰라 이전 레일의 레이어를 되돌리는 로직도 구현해둠 필요하면 작성
        //foreach (Transform child in neighborRail.railChild)
        //{
        //
        //    child.gameObject.layer = 23;
        //}
    }

    public void EnqueueRail()
    {
        trainComponents = FindObjectsOfType<TrainMovement>();

        for (int i = 0; i < trainComponents.Length; i++)
        {
            //기차에 위치값 추가
            trainComponents[i].EnqueueRailPos(gameObject.GetComponent<RailController>());
        }
    }
}
