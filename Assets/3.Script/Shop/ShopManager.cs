using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<TrainStation> endStation = new List<TrainStation>();

    [SerializeField] private Animator anim;
    private TrainWater trainWater;
    private TrainEngine trainEngine;
    [SerializeField] int readyCount;


    [SerializeField] GameObject test;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        trainWater = FindObjectOfType<TrainWater>();
        trainEngine = FindObjectOfType<TrainEngine>();
    }

    public void ShopOn()
    {
        transform.position = endStation[0].transform.GetChild(1).transform.position;
        anim.SetBool("isReady",true);
    }

    public void ShopOff()
    {
        endStation[0].newGameStart[0].SetActive(false);
        endStation[0].newGameStart[1].SetActive(true);
        endStation.Clear();
        anim.SetBool("isReady", false);
        trainEngine.anim.SetBool("CountDown", true);
        StartCoroutine(TrainStartMove());
        trainEngine.anim.SetBool("CountDown", false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(false);
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block")|| other.CompareTag("Item") || other.CompareTag("Items"))
        {
            GameObject obj = other.transform.GetChild(0).gameObject;
            obj.SetActive(true);
        }
    }

    public IEnumerator TrainStartMove()
    {
        yield return new WaitForSeconds(readyCount);
        //5초 지나는 ui 효과
        trainEngine.isGoal = false;
        trainEngine.isReady = false;
        trainWater.FireOff();
        test.SetActive(true);
    }
}
