using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRailItem : MyItem
{
    private RailController railController;
    protected override void Awake()
    {
        base.Awake();
        railController = GetComponent<RailController>();
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // 종류가 같다면 줍는다.
        if (handItem.Peek().CheckItemType(detectedItem.Peek()))
        {
            // 연결되어 있지 않는 레일이라면 
            if(!detectedItem.Peek().CheckConnectedRail())
            {
                while (handItem.Count < 3)
                {
                    if (detectedItem.Count == 0)
                        break;

                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                }
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // handItem은 무조건 railItem

        EItemType detectedItemType = detectedItem.Peek().ItemType;
        if (detectedItemType == EItemType.axe || detectedItemType == EItemType.pick || detectedItemType == EItemType.bucket)
        {
            MyItem temp = detectedItem.Pop();
            while (handItem.Count != 0)
            {
                if(CheckConnectedRail())
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    break;
                }
                else
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }

            if(handItem.Count != 0)
            {
                Debug.Log("다른데 내려놓는다.");
            }
            else
            {
                handItem.Push(temp);
                handItem.Peek().RePosition(player.RightHandTransform, Vector3.zero);
            }
        }
        else if (detectedItemType == EItemType.wood || detectedItemType == EItemType.steel)
        {
            // 여기 만약 이어질 수 있는 곳이라면 레일 하나만 놓고 나머지는 다른데 놓아야함
            if (handItem.Count <= 3 && detectedItem.Count <= 3)
            {
                Stack<MyItem> temp = new Stack<MyItem>(handItem);
                handItem.Clear();
                while (detectedItem.Count != 0)
                {
                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                }
                while (temp.Count != 0)
                {
                    detectedItem.Push(temp.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }
        }
        else if (detectedItemType == EItemType.rail)
        {
            if (!CheckConnectedRail())
            {
                while (handItem.Count != 0)
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        if(!detectedItem.Peek().CheckConnectedRail())
        {
            for (int i = 0; i < 3; i++)
            {
                if (detectedItem.Count == 0)
                    break;
                handItem.Push(detectedItem.Pop());
                handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * stackInterval);
            }
        }
        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        if (!CheckConnectedRail())
        {
            while (handItem.Count != 0)
            {
                detectedItem.Push(handItem.Pop());
                detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
            }
        }
        else
        {
            detectedItem.Push(handItem.Pop());
            detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);

            // 여기서 내려놓음
            detectedItem.Peek().GetComponent<RailController>().PutRail();
            Debug.Log(GetConnectedRail());
            // GetConnectedRail()?.PutRail();
        }
        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    private RailController GetConnectedRail()
    {
        RailController result = null;

        Vector3[] dir = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };
        for (int i = 0; i < dir.Length; i++)
        {
            if (Physics.Raycast(player.CurrentBlockTransform.position, dir[i], out RaycastHit hit, 1f, blockLayer))
            {
                if (hit.transform.childCount > 0)
                {
                    if(result == null)
                        result = hit.transform.GetChild(0).GetComponent<RailController>();
                }
            }
        }
        return result;
    }
}
