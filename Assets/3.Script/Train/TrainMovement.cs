using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Cinemachine.CinemachineVirtualCamera[] cameraTarget;
    [SerializeField] private int targetCount;
    [SerializeField] protected GameObject destroyParticle;
    public ParticleSystem fireEffect;

    public bool isGoal;
    public bool isBurn;
    public bool isReady;
    protected bool _isPlay;

    //게임오버
    public bool isOver;

    public float trainSpeed = 0.5f;
    public float _trainRotateSpeed;

    public float goalSpeed = 6f;
    public float _trainMoveSpeed;

    [SerializeField] private GameObject warningIcon;
    public Animator overText;
    private InGameScene manager;

    // Start is called before the first frame update
    //Transform startRayPos;
    protected void GetMesh()
    {
        if (trainType != TrainType.Spare)
        {
            trainMesh = transform.GetChild(0).GetComponent<Transform>();
            fireEffect = GetComponentInChildren<ParticleSystem>();
            destroyParticle.SetActive(false);
            trainMesh.gameObject.SetActive(true);
            isReady = true;
            trainUpgradeLevel = 1;

            if (trainType != TrainType.WorkBench || trainType != TrainType.StationDir || trainType != TrainType.Dynamite)
            {
                warningIcon = transform.GetChild(2).gameObject;
                warningIcon.SetActive(false);
            }
        }

        cameraTarget = GameObject.FindGameObjectWithTag("Cinemachine").GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
        overText = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<Animator>();
        manager = FindObjectOfType<InGameScene>();
        trainSpeed = 0.1f;

        if (trainType == TrainType.Engine)
        {
            cameraTarget[0].Follow = transform;
        }
    }
    private void Start()
    {
        CameraSwitch(1);
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
            return;
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
                    if(!isReady)
                    {
                        isReady = true;
                        // 역 도착하기
                        FindObjectOfType<InGameScene>().ArriveStation();

                        // ShopManager.Instance.ShopOn();
                    }
                }
            }
            else
            {
                //오버 조건
                TrainOver();
            }

        }
    }
    public void CameraSwitch(int num)
    {

        if(num== 2)
        {
            cameraTarget[1].Follow = GoalManager.Instance.lastRail.transform;
        }
        for (int i = 0; i < cameraTarget.Length; i++)
        {
            cameraTarget[i].gameObject.SetActive(true);
            if (i != num -1)
            {
                cameraTarget[i].gameObject.SetActive(false);
            }
        }
    }
    public virtual void TrainUpgrade()
    {
        destroyParticle.SetActive(true);
        //destroyParticle.SetActive(false);
        //상속해서 올리기
    }
    public void TrainOver()
    {
        if (trainType != TrainType.Spare && !isOver)
        {
            StopCoroutine(Warning());
            SoundManager.Instance.PlaySoundEffect("Train_Broken");
            trainMesh.gameObject.SetActive(false);
            destroyParticle.SetActive(false);
            destroyParticle.SetActive(true);
            isOver = true;
        }
        if(trainType == TrainType.Spare)
        {
            StartCoroutine(OverAnim());
        }
    }
    public IEnumerator OverAnim()
    {
        CameraSwitch(3);
        yield return new WaitForSeconds(2.5f);
        overText.SetBool("GameOver", true);
        yield return new WaitForSeconds(1.5f);
        manager._loadingSceneUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("IntroScene");
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
