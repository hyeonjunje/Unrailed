using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TrainMovement : MonoBehaviour
{
    float time = 0;
    public Queue<RailController> rails = new Queue<RailController>();
    public List<RailController> listToQue = new List<RailController>();

    Transform startRayPos;
    public Transform trainMesh;

    [SerializeField] private int targetCount;

    public bool isGoal;
    public float trainSpeed;
    public float _trainRotateSpeed;

    public float goalSpeed;
    private float _trainMoveSpeed;
    // Start is called before the first frame update
    private void Awake()
    {
        startRayPos = transform.GetChild(0).GetComponent<Transform>();
        trainMesh = transform.GetChild(0).GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        listToQue = rails.ToList();
        if (isGoal)
        {
            _trainMoveSpeed = goalSpeed;
            _trainRotateSpeed = goalSpeed;
        }
        else
        {
            _trainMoveSpeed = trainSpeed;
            _trainRotateSpeed = trainSpeed *2;
        }

        TrainMovePos();
    }
    void TrainMovePos()
    {
        if(rails.Count != 0)
        {
            // 큐에 들어온 레일 위치로 이동
            RotateTrain();
            transform.position = Vector3.MoveTowards(transform.position, rails.Peek().transform.position, _trainMoveSpeed * Time.deltaTime);
            // transform.LookAt(rails.Peek().transform.position);
            //var trainRotate = rails.Peek().transform.position;
            //Debug.Log(trainRotate);
            //transform.rotation = Quaternion.Slerp(transform.rotation, trainRotate, trainRotateSpeed * Time.deltaTime);

            if (transform.position == rails.Peek().transform.position)
            {
                rails.Dequeue();
            }
        }

        else
        {
            Destroy(gameObject);
        }

    }

    public void RotateTrain()
    {
        if(rails != null)
        {
            Vector3 dir = rails.Peek().transform.position - transform.position;
            trainMesh.transform.rotation = Quaternion.Lerp(trainMesh.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _trainRotateSpeed);
        }
    }
    public void EnqueueRailPos(RailController gameObject)
    {
        //rails.Add(gameObject);
        rails.Enqueue(gameObject);
        Debug.Log("큐에 추가");
    }
}
