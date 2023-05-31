using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRailItem : MyItem, IMyItem
{
    [SerializeField] private float stackInterval;
    [SerializeField] private LayerMask blockLayer;
    private RailController railController;
    private PlayerController player;

    private void Awake()
    {
        railController = GetComponent<RailController>();
        player = FindObjectOfType<PlayerController>();
    }

    public bool PickUp(int count = 0)
    {
        RePosition(player.TwoHandTransform, Vector3.up * count * stackInterval);

        return true;
    }

    public bool PutDown(int count = 0)
    {
        // 내려놓을 때 앞뒤좌우에 가장 끝 레일이 있으면
        // 하나 놓고 false 반환
        RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * count * stackInterval);

        RailController arounRail = DetectRail();
        if (arounRail != null)
        {
            arounRail.PutRail();
            railController.PutRail();
            return false;
        }

        return true;
    }
    public bool CheckItemType(IMyItem otherItem)
    {
        return itemType == otherItem.ItemType;
    }

    private RailController DetectRail()
    {
        Vector3[] dir = new Vector3[4] {Vector3.forward, Vector3.right, 
                                        Vector3.left, Vector3.back};

        for(int i = 0; i < dir.Length; i++)
        {
            if(Physics.Raycast(player.CurrentBlockTransform.position, dir[i], out RaycastHit hit, 1f, blockLayer))
            {
                if(hit.transform.childCount > 0)
                {
                    RailController railController = hit.transform.GetChild(0).GetComponent<RailController>();
                    return railController;
                }
            }
        }
        return null;
    }
}
