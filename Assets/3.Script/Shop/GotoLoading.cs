using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GotoLoading : MonoBehaviour
{

    [SerializeField] private float loadingSpeed;
    [SerializeField] private bool isLoad;
    [SerializeField] private float loadingCharge = 0f;
    public int loadTypeIndex;

    private bool _isGoto;
    // Start is called before the first frame update
    void OnEnable()
    {
        loadingCharge = 0;
        isLoad = false;
        _isGoto = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLoad)
        {
            loadingCharge = 0;
         
        }
        else
        {
            loadingCharge += Time.deltaTime * loadingSpeed;

            if (loadingCharge >= 10)
            {
                SelectNextGame(loadTypeIndex);
            }
        }
        ShopManager.Instance.goToLoading[loadTypeIndex].fillAmount = loadingCharge / 10;
    }

    private void SelectNextGame(int selectIdx)
    {
        if(selectIdx >= 1)  //Go to Lobby
        {
            _isGoto = true;
            isLoad = false;
            loadingCharge = 0;

            //저장하고 나가기
            //SceneManager.sceneLoaded('');

        }
        else //Go to Next Game
        {
            _isGoto = true;
            isLoad = false;
            loadingCharge = 0;
            ShopManager.Instance.ShopOff();
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < ShopManager.Instance.goToLoading.Length; i++)
        {
            ShopManager.Instance.goToLoading[i].fillAmount = 0;
        }
        _isGoto = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !_isGoto)
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