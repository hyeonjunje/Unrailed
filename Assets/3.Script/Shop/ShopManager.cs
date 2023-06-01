using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance = null;

    public List<TrainStation> endStation = new List<TrainStation>();
    public List<ShopItem> newCarList = new List<ShopItem>();
  

    public TrainMovement[] trains;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform[] shopUpgradeTrainPos;
    [SerializeField] private Transform[] shopEnginePos;
    public Transform[] shopNewTrainPos;

    [SerializeField] private Text[] shopUpgradeText;
    [SerializeField] private Text[] shopEngineText;
    public Text[] shopNewTrainText;

    public ShopItem[] trainPrefabs;
    public ShopItem[] trainNewPrefabs;
    public ShopItem[] enginePrefabs;

    private TrainWater trainWater;
    private TrainEngine trainEngine;

    [SerializeField] private int readyCount;

    public int trainCoin;
    public int itemIdx;


    [SerializeField] private GameObject test;

    bool _isShop;

    public Image[] goToLoading;

    private void Awake()
    {
        //싱글톤 패턴
        if(Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

        trains = FindObjectsOfType<TrainMovement>();
        anim = GetComponentInParent<Animator>();
        trainWater = FindObjectOfType<TrainWater>();
        trainEngine = FindObjectOfType<TrainEngine>();
    }


    public void ResetTrains()
    {
        trains = FindObjectsOfType<TrainMovement>();
    }
    public void RandItemSpawn()
    {
        for (int i = 0; i < shopUpgradeTrainPos.Length; i++)
        {
            GameObject shopUpObj = Instantiate(trainPrefabs[i].gameObject, shopUpgradeTrainPos[i].position, shopUpgradeTrainPos[i].rotation, shopUpgradeTrainPos[i]);
            shopUpObj.transform.GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < shopNewTrainPos.Length; i++)
        {
            GameObject shopNewObj = Instantiate(trainNewPrefabs[i].gameObject, shopNewTrainPos[i].position, shopNewTrainPos[i].rotation, shopNewTrainPos[i]);
            shopNewObj.transform.GetChild(0).gameObject.SetActive(false);
        }

        GameObject shopEngineObj = Instantiate(enginePrefabs[0].gameObject, shopEnginePos[0].position, shopEnginePos[0].rotation, shopEnginePos[0]);
        shopEngineObj.transform.GetChild(0).gameObject.SetActive(false);

        TrainCost();
    } //위치에 스폰
    public void TrainCost()
    {
        foreach(TrainMovement train in trains)
        {
            switch (train.trainType)
            {
                    //==========================================Upgrade
                case TrainType.WaterBox:
                    trainPrefabs[0].itemCost = train.trainUpgradeLevel;
                    shopUpgradeText[0].text = $"{train.trainUpgradeLevel}";
                    break;
   
                case TrainType.WorkBench:
                    trainPrefabs[1].itemCost = train.trainUpgradeLevel;
                    shopUpgradeText[1].text = $"{train.trainUpgradeLevel}";

                    break;

                case TrainType.ChestBox:
                    trainPrefabs[2].itemCost = train.trainUpgradeLevel;
                    shopUpgradeText[2].text = $"{train.trainUpgradeLevel}";
                    break;

                    //==========================================New Car


                case TrainType.StationDir:
                    trainNewPrefabs[0].itemCost = train.trainUpgradeLevel;
                    shopNewTrainText[0].text = $"{train.trainUpgradeLevel}";
                    break;

                case TrainType.Dynamite:
                    trainNewPrefabs[1].itemCost = train.trainUpgradeLevel;
                    shopNewTrainText[1].text = $"{train.trainUpgradeLevel}";
                    break;

                    //==========================================Engine

                case TrainType.Engine:
                    enginePrefabs[0].itemCost = train.trainUpgradeLevel;
                    shopEngineText[0].text = $"{train.trainUpgradeLevel}";
                    break;
            }
        }
    } // 코스트 출력 
    public void ShopOn()
    {
        if (!_isShop)
        {
            RandItemSpawn();
            _isShop = true;
        }
        // anim.gameObject.transform.position = endStation[0].transform.GetChild(1).transform.position;
        anim.gameObject.transform.position = endStation[0].transform.position;
        anim.SetBool("isReady",true);
    }  // 상점 오픈 애니메이션

    public void ShopOff()
    {
        endStation[0].newGameStart[0].SetActive(false);
        endStation[0].newGameStart[1].SetActive(true);
        endStation.Clear();
        anim.SetBool("isReady", false);
        StartCoroutine(TrainStartMove());
    } //상점 클로즈 애니메이션

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Invisible"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }

        /*if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }*/
        if (other.CompareTag("ShopItem"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Invisible"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }

        /*if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }*/
        if (other.CompareTag("ShopItem"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }
    }

    public IEnumerator TrainStartMove() // 열차 시작 카운트다운
    {
        yield return new WaitForSeconds(1f);
        trainEngine.anim.SetBool("CountDown", true);

        yield return new WaitForSeconds(0.1f);

        trainEngine.anim.SetBool("CountDown", false);
        yield return new WaitForSeconds(readyCount);
        //5초 지나는 ui 효과
        trainEngine.isGoal = false;
        trainEngine.isReady = false;
        _isShop = false;
        trainWater.FireOff();
        test.SetActive(true);
  
    }




}
