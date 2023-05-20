using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TrainMovement : MonoBehaviour
{
    public Queue<GameObject> rails = new Queue<GameObject>();
    public List<GameObject> listToQue = new List<GameObject>();
    //[SerializeField] private GameObject[] rails;
    //public List<GameObject> rails = new List<GameObject>();
    Transform startRayPos;

    [SerializeField] private int targetCount;

    public bool isGoal;
    public float trainSpeed;
    public float goalSpeed;
    private float _trainMoveSpeed;
    private float _trainRotateSpeed;
    // Start is called before the first frame update
    private void Awake()
    {
        startRayPos = transform.GetChild(0).GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        listToQue = rails.ToList();
        if (isGoal)
        {
            _trainMoveSpeed = goalSpeed;
        }
        else
        {
            _trainMoveSpeed = trainSpeed;
        }

        TrainMovePos();
    }
    void TrainMovePos()
    {
        if(rails.Count != 0)
        {
            // 큐에 들어온 레일 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, rails.Peek().transform.position, _trainMoveSpeed * Time.deltaTime);
            transform.LookAt(rails.Peek().transform.position);
            //var trainRotate = Quaternion.LookRotation(rails.Peek().transform.position);
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
    public void EnqueueRailPos(RailController gameObject)
    {
        //rails.Add(gameObject);
        rails.Enqueue(gameObject.gameObject);
        Debug.Log("큐에 추가");
    }
}
