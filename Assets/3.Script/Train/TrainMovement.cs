using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TrainMovement : MonoBehaviour
{
    public int trainNum;
   
    public Queue<RailController> rails = new Queue<RailController>();
    public List<RailController> listToQue = new List<RailController>();

    public Transform trainMesh;

    [SerializeField] private int targetCount;
    [SerializeField] protected GameObject destroyParticle;

    public ParticleSystem fireEffect;

    public bool isGoal;
    public bool isBurn;
   

    public float trainSpeed = 0.5f;
    public float _trainRotateSpeed;

    public float goalSpeed = 6f;
    private float _trainMoveSpeed;

    // Start is called before the first frame update
    //Transform startRayPos;
    protected void GetMesh()
    {
        trainMesh = transform.GetChild(0).GetComponent<Transform>();
        //destroyParticle = transform.GetChild(1).GetComponent<GameObject>();
        fireEffect = GetComponentInChildren<ParticleSystem>();
        destroyParticle.SetActive(false);
        trainMesh.gameObject.SetActive(true);
    }
   


    protected void TrainMovePos()
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
            _trainRotateSpeed = trainSpeed * 2;
        }


        if (rails.Count != 0)
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
            TrainOver();
        }
    }
    protected void TrainOver()
    {
        trainMesh.gameObject.SetActive(false);
        destroyParticle.SetActive(true);
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
