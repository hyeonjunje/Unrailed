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
    protected bool _isPlay;

    //���ӿ���
    public bool isOver;

    public float trainSpeed = 0.5f;
    public float _trainRotateSpeed;

    public float goalSpeed = 6f;
    public float _trainMoveSpeed;

    [SerializeField] private GameObject warningIcon;

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

            if (trainType != TrainType.WorkBench || trainType != TrainType.StationDir || trainType != TrainType.Dynamite)
            {
                warningIcon = transform.GetChild(2).gameObject;
                warningIcon.SetActive(false);
            }
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

            // ť�� ���� ���� ��ġ�� �̵�
            // transform.LookAt(rails.Peek().transform.position);
            //var trainRotate = rails.Peek().transform.position;
            //Debug.Log(trainRotate);
            //transform.rotation = Quaternion.Slerp(transform.rotation, trainRotate, trainRotateSpeed * Time.deltaTime);
        }

        else
        {
            if (isGoal)
            {
                //Ŭ���� ����
                //���� ���� �����ϴ� ������ ���߰� �ϸ� �� ������� �ֵ� ���� ������ ����

                if (trainType == TrainType.Engine)
                {
                    isReady = true;
                    ShopManager.Instance.ShopOn();
                }
            }
            else
            {
                //���� ����
                TrainOver();
            }

        }
    }

    public virtual void TrainUpgrade()
    {
        destroyParticle.SetActive(true);
        //destroyParticle.SetActive(false);
        //����ؼ� �ø���
    }
    protected void TrainOver()
    {
        if (trainType != TrainType.Spare && !isOver)
        {
            StopCoroutine(Warning());
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
            //ť�� �߰�
            rails.Enqueue(gameObject);
    }

    public IEnumerator Warning()
    {
        _isPlay = true;
        warningIcon.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningIcon.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        _isPlay = false;
    }
}
