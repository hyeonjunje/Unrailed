using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TrainType { Engine, WaterBox, ChestBox, WorkBench, StationDir, Dynamite, Spare }
public class TrainMovement : MonoBehaviour
{
    public TrainType trainType;

    public int trainNum;
   
    public Queue<RailController> rails = new Queue<RailController>();
    public List<RailController> listToQue = new List<RailController>();

    public Transform trainMesh;
    public int trainUpgradeLevel;
    [SerializeField] private int targetCount;
    [SerializeField] protected GameObject destroyParticle;
    public ParticleSystem fireEffect;

    public bool isGoal;
    public bool isBurn;
    public bool isReady;

    //게임오버
    public bool isOver;

    public float trainSpeed = 0.5f;
    public float _trainRotateSpeed;

    public float goalSpeed = 6f;
    private float _trainMoveSpeed;

    // Start is called before the first frame update
    //Transform startRayPos;
    protected void GetMesh()
    {
        if(trainType != TrainType.Spare)
        {
            trainMesh = transform.GetChild(0).GetComponent<Transform>();
            fireEffect = GetComponentInChildren<ParticleSystem>();
            destroyParticle.SetActive(false);
            trainMesh.gameObject.SetActive(true);
            trainUpgradeLevel = 1;
        }
    }

    protected void TrainMovePos()
    {
        listToQue = rails.ToList();

        if (!isReady)
        {
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

        }
        else
        {
            _trainMoveSpeed = 0;
        }

        if (rails.Count != 0)
        {
            RotateTrain();
            transform.position = Vector3.MoveTowards(transform.position, rails.Peek().transform.position, _trainMoveSpeed * Time.deltaTime);


            if (transform.position == rails.Peek().transform.position)
            {
                rails.Dequeue();
            }

            // 큐에 들어온 레일 위치로 이동
            // transform.LookAt(rails.Peek().transform.position);
            //var trainRotate = rails.Peek().transform.position;
            //Debug.Log(trainRotate);
            //transform.rotation = Quaternion.Slerp(transform.rotation, trainRotate, trainRotateSpeed * Time.deltaTime);
        }

        else
        {
            if (isGoal)
            {
                //클리어 조건
                //제일 먼저 도착하는 엔진을 멈추게 하면 뒤 따라오는 애들 전원 정지로 구현

                if (trainType == TrainType.Engine)
                {
                    isReady = true;
                    ShopManager.Instance.ShopOn();
                }
            }
            else
            {
                //오버 조건
                TrainOver();
            }

        }
    }

    public virtual void TrainUpgrade()
    {
        destroyParticle.SetActive(true);
        //destroyParticle.SetActive(false);
        //상속해서 올리기
    }
    protected void TrainOver()
    {
        if (trainType != TrainType.Spare && !isOver)
        {
            trainMesh.gameObject.SetActive(false);
            destroyParticle.SetActive(false);
            destroyParticle.SetActive(true);
            isOver = true;
        }
    }

    public void RotateTrain()
    {
        if(rails != null)
        {
            Vector3 dir = rails.Peek().transform.position - transform.position;
            if(trainMesh != null)
            trainMesh.transform.rotation = Quaternion.Lerp(trainMesh.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _trainRotateSpeed);
        }
    }
    public void EnqueueRailPos(RailController gameObject)
    {
        if (trainType != TrainType.Dynamite || trainType != TrainType.StationDir)
            //큐에 추가
            rails.Enqueue(gameObject);
    }
}
