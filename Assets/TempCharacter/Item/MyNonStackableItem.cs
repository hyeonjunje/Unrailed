using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNonStackableItem : MyItem, IMyItem
{
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public bool PickUp(int count = 0)
    {
        RePosition(player.RightHandTransform, Vector3.zero);

        return true;
    }

    public bool PutDown(int count = 0)
    {
        RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f);

        return true;
    }

    public bool CheckItemType(IMyItem otherItem)
    {
        return ItemType == otherItem.ItemType;
    }
}
