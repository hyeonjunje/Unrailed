using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStackableItem : MyItem, IMyItem
{
    [SerializeField] private float stackInterval;

    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public bool PickUp(int count = 0)
    {
        RePosition(player.TwoHandTransform, Vector3.up * count * stackInterval);

        return true;
    }

    public bool PutDown(int count = 0)
    {
        RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * count * stackInterval);

        return true;
    }

    public bool CheckItemType(IMyItem otherItem)
    {
        return itemType == otherItem.ItemType;
    }
}
