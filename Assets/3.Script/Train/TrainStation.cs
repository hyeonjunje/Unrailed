using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStation : MonoBehaviour
{
    public ShopManager shop;
    public GameObject[] newGameStart;

    // Update is called once per frame

    private void Awake()
    {
        shop = FindObjectOfType<ShopManager>();
    }
    private void OnEnable()
    {
        // shop.endStation.Add(this.gameObject.GetComponent<TrainStation>());
    }
}
