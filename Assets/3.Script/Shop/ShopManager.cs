using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance = null;

    public List<ShopItem> newCarList = new List<ShopItem>();

    public TrainMovement[] trains;

    [SerializeField] private Animator anim;
    [SerializeField] public Transform[] shopUpgradeTrainPos;
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

    bool _isShop;
    public Image[] goToLoading;

    public Transform currentStation = null;
    public Transform nextGame;

    public bool isPlayerShop;
    private void Awake()
    {
        //�̱��� ����
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        anim = GetComponentInParent<Animator>();
        trains = FindObjectsOfType<TrainMovement>();
        trainWater = FindObjectOfType<TrainWater>();
        trainEngine = FindObjectOfType<TrainEngine>();
    }

    public void StartTrainMove()
    {
        trains = FindObjectsOfType<TrainMovement>();
        trainWater = FindObjectOfType<TrainWater>();
        trainEngine = FindObjectOfType<TrainEngine>();

        StartCoroutine(TrainStartMove());
    }

    public void ShopOn()
    {
        if (!_isShop)
        {
            RandItemSpawn();

            isPlayerShop = true;
            _isShop = true;
        }

        //Debug.Log("�������� ��ġ �̵�");
        anim.transform.position = currentStation.position;

        anim.SetBool("isReady", true);
        SoundManager.Instance.audioSourdEngine.Stop();
        SoundManager.Instance.StopAllSound();
        SoundManager.Instance.PlaySoundBgm("Shop_Bgm");
    }  // ���� ���� �ִϸ��̼�

    public void ShopOff()
    {
        trainEngine.CameraSwitch(1);
        anim.SetBool("isReady", false);
        StartCoroutine(TrainStartMove());
    } //���� Ŭ���� �ִϸ��̼�

    public void ResetTrains()
    {
        trains = FindObjectsOfType<TrainMovement>();
    }

    private void RandItemSpawn()
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
    } //��ġ�� ����

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
    } // �ڽ�Ʈ ��� 

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Invisible"))
        {
            // ��
            if(other.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                other.GetComponent<InvisibleBlock>().UnShow();
            }
            else if(other.gameObject.layer == LayerMask.NameToLayer("Rail"))
            {
                if (!other.GetComponent<RailController>().isInstance)
                    for (int i = 0; i < other.transform.childCount; i++)
                        other.transform.GetChild(i).gameObject.SetActive(false);
            }
            // ���� ������ ������, ��������
            else
            {
                other.transform.GetChild(0).gameObject.SetActive(false);
                // other.gameObject.SetActive(false);
            }
        }

        // ��, ����, ���� ���
        if(other.CompareTag("InvisibleObject"))
        {
            other.transform.GetChild(0).gameObject.SetActive(false);
        }

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
            if (other.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                other.GetComponent<InvisibleBlock>().Show();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Rail"))
            {
                if (!other.GetComponent<RailController>().isInstance)
                    other.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                other.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        // ��, ����, ���� ���
        if (other.CompareTag("InvisibleObject"))
        {
            other.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (other.CompareTag("ShopItem"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }
    }

    private IEnumerator TrainStartMove() // ���� ���� ī��Ʈ�ٿ�
    {
        SoundManager.Instance.StopSoundBgm("Shop_Bgm");
        isPlayerShop = false;
        trainEngine.isReady = true;
        yield return new WaitForSeconds(10f);
        trainEngine.anim.SetBool("CountDown", true);

        yield return new WaitForSeconds(0.1f);

        trainEngine.anim.SetBool("CountDown", false);

        for (int i = 0; i < readyCount; i++)
        {
            SoundManager.Instance.PlaySoundEffect("Train_Count");
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlaySoundEffect("Train_Start");
        trainEngine.isGoal = false;
        trainEngine.isReady = false;
        _isShop = false;
        trainWater.FireOff();
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.audioSourdEngine.Play();
        SoundManager.Instance.PlaySoundBgm("InGame_Bgm");
        
    } 
}
