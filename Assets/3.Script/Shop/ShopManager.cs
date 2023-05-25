using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public List<TrainStation> endStation = new List<TrainStation>();

    [SerializeField] private Transform[] shopUpgradeTrainPos;
    [SerializeField] private Transform[] shopNewTrainPos;
    [SerializeField] private Transform[] shopEnginePos;

    [SerializeField] private Text[] shopUpgradeText;
    [SerializeField] private Text[] shopNewTrainText;
    [SerializeField] private Text[] shopEngineText;

    public GameObject[] trainPrefabs;
    public GameObject[] trainNewPrefabs;
    public GameObject[] enginePrefabs;

    [SerializeField] private Animator anim;
    private TrainWater trainWater;
    private TrainEngine trainEngine;
    [SerializeField] int readyCount;


    [SerializeField] GameObject test;

    bool _isShop;
    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        trainWater = FindObjectOfType<TrainWater>();
        trainEngine = FindObjectOfType<TrainEngine>();
    }

    public void RandItemSpawn()
    {
        for (int i = 0; i < shopUpgradeTrainPos.Length; i++)
        {
            Instantiate(trainPrefabs[i], shopUpgradeTrainPos[i].position, Quaternion.identity);
        }
        for (int i = 0; i < shopNewTrainPos.Length; i++)
        {
            Instantiate(trainNewPrefabs[i], shopNewTrainPos[i].position, Quaternion.identity);
        }
        Instantiate(enginePrefabs[0], shopEnginePos[0].position, Quaternion.identity);
    }
    public void ShopOn()
    {
        if (!_isShop)
        {
            RandItemSpawn();
            _isShop = true;
        }
        anim.gameObject.transform.position = endStation[0].transform.GetChild(1).transform.position;
        anim.SetBool("isReady",true);
    }

    public void ShopOff()
    {
        endStation[0].newGameStart[0].SetActive(false);
        endStation[0].newGameStart[1].SetActive(true);
        endStation.Clear();
        anim.SetBool("isReady", false);
        StartCoroutine(TrainStartMove());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }
        if (other.CompareTag("ShopItem"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }
        if (other.CompareTag("ShopItem"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }
    }

    public IEnumerator TrainStartMove()
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
