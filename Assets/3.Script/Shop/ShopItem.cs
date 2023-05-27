using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public enum ItemType { ItemEngine, ItemWaterBox, ItemChestBox, ItemWorkBench, ItemDir, ItemDynamite }
    public ItemType itemType;
    public int itemCost;
    public int itemIdx;
    public bool _isSpawn = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (itemType == ItemType.ItemDir || itemType == ItemType.ItemDynamite)
            ShopManager.Instance.newCarList.Add(this);

    }
    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        _isSpawn = true;

    }
    public void TradeItem(Collider col)
    {
        ShopManager.Instance.trainCoin--;

        TrainMovement train = col.GetComponent<TrainMovement>();

        if (train.trainType != TrainMovement.TrainType.Spare)
        {
            train.trainUpgradeLevel++;
        }
        else
        {
            TrainSpare spare = col.GetComponent<TrainSpare>();
            spare.ChangeTrain(itemIdx);
        }

        _isSpawn = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isSpawn && (itemType == ItemType.ItemDir || itemType == ItemType.ItemDynamite))
        {
            if (other.CompareTag("Train") )
            {
                TrainSpare trains = other.GetComponent<TrainSpare>();

                if (trains.trainIndex == 0)
                {
                    trains.ResetTrain(1); 
                }
                else if (trains.trainIndex == 1)
                {
                    trains.ResetTrain(0);
                }

                TradeItem(other);
            }

            else if (other.CompareTag("ShopItem"))
            {
                ShopItem items = other.GetComponent<ShopItem>();


                if ((items.itemType == ItemType.ItemDir || items.itemType == ItemType.ItemDynamite) && items._isSpawn)
                {
                    other.gameObject.SetActive(false);
                    other.gameObject.SetActive(true);
                }
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
