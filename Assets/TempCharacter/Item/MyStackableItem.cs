using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStackableItem : MyItem
{
    public override Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // 종류가 같다면 줍는다.
        if(handItem.Peek().CheckItemType(detectedItem.Peek()))
        {
            while(handItem.Count < 3)
            {
                if (detectedItem.Count == 0)
                    break;

                handItem.Push(detectedItem.Pop());
                handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // handItem은 무조건 StackableItem

        EItemType detectedItemType = detectedItem.Peek().ItemType;
        if (detectedItemType == EItemType.axe || detectedItemType == EItemType.pick || detectedItemType == EItemType.bucket)
        {
            MyItem temp = detectedItem.Pop();
            while(handItem.Count != 0)
            {
                detectedItem.Push(handItem.Pop());
                detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
            }
            handItem.Push(temp);
            handItem.Peek().RePosition(player.RightHandTransform, Vector3.zero);
        }
        else if (detectedItemType == EItemType.wood || detectedItemType == EItemType.steel)
        {
            if (handItem.Peek().CheckItemType(detectedItem.Peek()))
            {
                while (handItem.Count != 0)
                {
                    detectedItem.Push(handItem.Pop());
                    detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                }
            }
            else
            {
                if(handItem.Count <= 3 && detectedItem.Count <= 3)
                {
                    Stack<MyItem> temp = new Stack<MyItem>(handItem);
                    handItem.Clear();
                    while(detectedItem.Count != 0)
                    {
                        handItem.Push(detectedItem.Pop());
                        handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                    }
                    while(temp.Count != 0)
                    {
                        detectedItem.Push(temp.Pop());
                        detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
                    }
                }
                else
                {
                    // 핸드아이템은 다른데 두고
                    // detected아이템을 한계까지 든다.
                    int count = 0;
                    Transform aroundTransform = player.AroundEmptyBlockTranform;
                    while (handItem.Count != 0)
                    {
                        handItem.Peek().RePosition(aroundTransform, Vector3.up * 0.5f + Vector3.up * count++ * stackInterval);
                        handItem.Pop();
                    }

                    while(handItem.Count < 3)
                    {
                        if (detectedItem.Count == 0)
                            break;
                        handItem.Push(detectedItem.Pop());
                        handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * (handItem.Count - 1) * stackInterval);
                    }
                }
            }
        }
        else if (detectedItemType == EItemType.rail)
        {
            if (!CheckConnectedRail())
            {
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
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        for(int i = 0; i < 3; i++)
        {
            if (detectedItem.Count == 0)
                break;
            handItem.Push(detectedItem.Pop());
            handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * stackInterval);
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        while(handItem.Count != 0)
        {
            detectedItem.Push(handItem.Pop());
            detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * stackInterval);
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }
}
