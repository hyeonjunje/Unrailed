using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoLoading : MonoBehaviour
{

    [SerializeField] private float loadingSpeed;
    [SerializeField] private bool isLoad;
    [SerializeField] private float loadingCharge = 0f;
    public int loadTypeIndex;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLoad)
        {
            loadingCharge += Time.deltaTime;

        }
        else
        {
            loadingCharge = 0;
        }
        ShopManager.Instance.goToLoading[loadTypeIndex].fillAmount = loadingCharge / 10;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isLoad = true;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isLoad = false;
        }
    }
}